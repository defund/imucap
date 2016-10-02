//This script handles the graphical "lightning bolt" that is a part of the lightning attack. Only graphical
//effects are handled in this script. All functional attack mechanics are handled in the LightningAttack script

using UnityEngine;

public class LightningBolt : MonoBehaviour
{
	[HideInInspector] public Vector3 EndPoint;		//The end point of the lightning bolt

	[Header("Bolt Properties")]
	[SerializeField] float rayHeight = 2.0f;		//How high the bolt is off the "ground" (if the floor is not at 0, then this needs to be changed)
	[SerializeField] float effectDuration = .75f;	//How long does the bolt last
	[SerializeField] float phaseDuration = .1f;		//How long does the bolt stay in each "phase" (the zig-zag of the bolt)

	[Header("Bolt rendering")]
	[SerializeField] LineRenderer rayRenderer;		//Reference to a line renderer component
	[SerializeField] AnimationCurve[] rayPhases;	//Array of animation curves used to draw the different bolt phases

	int phaseIndex = 0;								//The phase our bolt currently is on
	float timeToChangePhase;						//When the bolt will change phase
	float timeSinceEffectStarted;					//How long the bolt has been alive
	Vector3 vectorOfBolt;							//The relative vector of the bolt

	//When the bolt is enabled, reset the time variables
	void OnEnable()
	{
		timeToChangePhase = 0f;
		timeSinceEffectStarted = 0f;
	}

	void Update()
	{
		//Accumulate how much time has passed since this bolt was activated
		timeSinceEffectStarted += Time.deltaTime;
		//If the bolt has been alive long enough deactivate it
		if(timeSinceEffectStarted >= effectDuration)
			gameObject.SetActive(false);

		//Calculate the relative vector of the bolt by subtracting its end point from where the firing point of the player
		//currently is
		vectorOfBolt = EndPoint - transform.position;

		//If it's time to change phases...
		if (timeSinceEffectStarted >= timeToChangePhase)
		{
			//...determine the new time to change phases and...
			timeToChangePhase = timeSinceEffectStarted + phaseDuration;
            //...change the phase
			ChangePhase();
		}
	}

	//This method calculates the seemingly random appearance of a bolt of lightning
	void ChangePhase()
	{
		//Increase our phase index
		phaseIndex++;
		//If the phase index is larger than our array reset it back to 0
		if (phaseIndex >= rayPhases.Length)
			phaseIndex = 0;

		//Grab the current animation curve
		AnimationCurve curve = rayPhases[phaseIndex];
		//Tell the line renderer how many corners or vertices it will have
		rayRenderer.SetVertexCount(curve.keys.Length);
		//Loop through the vertices in our current animation curve and...
		for (int index = 0; index < curve.keys.Length; ++index)
		{
			//...grab the current keyframe, then...
			Keyframe key = curve.keys[index];

			//...calculate a new point based on our starting position, the vector of the bolt, and the position of the key (time
			//is the position of the keyframe along the animation curve timeline and it will have a value between 0 and 1)...
			Vector3 point = transform.position + vectorOfBolt * key.time;
			//...move our new point along the Y axis so that it is raised off of the ground...
			point += Vector3.up  *key.value * rayHeight;
			//...finally, set a vertices of the line renderer to our new point.
            rayRenderer.SetPosition(index, point);
		}
	}
}
