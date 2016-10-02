//This script handled player input on standalone platforms (platforms with a keyboard and mouse). This script will 
//disable itself if the porject is built for mobile

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputPC : MonoBehaviour
{
	[SerializeField] PlayerMovement playerMovement = null;	//Reference to the player's movement script
	[SerializeField] PlayerAttack playerAttack = null;		//Reference to the player's attack script
	[SerializeField] PauseMenu pauseMenu;					//Reference to the pause menu

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab the needed component references
		playerMovement = GetComponent<PlayerMovement> ();
		playerAttack = GetComponent<PlayerAttack> ();
		//Find an instance of the PauseMenu script in the scene
		pauseMenu = FindObjectOfType<PauseMenu>();
	}

	//If this is a mobile platform, lines 25 through 28 will be enabled and this script will remove itself from the player
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
	void Awake()
	{
		Destroy(this);
	}
#endif

	void Update ()
	{
		//If there is a pause menu and the player presses the Cancel input axis, pause the game
		if (pauseMenu != null && Input.GetButtonDown("Cancel"))
			pauseMenu.Pause();
		//If the player cannot update, leave
		if (!CanUpdate())
			return;
		//Handle inputs for movement, attacking, and allies
		HandleMoveInput();
		HandleAttackInput();
		HandleAllyInput();
	}

	bool CanUpdate()
	{
		//If the game is paused, the player cannot update
		if (pauseMenu != null && pauseMenu.IsPaused)
			return false;
		//If this player isn't the player assigned to the GameManager, then this player cannot update
		if (GameManager.Instance.Player == null || GameManager.Instance.Player.transform != transform)
			return false;
		//If the above two statements aren't true, then the player can update
		return true;
	}

	void HandleMoveInput()
	{
		//If there is no movement script, leave
		if (playerMovement == null)
			return;
		//Get the raw Horizontal and Vertical inputs (raw inputs have no smoothing applied)
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		//Tell the movement script to move on the X and Z axes with no Y axis movement
		playerMovement.MoveDirection = new Vector3(horizontal, 0, vertical);
		//If there is a MouseLocation script and the mouse's position is valid...
		if (MouseLocation.Instance != null && MouseLocation.Instance.IsValid) {
			//Find the point the player should look at by subtracting the player's position from the mouse's position
			Vector3 lookPoint = MouseLocation.Instance.MousePosition - playerMovement.transform.position;
			//Tell the player what direction to look
			playerMovement.LookDirection = lookPoint;
		}
	}

	void HandleAttackInput()
	{
		//If there is no attack script, leave
		if (playerAttack == null)
			return;

		//If the player presses the SwitchAttack input axis, tell the attack script to switch weapons
		if (Input.GetButtonDown("SwitchAttack"))
		{
			playerAttack.SwitchAttack();
		}
		//If the player presses (or holds) Fire1, start firing
		if (Input.GetButton("Fire1"))
		{
			playerAttack.Fire();
		}
		//Otherwise, stop firing
		else if(Input.GetButtonUp("Fire1"))
		{
			playerAttack.StopFiring ();		
		}
	}

	void HandleAllyInput()
	{
		//If the player presses the SummonAlly input axis and there is a GameManager, tell the GameManager to summon an ally
		if (Input.GetButtonDown("SummonAlly") && GameManager.Instance != null)
			GameManager.Instance.SummonAlly();

	}
}

