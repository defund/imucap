//This script handles the player's ability to attack. The biggest responcibility of this script is to maintain the timing of attack cooldowns
//so that the player cannot attack too fast. Mostly, this is a "pass through" or "bridge" script, which means that it receives input from
//the PlayerInput scripts and then passes the input along to the appropriate attack. Very little attack logic (apart from timing) exists in this 
//script. 

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[Header("Attacks")]
	[SerializeField] LightningAttack lightningAttack;	//Reference to a lightning attack script
	[SerializeField] FrostAttack frostAttack;			//Reference to a frost attack script
	[SerializeField] StinkAttack stinkAttack;			//Reference to a stink attack script
	[SerializeField] SlimeAttack slimeAttack;			//Reference to a slime attack script
	[SerializeField] int numberOfAttacks;				//The number of attacks the player has

	[Header("UI")]
	[SerializeField] Countdown countDown;				//A reference to the countdown slider

	int attackIndex = 0;								//The idnex of the attack the player is currently using
	float attackCooldown = 0f;							//How long the player must wait before attacking again
	float timeOfLastAttack = 0f;						//The time when the player last attacked
	bool canAttack = true;								//Whether or not the player can attack

	//This method switches the active attack on the player
	public void SwitchAttack()
	{
		//If the player can't attack, leave
		if (!canAttack)
			return;
		//Increase the attack index, then if the index is too high, set it back to 0
		attackIndex++;
		if (attackIndex >= numberOfAttacks)
			attackIndex = 0;

		//Turn off all enabled attacks
		DisableAttacks();
		//The switch statement is basically a streamlined if / else if statement. Here
		//the value of attackIndex is being examined and then the results are listed as
		//cases below
		switch (attackIndex)
		{
			//If the value of attackIndex is 0...
			case 0:
				//...and if lightningAttack exists, enable it
				if(lightningAttack != null)
					lightningAttack.gameObject.SetActive(true);
				break;
			//If the value of attackIndex is 1...
			case 1:
				//...and if frostAttack exists, enable it
				if(frostAttack != null)
					frostAttack.gameObject.SetActive(true);
				break;
			//If the value of attackIndex is 2...
			case 2:
				//...and if stinkAttack exists, enable it
				if(stinkAttack != null)
					stinkAttack.gameObject.SetActive(true);
				break;
			//If the value of attackIndex is 3...
			case 3:
				//...and if slimeAttack exists, enable it
				if(slimeAttack != null)
					slimeAttack.gameObject.SetActive(true);
				break;
		}
	}

	//This method is called whenever the player presses the attack input
	public void Fire()
	{
		//If the attack isn't ready, or the player cannot attack, leave
		if (!ReadyToAttack() || !canAttack)
			return;

		//Examing the value of attackIndex. Note that we are only handling lightning
		//and frost here. This is because the stink and slime attacks only fire
		//when we release the attack button. Therefore, they are fired in the 
		//StopFiring() method
		switch (attackIndex)
		{
			//If the value of attackIndex is 0, call ShootLightning()
			case 0:
				ShootLightning();
				break;
			//If the value of attackIndex is 1, call ToggleFrost()
			case 1:
				ToggleFrost(true);
				break;
		}
	}

	//This method is called whenever the player releases the attack input
	public void StopFiring()
	{
		//If the attack isn't ready, or the player cannot attack, leave
		if (!ReadyToAttack() || !canAttack)
			return;

		//Examing the value of attackIndex. 
		switch (attackIndex)
		{
			//If the value of attackIndex is 1, call ToggleFrost()
			case 1:
				ToggleFrost(false);
				break;
			//If the value of attackIndex is 2, call ShootStink()
			case 2:
				ShootStink();
				break;
			//If the value of attackIndex is 3, call ShootSlime()
			case 3:
				ShootSlime();
				break;
		}
	}

	//Handles telling the lightning attack to fire
	void ShootLightning()
	{
		//If there is no lightning attack, leave
		if (lightningAttack == null)
			return;

		//Fire lightning
		lightningAttack.Fire();
		//Record the cooldown of the lightning attack
		attackCooldown = lightningAttack.Cooldown;
		//record how long to wait before we can attack again
		BeginCountdown();
	}

	//Handles toggling frost on and off. Note that the frost attack has no cooldown
	void ToggleFrost(bool isAttacking)
	{
		//If there is no frost attack, leave
		if (frostAttack == null)
			return;
		//If true is passed into this method, shoot frost
		if (isAttacking) 
			frostAttack.Fire ();
		//Otherwise, stop shooting frost
		else
			frostAttack.StopFiring ();
	}

	//Handles telling the stink attack to fire
	void ShootStink()
	{
		//If there is no stink attack, leave
		if (stinkAttack == null)
			return;
		//Shoot a stink projectile
		stinkAttack.Fire();
		//record the cooldown of the stink attack
		attackCooldown = stinkAttack.Cooldown;
		//record how long to wait before we can attack again
		BeginCountdown();
	}

	//Handles telling the slime attack to fire
	void ShootSlime()
	{
		//If there is no slime attack, leave
		if (slimeAttack == null)
			return;
		//Attempt to fire slime. If it was successful...
		if (slimeAttack.Fire())
		{
			//...record the cooldown of the slime attack...
			attackCooldown = slimeAttack.Cooldown;
			//...and record how long to wait before we can attack again
			BeginCountdown();
		}
	}
		
	bool ReadyToAttack()
	{
		//If enough time has passed return true (the player can attack) otherwise return false
		return Time.time >= timeOfLastAttack + attackCooldown;
	}

	//Called from PlayerHealth script
	public void Defeated()
	{
		//Player cannot attack
		canAttack = false;
		//Turn off all attacks
		DisableAttacks();
	}

	//This method sets the countdown until the player can attack again
	void BeginCountdown()
	{
		//Record the current time
		timeOfLastAttack = Time.time;
		//If there is a countdown slider, tell it to begin counting down
		if (countDown != null)
			countDown.BeginCountdown(attackCooldown);
	}

	//This method turns off all attacks
	void DisableAttacks()
	{
		//Go through each attack and if a game object for it exists, turn it off
		if(lightningAttack != null)
			lightningAttack.gameObject.SetActive(false);

		if(frostAttack != null)
			frostAttack.gameObject.SetActive(false);

		if(stinkAttack != null)
			stinkAttack.gameObject.SetActive(false);

		if(slimeAttack != null)
			slimeAttack.gameObject.SetActive(false);
	}
}
