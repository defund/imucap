//This script handles the slime debuff. This debuff attaches to an enemy and slowly damages them while
//preventing the enemy from attacking

using UnityEngine;
using System.Collections;

public class SlimeDebuff : MonoBehaviour
{
	[HideInInspector] EnemyAttack targetAttack;		//Reference to the enemy's attack script
	[HideInInspector] EnemyHealth targetHealth;		//Reference to the enemy's health script

	[SerializeField] float effectDuration = 3f;		//How long the effect lasts
	[SerializeField] int attacksPerSecond = 4;		//How many times the attack damages the enemy per second
	[SerializeField] int damagePerAttack = 10;		//How much damage the attack does per attack

	WaitForSeconds attackDelay;						//How long to wait in between attacks

	void Awake()
	{
		//Calculate the delay between attacks by dividing 1 by the number of attacks in a second.
		//(example: 2 attacks per second = 1f / 2 = .5f delay between attacks
		attackDelay = new WaitForSeconds(1f / attacksPerSecond);
	}

	//Called by SlimeProjectile script
	public void AttachToEnemy(EnemyAttack enemy)
	{
		//Set the targetAttack variable to the provided enemy
		targetAttack = enemy;
		//Get a reference to the enemy's health
		targetHealth = targetAttack.GetComponent<EnemyHealth>();
		//Tell the enemy about this debuff
		targetAttack.SlimeDebuff = this;
		//Nest this debuff to the enemy (so it follows the enemy around)
		transform.parent = targetAttack.transform;
		//Center this debuff on the enemy, except move it slightly up
		transform.localPosition = new Vector3(0f, 1f, 0f);
		//Start attacking the enemy
		StartCoroutine("AttackEnemy");
	}

	//Releases the enemy after the appropriate amount of time
	public void ReleaseEnemy()
	{
		//Forget the target of the debuff and un-parent it from the enemy
		targetAttack = null;
		targetHealth = null;
		transform.parent = null;
		//Turn the debuff off
		gameObject.SetActive(false);
	}

	//This coroutine damages the enemy over time
	IEnumerator AttackEnemy()
	{
		//Calculate how many attacks should occur
		int totalAttacks = Mathf.RoundToInt(effectDuration * attacksPerSecond);
		//Loop until enough attacks have been completed
		for (int i = 0; i < totalAttacks; i++)
		{
			//Tell the enemy to take damage
			targetHealth.TakeDamage(damagePerAttack);
			//Wait until it is time to attack again
			yield return attackDelay;
		}

		//After the attacks have been complete, release the enemy
		ReleaseEnemy();
	}
}
