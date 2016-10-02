//This script controls the enemy's ability to attack. Attacking occurs when the player is within a trigger collider on the enemy (in range). Furthermore, the enemy
//doesn't attack whenever the player is in range, instead there is a time interval that the enemy tries to attack. Therefore, the player is only attacked
//if they are in range and the time interval occurs. This allows players to dodge in and out of range of enemies with slower attacks.

using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
	//We want this to be public, but not in the inspector, so we use [HideInInspector]
	[HideInInspector] public SlimeDebuff SlimeDebuff;		//Reference to a slime debuff that may prevent attacking

	[SerializeField] float timeBetweenAttacks = 0.5f;		//How much time goes by between each attack (the attack interval)
	[SerializeField] int attackDamage = 10;					//How much damage the enemy attacks for
	[SerializeField] Animator animator;						//Reference to the animator component

	bool canAttack;											//Can this enemy attack?
	bool playerInRange;										//Is the player in range?
	WaitForSeconds attackDelay;								//Variable to hold the attack delay

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Get a reference to the animator component
		animator = GetComponent<Animator>();
	}

	void Awake()
	{
		//Initialize our attackDelay variable here (it can be more efficient than doing it in the coroutine)
		attackDelay = new WaitForSeconds(timeBetweenAttacks);
	}

	//When this game object is enabled...
	void OnEnable()
	{
		//Remove any existing debuff and allow the enemy to attack (in 
		//case the enemy was defeated with a slime debuff still on it)
		SlimeDebuff = null;
		canAttack = true;
		//Start the AttackPlayer coroutine
		StartCoroutine("AttackPlayer");
	}

	//When the player enters the trigger collider this is called. In reality, it could be
	//any game object with a collider that enters the trigger. We can be confident that it
	//will be the player, however, since a rigidbody is needed for collision. The player is
	//the only object with a rigidbody, so it is likely the player that has entered the collider
	void OnTriggerEnter (Collider other)
	{
		//In case other rigidbodies are added to the project, we still want to make sure that the player
		//is the object that entered this collider. If the game object entering this collider
		//is the Player value of the GameManager (it's the player)...
		if(other.transform == GameManager.Instance.Player.transform)
		{
			//Record that the player is in range
			playerInRange = true;
		}
	}

	//When the player leaves the trigger collider this is called
	void OnTriggerExit (Collider other)
	{
		//If the game object leaving this collider
		//is the Player value of the GameManager (it's the player)...
		if(other.transform == GameManager.Instance.Player.transform)
		{
			//Record that the player is not in range
			playerInRange = false;
		}
	}

	//This coroutine checks to see if the enemy can attack the player at a set time interval
	IEnumerator AttackPlayer()
	{
		//Start by waiting a single frame to give the game a chance to initialize.
		//This is usefull if you start with an enemy in the scene (instead of spawning it)
		yield return null;

		//If there is no GameManager, leave this coroutine permanently (that's 
		//what 'yield break' does
		if (GameManager.Instance == null)
			yield break;

		//While the enemy can attack and the player isn't defeated...
		while (canAttack && CheckPlayerStatus())
		{
			//...and if the player is in range and the enemy isn't slimed...
			if (playerInRange && SlimeDebuff == null) 
			{
				//...Tell the player to take damage. Note that we route enemy damage
				//through to GameManager in case we want to do any modification or validation
				GameManager.Instance.Player.TakeDamage (attackDamage);
			}
			//Finally, wait an attack delay interval before looping again
			yield return attackDelay;
		}
	}

	//This method determines if the player can be attacked
	bool CheckPlayerStatus()
	{
		//If the player is alive and well, return true
		if (GameManager.Instance.Player.IsAlive())
			return true;

		//If the player isn't alive, the enemies have won, trigger the PlayerDead parameter
		animator.SetTrigger("PlayerDead");
		//Call the Defeated() method, even though the enemies won, we don't want them
		//running more attack code as it is unnecessary
		Defeated();
		//Return false because the player isn't attackable
		return false;
	}

	//Called when the enemy needs to stop attacked (either it was defeated or the player was)
	public void Defeated()
	{
		//Enemy can no longer attack
		canAttack = false;
		//If a slime debuff exists on the enemy, remove it
		if (SlimeDebuff != null)
			SlimeDebuff.ReleaseEnemy();
	}
}

