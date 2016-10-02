//This script handled player input on mobile platforms (platforms with a touch interface). This script will 
//disable itself if the porject is not built for mobile

using UnityEngine;

public class PlayerInputTouch : MonoBehaviour
{
	[SerializeField] PlayerMovement playerMovement = null;       //Reference to the player's movement script
	[SerializeField] PlayerAttack playerAttack = null;           //Reference to the player's attack script
	[SerializeField] PauseMenu pauseMenu;						//Reference to the pause menu
	[SerializeField] Touchpad touchPad;							//Reference to the touchpad UI script

	bool isFiring;												//Is the player currently firing

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Grab the needed component references
		playerMovement = GetComponent<PlayerMovement>();
		playerAttack = GetComponent<PlayerAttack>();
		//Find an instance of the PauseMenu and Touchpad scripts in the scene
		pauseMenu = FindObjectOfType<PauseMenu>();
		touchPad = FindObjectOfType<Touchpad>();
	}

	//If this is not a mobile platform, lines 28 through 31 will be enabled and this script will remove itself from the player
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WP8
	void Awake()
	{
		Destroy(this);
	}
#endif

	void Update()
	{
		//If the player cannot update, leave
		if (!CanUpdate())
			return;
		//Handle inputs for movement and attacking
		HandleMoveInput();
		HandleAttackInput();
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

		//If there is a touchpad...
		if (touchPad != null)
		{
			//...get the direction of any movement touch that exists...
			Vector2 movement = touchPad.GetDirection();
			//...tell the movement script to move on the X and Z axes with no Y axis movement
			playerMovement.MoveDirection = new Vector3(movement.x, 0, movement.y);
		}
		//If there is a MouseLocation script and the mouse's position is valid...
		if (MouseLocation.Instance != null && MouseLocation.Instance.IsValid)
		{
			//Find the point the player should look at by subtracting the player's position from the mouse's position
			Vector3 lookPoint = MouseLocation.Instance.MousePosition - playerMovement.transform.position;
			//Tell the player what direction to look
			playerMovement.LookDirection = lookPoint;
		}
	}

	void HandleAttackInput()
	{
		//If there is no attack script or not touchpad, leave
		if (playerAttack == null || touchPad == null)
			return;

		//If the player has pressed the button for switching weapons, tell the attack script
		if(touchPad.GetSwitchWeapons())
			playerAttack.SwitchAttack();

		//If the player is attacking (by touching the screen while moving)...
		if (touchPad.GetFiring())
		{
			//...the player is firing...
			isFiring = true;
			//...let the attack script know
			playerAttack.Fire();
		}
		//Otherwise, if the player is currently firing
		else if (isFiring)
		{
			//The player is no longer firing
			isFiring = false;
			//Tell the attack script to stop firing
			playerAttack.StopFiring();
		}
	}
}

