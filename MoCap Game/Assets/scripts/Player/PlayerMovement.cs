//This script handles moving the player. As the player doesn't move using a navmesh agent, some calculations have to be done to
//get the appropriate level of control.

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[HideInInspector] public Vector3 MoveDirection = Vector3.zero;		//The direction the player should move
	[HideInInspector] public Vector3 LookDirection = Vector3.forward;	//The direction the player should face

	[SerializeField] float speed = 6f;									//The speed that the player moves
	[SerializeField] Animator animator;									//Reference to the animator component
	[SerializeField] Rigidbody rigidBody;								//Reference to the rigidbody component

	bool canMove = true;												//Can the player move?

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab the needed component references
		animator = GetComponent <Animator> ();
		rigidBody = GetComponent <Rigidbody> ();
	}

	//Move with physics so the movement code goes in FixedUpdate()
	void FixedUpdate ()
	{
		//If the player cannot move, leave
		if (!canMove)
			return;

		//Remove any Y value from the desired move direction
		MoveDirection.Set (MoveDirection.x, 0, MoveDirection.z);
		//Move the player using the MovePosition() method of its rigidbody component. This moves the player is a more
		//physically accurate way than transform.Translate() does
		rigidBody.MovePosition (transform.position + MoveDirection.normalized * speed * Time.deltaTime);

		//Remove any Y value from the desired look direction
		LookDirection.Set (LookDirection.x, 0, LookDirection.z);
		//Rotate the player using the MoveRotation() method of its rigidbody component. This rotates the player is a more
		//physically accurate way than transform.Rotate() does. We also use the LookRotation() method of the Quaternion
		//class to help use convert our euler angles into a quaternion
		rigidBody.MoveRotation (Quaternion.LookRotation (LookDirection));
		//Set the IsWalking paramter of the animator. If the move direction has any magnitude (amount), then the player is walking
		animator.SetBool ("IsWalking", MoveDirection.sqrMagnitude > 0);
    }

	//Called when the player is defeated
	public void Defeated()
	{
		//Player can no longer move
		canMove = false;
	}
}

