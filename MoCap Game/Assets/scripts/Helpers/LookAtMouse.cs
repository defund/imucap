//This script is used by the character selection PC spotlight. All it does is use the MouseLocation
//script to find the mouse cursor and then "look at" it (LookAt() is a method the points an object's
//positive Z axis at some point)

using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
	void Update ()
	{
		//If the a MouseLocation script is in the scene and its mouse location is valid...
		if (MouseLocation.Instance != null && MouseLocation.Instance.IsValid)
		{
			//Point the Z axis of this game object at it
			transform.LookAt (MouseLocation.Instance.MousePosition);
		}
	}
}

