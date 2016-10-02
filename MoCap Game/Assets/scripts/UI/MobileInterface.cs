//This script simply turns the mobile interface off if it is not built for a mobile platform. Additionally, this script listens for
//the "Cancel" input axis (the back button on Android phones) and exits the game if it is pressed

using UnityEngine;

public class MobileInterface : MonoBehaviour
{
	//If this is not a mobile platform, disable this game object when it starts up
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WP8
	void Awake()
	{
		gameObject.SetActive(false);
	}
#endif

	void Update()
	{
		//If the cancel input axis is pressed (back button on Android), quit the game
		if(Input.GetButtonDown("Cancel"))
			Application.Quit();
	}
}
