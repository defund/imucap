//This script handles the game management. Game managers are often completely different and generally provide whatever
//specific and varied services an individual game may need. In this project, in an effort to make the code simple to understand
//and modular, the game manager is tied into several core functions of the player, enemies, and allies. Namely, this manager
//keeps track of the player and the players state, handles all scoring, interfaces with the UI, summons the allies, and 
//reloads the scene when the player is defeated

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;				//This script, like MouseLocation, has a public static reference to itself to that other scripts
													//can access it from anywhere without needing to find a reference to it

	[Header("Player and Enemy Properties")]
	public PlayerHealth Player;						//A reference to the player's health script which will be considered "the player"
	public Transform EnemyTarget;					//The object that enemies are chasing. This needs to be separate from the player because the game manager
													//can make enemies chase something that isn't the player (as is the case with allies)

	[SerializeField] float delayOnPlayerDeath = 1f;	//How long to wait once the player has been defeated

	[Header("UI Properties")]
	[SerializeField] Text infoText;					//Text UI element to show info to the player (info may be instructions or the player's score)
	[SerializeField] Text gameoverText;				//Text UI element informing the player that they have lost

	[Header("Player Selection Properties")]
	[SerializeField] GameObject enemySpawners;		//The game object parent of all of the enemy spawners
	[SerializeField] Animator cameraAnimator;		//A reference to the animator on the camera (to transition it when the player is chosen)

	[Header("Ally Properties")]
	[SerializeField] AllyManager allyManager;		//A reference to the attached ally manager script

	int score = 0;									//The player's current score

	void Awake()
	{
		//This is a common approach to handling a class with a reference to itself.
		//If instance variable doesn't exist, assign this object to it
		if (Instance == null)
			Instance = this;
		//Otherwise, if the instance variable does exist, but it isn't this object, destroy this object.
		//This is useful so that we cannot have more than one GameManager object in a scene at a time.
		else if (Instance != this)
			Destroy(this);
	}

	//Called by the PlayerSelect script when a player has been selected at the beginning of the game
	public void PlayerChosen(PlayerHealth selected)
	{
		//Record the player into the Player variable
		Player = selected;
		//Set the target of the enemies to the player's transform
		EnemyTarget = Player.transform;

		//If the info text UI element exists, tell it to say "Score: 0"
		if(infoText != null)
			infoText.text = "Score: 0";

		//If the enemy spawners game object exists, enable it
		if(enemySpawners != null)
			enemySpawners.SetActive (true);

		//If the reference to the camera's animator exists, trigger the Start parameter
		if(cameraAnimator != null)
			cameraAnimator.SetTrigger("Start");
	}

	//Called from the PlayerHealth script when the player has been defeated
	public void PlayerDied()
	{
		//The enemies no longer have a target
		EnemyTarget = null;

		//If the game over text UI element exists, turn it on
		if(gameoverText != null)
			gameoverText.enabled = true;
	}

	//Called from the PlayerHealth script when the player is done playing their death animation
	public void PlayerDeathComplete()
	{
		//Call the ReloadScene() method after the set delay
		Invoke("ReloadScene", delayOnPlayerDeath);
	}

	//Called from the EnemyHealth script when an enemy is defeated
	public void AddScore(int points)
	{
		//Add points to the player's score
		score += points;
		//If the info text UI element exists, update it to say the player's score
		if (infoText != null)
			infoText.text = "Score: " + score;
		//If the ally manager exists, give it some points for the player to spend on an ally
		if (allyManager != null)
			allyManager.AddPoints(points);
	}

	//Called from the PlayerInput scripts and / or the Ally button OnClick() event
	public void SummonAlly()
	{
		//If there is no ally manager, leave
		if (allyManager == null)
			return;
		//Attempt to get an Ally by calling SummonAlly()
		Ally ally = allyManager.SummonAlly();
		//If SummonAlly() was able to spawn an ally...
		if (ally != null)
		{
			//...set the ally as the target of the enemies...
			EnemyTarget = ally.transform;
			//...and call UnsummonAlly() after a set delay
			Invoke("UnSummonAlly", ally.Duration);
		}
	}

	//This method removes the ally from the scene
	void UnSummonAlly()
	{
		//Enemies go back to chasing the player
		EnemyTarget = Player.transform;
		//Tell the ally manager to remove the ally
		allyManager.UnSummonAlly();
	}

	//This method reloads the scene after the player has been defeated
	void ReloadScene()
	{
		//Get a reference to the current scene
		Scene currentScene = SceneManager.GetActiveScene();
		//Tell the SceneManager to load the current scene (which reloads it)
		SceneManager.LoadScene(currentScene.buildIndex);
	}
}
