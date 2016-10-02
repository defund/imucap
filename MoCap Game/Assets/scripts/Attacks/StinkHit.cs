//This script handles the stink hit "splash" attack (splash attack is a term meaning the attack hits all enemies in a radius, or "splashes" all over an area). The splash hit
//will play a graphical effect and cause enemies in the area to go running from the player.

using UnityEngine;

public class StinkHit : MonoBehaviour
{
	[SerializeField] float explosionRadius = 3f;	//Radius of the explosion
	[SerializeField] float explosionDuration = 4f;	//How long the explosion effect plays
	[SerializeField] LayerMask whatIsShootable;		//LayerMask determining what this explosion can effectr

	Collider[] enemiesHit;							//An array holding a collection of enemy colliders

	//When this game object is enabled, it immediately explodes
	void OnEnable()
	{
		//Create a Physics.OverlapSphere which tests the volume of a sphere for any colliders. Like a raycast, this
		//can be set to only find colliders on certain layers. The colliders hit are stored in our enemiesHit array
		enemiesHit = Physics.OverlapSphere(transform.position, explosionRadius, whatIsShootable);
		//Loop through the array of enemy colliders
		for (int i = 0; i < enemiesHit.Length; i++)
		{
			//try to get a reference to an EnemyMovement script off of the colliders
			EnemyMovement enemyMovement = enemiesHit[i].GetComponent<EnemyMovement>();

			//If the EnemyMovement script exists, tell the enemy to run away
			if (enemyMovement != null)
				enemyMovement.Runaway();
		}

		//Call the StopExploding() method after a set period of time
		Invoke("StopExploding", explosionDuration);
	}

	//This method tells enemies that were hit to stop running away
	void StopExploding()
	{
		//Loop through the array of enemy colliders
		for (int i = 0; i < enemiesHit.Length; i++)
		{
			//try to get a reference to an EnemyMovement script off of the colliders
			EnemyMovement enemyMovement = enemiesHit[i].GetComponent<EnemyMovement>();

			//If the EnemyMovement script exists, tell the enemy to come back
			if (enemyMovement != null)
				enemyMovement.ComeBack();
		}
		//Turn this game object off
		gameObject.SetActive(false);
	}
}
