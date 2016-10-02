//This script uses an image UI element as a full screen touchpad. The image that this is attached to is treated as being a hardware touchpad (like
//one you might have on a laptop computer). Furthermore, this touchpad is self adapting for when players switch hands for control or
//use different portions of the screen. The way that input is treated is this: The first touch is considered to be the movement touch. Therefore and the 
//first finger to touch the screen is moved, the player moves. The second touch controls where the player looks. Attacks like lightning and frost will
//fire the entire time the second touch is on the screen, while the stink and slime will fire once the second touch is removed. 

using UnityEngine;
using UnityEngine.EventSystems;

//This script implements three interfaces (IPointerDownHandler, IDragHandler, IPointerUpHandler). This basically means that if this script has specific methods 
//(OnPointerDown, OnDrag, and OnPointerUp), they will be called automatically by Unity
public class Touchpad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField] float smoothing = 5f;	//Amount to smooth touch imputs
	
	Vector2 moveOrigin;						//The original touch position for movement
	Vector2 moveDirection;					//The direction the movement touch has moved on the screen
	Vector2 smoothDirection;				//Amount of smoothing applied to movement (so that it behaves like a joystick)
	bool switchWeapons;						//Is the player supposed to switch attacks?
	int aimTouchID;							//Finger ID that is handling aiming
	int moveTouchID;						//Finger ID that is handling movement

	const int nullValue = -999;				//A default "junk" value. This is needed to make the touchpad work with the Unity Remote app
	
	void Awake ()
	{
		//Set initial values
		moveDirection = Vector2.zero;
		moveTouchID = nullValue;
		aimTouchID = nullValue;
	}

	//Called by Unity whenever something touches the UI element this script is on
	public void OnPointerDown (PointerEventData data)
	{
		//If we don't have a movement touch currently...
		if (moveTouchID == nullValue)
		{
			//...record this touch as the movement touch
			moveTouchID = data.pointerId;
			//...record the position of this touch
			moveOrigin = data.position;
		}
		//Otherwise, if we don't currently have an aim touch...
		else if (aimTouchID == nullValue)
		{
			//...record this touch as the aim touch
			aimTouchID = data.pointerId;
			//...and if there is a MouseLocation script in the scene, tell 
			//it to start aiming the mouse with this touch
			if(MouseLocation.Instance != null)
				MouseLocation.Instance.StartTouchAim(data.position);
		}
	}

	//Called by Unity whenever a touch is dragged across the UI element this script is on
	public void OnDrag (PointerEventData data)
	{
		//If the dragging touch is the movement touch...
		if (data.pointerId == moveTouchID)
		{
			//...record the dragging touch's current position
			Vector2 currentPosition = data.position;
			//...calculate the direction raw it moved by subtracting the original position 
			//from its current position
			Vector2 directionRaw = currentPosition - moveOrigin;
			//...finally save the move direction as the normalize raw direction (normalizing
			//something reduces it to values between 0 and 1. So, for example a Vector3
			//with values of (4, 5, 10) would be normalized to (.25, .5, 1)
			moveDirection = directionRaw.normalized;
		}
		//Otherwise, if the dragging touch is the aim touch...
		else if (data.pointerId == aimTouchID)
		{
			//...and if there is a MouseLocation script, update its mouse position
			if(MouseLocation.Instance != null)
				MouseLocation.Instance.UpdatePosition(data.position);
		}
	}

	//Called by Unity whenever a touch is removed from the UI element this script is on
	public void OnPointerUp (PointerEventData data)
	{
		//If the removed touch is the movement touch...
		if (data.pointerId == moveTouchID)
		{
			//...reset the movement direction to 0
			moveDirection = Vector3.zero;
			//...reset the movement touch ID to the null value
			moveTouchID = nullValue;
		}
		//Otherwise, if the removed touch is the aiming touch...
		else if (data.pointerId == aimTouchID)
		{
			//...reset the ID of the aim touch to the null value
			aimTouchID = nullValue;
			//...and if there is a MouseLocation, tell it to stop using the touch position
			if(MouseLocation.Instance != null)
				MouseLocation.Instance.StopTouchAim();
		}
	}

	//Called by the player to get the direction it should move
	public Vector2 GetDirection ()
	{
		//Clamp the value of smoothDirection so that the player won't move really fast if the 
		//touch is dragged all the way across the screen (behaves like a mechanical thumbstick)
		smoothDirection = Vector2.MoveTowards (smoothDirection, moveDirection, smoothing);
		//Return the direction the player should move
		return smoothDirection;
	}
		
	public bool GetFiring()
	{
		//If there is currently an aiming touch, return true
		if (aimTouchID == nullValue)
			return false;

		//If not, return false
		return true;
	}

	//This method is called by the weapon switching UI button
	public void SwitchWeapons()
	{
		//The player should switch weapons
		switchWeapons = true;
	}

	//This is called by the PlayerInputMobile script
	public bool GetSwitchWeapons()
	{
		//If the player should switch weapons...
		if (switchWeapons)
		{
			//set the switchWeapons variable to false so we don't switch weapons multiple times
			switchWeapons = false;
			//Tell the player that they should switch weapons
			return true;
		}
		//Tell the player that they should not switch weapons
		return false;
	}
}