//This script allows a camera to follow the player smoothly and without rotation

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] float smoothing = 5f;							//Amount of smoothing to apply to the cameras movement
	[SerializeField] Vector3 offset = new Vector3 (0f, 15f, -22f);	//The offset of the camera from the player (how far back and above the player the camera should be)

	//FixedUpdate is used to handle physics based code. No physics code exists in this FixedUpdate, but since the player's movement code
	//is handled in FixedUpdate, we are moving the camera in FixedUpdate as well so that they stay in sync
	void FixedUpdate ()
	{
		//Use the player's position and offset to determine where the camera should be
		Vector3 targetCamPos = GameManager.Instance.Player.transform.position + offset;
		//Smoothly move from the current position to the desired position using a Lerp, which is short
		//for linear interpolation. Basically, it takes where you are, where you want to be, and an amount of time
		//and then tells you where you will be along that line
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}

