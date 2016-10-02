//This script handles the frost attack. The frost attack is a continual attack which places a frost debuff on enemies
//in its cone of effect. The attack has no cooldown and uses a mesh collider in the shape of an arc to determine which 
//enemies are in range

using UnityEngine;

public class FrostAttack : MonoBehaviour
{
	[Header("Weapon Specs")]
	[SerializeField] int maxFreezableEnemies = 20;		//How many enemies can be freezed at once. This is helpful as it limits the number of frost debuffs that need to be tracked and thus reduces lag

	[Header("Weapon References")]
	[SerializeField] GameObject frostDebuffPrefab;		//Reference to the frost debuff prefab
	[SerializeField] GameObject frostCone;				//Reference to the game object with the arc mesh collider on it
	[SerializeField] MeshCollider frostArc;				//Reference to the arc mesh collider

	FrostDebuff[] debuffs;								//Array of frost debuffs that have been created
	bool isFiring = false;								//Is the attack currently firing?

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Grab the first child game object (which will be the frost cone since there is only one)
		frostCone = transform.GetChild (0).gameObject;
		//Get the mesh collider reference from the frostCone
		frostArc = GetComponentInChildren<MeshCollider> ();
	}

	void Awake()
	{
		//Create a new array of FrostDebuffs 
		debuffs = new FrostDebuff[maxFreezableEnemies];
		//Loop through the array...
		for(int i = 0; i < maxFreezableEnemies; i++)
		{
			//...create (instantiate) a new game object...
			GameObject obj = (GameObject)Instantiate (frostDebuffPrefab);
			//...deactivate it...
			obj.SetActive (false);
			//..and then save it into the array
			debuffs [i] = obj.GetComponent<FrostDebuff> ();
		}
	}

	//When we disable this game object, call the StopFiring() method
	void OnDisable()
	{
		StopFiring ();
	}

	//Called from PlayerAttack script
	public void Fire()
	{
		//If we aren't currently firing...
		if (!isFiring) 
		{
			//...turn on the frostCone game object and enable the mesh collider
			frostCone.SetActive (true);
			frostArc.enabled = true;

			//We are now firing.
			isFiring = true;
		}
	}

	//Called from PlayerAttack script and OnDisable()
	public void StopFiring()
	{
		//If we aren't currently firing, leave this method
		if (!isFiring)
			return;

		//Turn off the frostCone game object and disable the collider
		frostCone.SetActive (false);
		frostArc.enabled = false;

		//We are no longer firing
		isFiring = false;

		//Loop through our array of debuffs and...
		for (int i = 0; i < debuffs.Length; i++) 
		{
			//...if the debuff is currently active (which means it is attached to an enemy) tell
			//it to release its enemy
			if (debuffs [i].gameObject.activeInHierarchy)
				debuffs [i].ReleaseEnemy ();
		}
	}

	//Attaches a debuff to the specified enemy. This is called from OnTriggerEnter()
	void AttachDebuffToEnemy(EnemyMovement enemy)
	{
		//Loop through our array of debuffs, looking for an inactive one...
		for (int i = 0; i < debuffs.Length; i++) 
		{
			//...if this debuff isn't active...
			if (!debuffs [i].gameObject.activeInHierarchy) 
			{
				//...activate it and tell it to attach to the specified enemy...
				debuffs [i].gameObject.SetActive (true);
				debuffs [i].AttachToEnemy (enemy);
				//...then return so we don't try to attach more than one
				return;
			}
		}
	}

	//Called when an object enters the arc mesh collider
	void OnTriggerEnter(Collider other)
	{
		//Try to get a reference to an EnemyMovement script from the object that hit the collider
		EnemyMovement enemy = other.GetComponent<EnemyMovement> ();
		//If there is no EnemyMovement script, then this isn't an enemy and we should leave the method
		if (enemy == null)
			return;

		//If the enemy already has a frost debuff attached, tell that frost debuff
		//to reattach to the enemy (this happens when a frozen enemy leaves and reenters the arc mesh collider)
		if (enemy.FrostDebuff != null)
			enemy.FrostDebuff.AttachToEnemy (enemy);
		//If there isn't already a frost debuff attached, attach a new one
		else
			AttachDebuffToEnemy (enemy);
	}

	//Called when an object leaves the arc mesh collider
	void OnTriggerExit(Collider other)
	{
		//Try to get a reference to an EnemyMovement script from the object that hit the collider
		EnemyMovement enemy = other.GetComponent<EnemyMovement> ();
		//If there is no EnemyMovement script, then this isn't an enemy and we should leave the method
		if (enemy == null)
			return;

		//If the enemy already has a frost debuff attached, tell that frost debuff
		//to release the enemy
		if (enemy.FrostDebuff != null)
			enemy.FrostDebuff.ReleaseEnemy ();
	}
}
