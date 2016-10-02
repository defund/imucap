//This script controls the health functions of the enemies. This script also is responsible for turning
//the enemy movement and attack off in the event of the enemy being defeated. Since the enemies aren't destroyed after
//being defeated (they are just disabled since the game maintains 'pools' or collections of enemies) there is
//code in place to reset the values of the enemies when they respawn

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[HideInInspector] public EnemySpawner Spawner;		//A Reference to the spawner that created this enemy

	[Header("Health Properties")]
	[SerializeField] int maxHealth = 100;				//How much health this enemy has
	[SerializeField] int scoreValue = 10; 				//How many points this enemy is worth

	[Header("Defeated Effects")]
	[SerializeField] float sinkSpeed = 2.5f;			//How fast the enemy sinks into the ground		
	[SerializeField] float deathEffectTime = 2f;		//How long it takes the enemy to play its full death sequence before being deactivated
	[SerializeField] AudioClip deathClip = null;		//Audio clip of the death sound of the enemy
	[SerializeField] AudioClip hurtClip = null;			//Audio clip of the hurt sound of the enemy

	[Header("Script References")]
	[SerializeField] EnemyAttack enemyAttack;			//Reference to the enemy's attack script
	[SerializeField] EnemyMovement enemyMovement;		//Reference to the enemy's movement script

	[Header("Components")]
	[SerializeField] Animator animator;					//Reference to the animator component
	[SerializeField] AudioSource audioSource;			//Reference to the audio source component
	[SerializeField] CapsuleCollider capsuleCollider;	//Reference to the capsule collider component
	[SerializeField] ParticleSystem hitParticles;		//Reference to the particle system on the hit particles game object

	[Header("Debugging Properties")]
	[SerializeField] bool isInvulnerable;				//Is the enemy immune to all damage?

	int currentHealth;									//Current health amount of enemy
	bool isSinking;										//Is the enemy currently sinking?

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab references to all of the needed enemy components
		enemyAttack = GetComponent<EnemyAttack>();
		enemyMovement = GetComponent<EnemyMovement>();

		animator = GetComponent <Animator> ();
		audioSource = GetComponent <AudioSource> ();
		capsuleCollider = GetComponent <CapsuleCollider> ();
		//Get the hit particles component from a child object
		hitParticles = GetComponentInChildren <ParticleSystem> ();
	}

	//When this game object is enabled...
	void OnEnable ()
	{
		//...reset the health, isSinking, and make the capsule collider solid again (it is 
		//turned into a trigger so the enemy can sink through the ground)
		currentHealth = maxHealth;
		isSinking = false;
		capsuleCollider.isTrigger = false;

		//If there is an audio source, set the clip to the hurt sound
		if(audioSource != null)
			audioSource.clip = hurtClip;
	}

	void Update()
	{
		//If the enemy isn't currently sinking, return
		if(!isSinking)
			return;
		//If the enemy is sinking, move downward along the -Y axis
		transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
	}

	//This method is called whenever the enemy is damaged by an attack
	public void TakeDamage(int amount)
	{
		//If the enemy is already defeated or is invulnerable, leave
		if (currentHealth <= 0 || isInvulnerable)
			return;

		//Reduce the current health by the amount of damage
		currentHealth -= amount;

		//If the current health is now at or below 0, the enemy is defeated
		if (currentHealth <= 0)
			Defeated();

		//If there is an audio source, play it
		if(audioSource != null)
			audioSource.Play();
		//Play the hit particle effect
		hitParticles.Play();
	}

	//Called when the enemy health is reduce to 0 or lower
	void Defeated()
	{
		//Capsule collider becomes a trigger to that the enemy can sink into the ground and so that
		//this collider won't interfere with player attacks
		capsuleCollider.isTrigger = true;

		//Enabled the animator (in case it was disabled by a frost debuff)
		animator.enabled = true;
		//Trigger the "Dead" parameter of the animator
		animator.SetTrigger("Dead");

		//If there is an audio source, set its clip to the death sound
		if(audioSource != null)
			audioSource.clip = deathClip;

		//Tell the attack and movement script that the enemy has been defeated
		enemyAttack.Defeated();
		enemyMovement.Defeated();

		//Tell the game manager to give the player some points
		GameManager.Instance.AddScore(scoreValue);
		//Call the TurnOff() method after a period of time
		Invoke("TurnOff", deathEffectTime);
	}

	//Called once the enemy's "defeated" effects have finished playing
	void TurnOff()
	{
		//Disable the game object
		gameObject.SetActive(false);
	}

	// Accessed from an event in the enemy's Death animation
	public void StartSinking()
	{
		//The enemy is now sinking
		isSinking = true;
	}
}

