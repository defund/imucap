//This script handles the frost debuff. The debuff is applied to an enemy when the enemy enters the range of
//our frost attack. The debuff only applies a particle effect to the enemyfor the first period of time. 
//After the debuff has been on an enemy for a long enough period of time, it freezes the enemy in place
//and then stays on for another period of time, even if the enemy is no longer in range of the frost attack

using UnityEngine;

public class FrostDebuff : MonoBehaviour
{
	[SerializeField] GameObject mist;				//Reference to the mist game object
	[SerializeField] GameObject iceBlock;			//Reference to the iceBlock game object
	[SerializeField] float freezeDelay = 1f;		//How long it takes to freeze an enemy
	[SerializeField] float freezeDuration = 2f;		//How long the enemy stays frozen after leaving the range of the frost attack

	EnemyMovement target;							//The enemy target of this debuff
	float timeToToggleEffect;						//Contains the time, in seconds, where the debuff effects will be toggled
	bool isFreezing;								//Is this debuff currently freeszing an enemy solid?
	bool isAttached;								//Is this debuff currently attached to an enemy?

	//When we enable this game object...
	void OnEnable()
	{
		//...Enable the mist and iceBlock game objects. This will start playing their particle effects
		mist.SetActive (true);
		iceBlock.SetActive(false);

		//This debuff is not currently attached nor freezing an enemy
		isAttached = false;
		isFreezing = false;
	}

	void Update()
	{
		//Position the debuff in the same spot as our target
		transform.position = target.transform.position;

		//If this debuff isn't attached or freezing an enemy...
		if (!isAttached && !isFreezing) 
		{
			//...and if the target enemy knows about this debuff, tell the enemy to forget this debuff...
			if (target.FrostDebuff != null)
				target.FrostDebuff = null;

			//... forget the debuff's target and disable the debuff
			target = null;
			gameObject.SetActive (false);
		}
		//Otherwise, if the debuff is attached but not currently freezing call CheckForFreeze()
		else if (isAttached && !isFreezing)
		{
			CheckForFreeze ();
		}
		//Finally, if the debuff isn't attached but is freezing an enemy, call CheckForUnfreeze()
		else if (!isAttached && isFreezing)
		{
			CheckForUnFreeze ();
		}
	}

	//Called by Update()
	void CheckForFreeze()
	{
		//If this debuff has been attached to the enemy long enough...
		if (Time.time >= timeToToggleEffect) 
		{
			//...freeze it by calling FreezeTarget()
			FreezeTarget ();
		}
	}

	//Called by Update()
	void CheckForUnFreeze()
	{
		//If this debuff has been freezing the enemy long enough...
		if (Time.time >= timeToToggleEffect) 
		{
			//...unfreeze it by calling UnFreezeTarget()
			UnFreezeTarget ();
		}
	}

	//This method "attaches" the debuff to an enemy
	public void AttachToEnemy(EnemyMovement enemy)
	{
		//If the debuff currently already has a target, return
		if (target != null)
			return;

		//Set the target to the desired enemy
		target = enemy;
		//Tell the enemy about this frost debuff
		target.FrostDebuff = this;
		//This debuff is now attached
		isAttached = true;
		//Determine at what time the debuff will apply a freeze to the enemy
		timeToToggleEffect = Time.time + freezeDelay;
	}

	//This method "releases" the enemy from the debuff
	public void ReleaseEnemy()
	{
		//If the debuff doesn't have a target, return
		if (target == null)
			return;

		//The debuff is no longer attached
		isAttached = false;
		//If the debuff was freezing an enemy determine at what time it should release the enemy
		if (isFreezing)
			timeToToggleEffect = Time.time + freezeDuration;
	}

	//This method applies the freeze effect to an enemy
	void FreezeTarget()
	{
		//This debuff is now freezing an enemy
		isFreezing = true;
		//Tell the target enemy to be frozen (stop moving)
		target.Freeze();

		//Enable the mist and iceBlock game objects
		mist.SetActive (false);
		iceBlock.SetActive(true);
	}

	//This method removes the freeze effect from an enemy
	void UnFreezeTarget()
	{
		//No longer freezing
		isFreezing = false;
		//Tell the target to stop being frozen (resume moving)
		target.UnFreeze();
	}
}
