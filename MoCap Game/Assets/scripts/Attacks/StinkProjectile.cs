//This script handles the stink projectile. The stink projectile is a "lobbed" projectile (lobbed is a term meaning the attack is not targetted, but instead travels in a vertical arc
//and generally take an amount of time before reaching its destination). The projectile can be interrupted by hitting another collider before reaching its target. Upon impact
//this projectile explodes

using UnityEngine;
using System.Collections;

public class StinkProjectile : MonoBehaviour
{
	[SerializeField] float speed = 20f;				//How fast the projectile travels
	[SerializeField] AnimationCurve arc;			//An animation curve laying out the path of the projectile
	[SerializeField] ParticleSystem trailParticles;	//Reference to a particle effect
	[SerializeField] StinkHit stinkHit;				//Reference to the stink hit game object

	Vector3 startPoint;								//Where the projectile starts
	Vector3 endPoint;								//Where the projectile wants to end
	bool isFlying;									//Is the projectile currently flying?

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Find the trailPartciles particle system component
		trailParticles = GetComponent<ParticleSystem>();
	}

	//Called by the StinkAttack script
	public void StartPath(Vector3 start, Vector3 end)
	{
		//The projectile is now flying
		isFlying = true;
		//Record the projectile's start point and desired end point
		startPoint = start;
		endPoint = end;
		//Start the FollowPath() coroutine
		StartCoroutine("FollowPath");
	}

	//This coroutine controls the stink projectile while it travels to its destination
	IEnumerator FollowPath()
	{
		//Turn the trail particles off while we move the projectile to its initial position.
		//Otherwise, we would see the particles shoot across the screen when we fired the attack
		trailParticles.Stop(true);
		//Move the projectile to the start position
		transform.position = startPoint;
		//Calculate the vector of the path and...
		Vector3 pathVector = endPoint - startPoint;
		//...determine the total distance the projectile has to cover
		float totalDistance = pathVector.magnitude;
		//The projectile currently hasn't traveled any distance
		float traveledDistance = 0f;
		//Now that the projectile is positioned, turn the particles back on
		trailParticles.Play(true);

		//While the projectile hasn't traveled far enough...
		while (totalDistance - traveledDistance > 0f)
		{
			//...calculate how far the projectile has traveled...
			traveledDistance += speed * Time.deltaTime;
			//...calculate a new position based on where the projectile started and how far it has traveled...
			Vector3 newPosition = startPoint + (pathVector.normalized * traveledDistance);
			//...move the projectile up so that it flies in an arc...
			float arcHeight = arc.Evaluate(traveledDistance / totalDistance);
			newPosition.y += arcHeight;
			//...move the projectile to the new calculated position
			transform.position = newPosition;
			//...and finally, wait until the next frame
			yield return null;
		}

		//Once the projectile has traveled far enough, explode!
		Explode();
	}

	void Explode()
	{
		//Projectile is no longer flying
		isFlying = false;
		//Move the stink hit to the landing spot of the projectile
		stinkHit.transform.position = transform.position;
		//Enable the stink hit
		stinkHit.gameObject.SetActive(true);
		//Disable this game object
		gameObject.SetActive(false);
	}

	//Called when a collider enter this trigger collider. If this projectile hits
	//anything while it is traveling, it will explode early
	void OnTriggerEnter(Collider other)
	{
		//If the projectile is flying, explode!
		if(isFlying)
			Explode();
	}
}
