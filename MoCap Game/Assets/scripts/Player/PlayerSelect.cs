//This script allows the player game object to be selected as the playable character of the game. This script is a large part of the 
//character selection system. The two roles of this script are to tell the GameManager if this game object is the selected player, and
//controlling the removal of this game object if it wasn't selected

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelect : MonoBehaviour 
{
	[Header("Player to Disable")]
	[SerializeField] PlayerSelect otherCharacter;		//The other player in the scene.

	[Header("References")]
	[SerializeField] PlayerHealth playerHealth;			//Reference to this player's health
	[SerializeField] Rigidbody rigidBody;				//Reference to this player's rigidbody component
	[SerializeField] CapsuleCollider capsuleCollider;	//Reference to this player's collider component
	[SerializeField] Animator animator;					//Reference to this player's animator

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Grab the needed component references
		playerHealth = GetComponent<PlayerHealth> ();
		rigidBody = GetComponent<Rigidbody> ();
		capsuleCollider = GetComponent<CapsuleCollider> ();
		animator = GetComponent<Animator> ();
	}

	//This is called when the mouse is clicked down and then released while the cursor is over this 
	//game object's collider
	void OnMouseUp()
	{
		//If this isn't a mobile platform...
		#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WP8
			//...and if the pointer is over a UI element, then leave this method without choosing a player
			//(this prevents the player from being chosen while the pause menu is open)
			if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
				return;
		#endif
		//Tell this GameManager that this is the chosen player
		GameManager.Instance.PlayerChosen (playerHealth);

		//If there is another player, tell it that it wasn't selected
		if(otherCharacter != null)
			otherCharacter.DisableSelectableCharacter ();

		//Disable this script (it isn't needed anymore)
		enabled = false;
	}

	//Called from a different player when it was selected instead of this player
	public void DisableSelectableCharacter()
	{
		//Turn off the capsule collider so this player can't be collided with 
		capsuleCollider.enabled = false;
		//Player the death animation
		animator.SetTrigger ("Die");
	}

	//This method is called by an event in the Death animation on the player
	void DeathComplete ()
	{
		//Remove any drag on the rigidbody so this player can sink into the ground
		rigidBody.drag = 0f;
		//Destroy this game object after 1 second
		Destroy (gameObject, 1f);
	}
}
