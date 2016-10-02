//This script controls the spotlight game objects that assist with the character selection system.
//This script determines which spotlight (or spotlights) should be turned on and then turns them off
//when a player is selected

using UnityEngine;

public class CharacterSpotlight : MonoBehaviour 
{
	[SerializeField] GameObject spotlightPC;		//The PC (or other non-mobile) spotlight game object
	[SerializeField] GameObject spotlightMobile;	//The mobile spotlight game object

	void Awake()
	{
		//This is platform specific code. Any code that isn't in the appropriate section
		//is effectively turned into a comment (essentialy doesn't exist when the project is built).
		//If this is a mobile platform (Android, iOS, or WP8)... 
		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
			//...then activate the mobile spotlights
			spotlightMobile.SetActive(true);

		#else
			//Otherwise activate the PC spotlights
			spotlightPC.SetActive(true);

		#endif
	}

	void Update()
	{
		//Conitnuously look for the GameManager to have a player assigned. Once it does, we know the
		//player has been selected and we can disable this game object
		if (GameManager.Instance.Player != null) 
		{
			gameObject.SetActive (false);
		}
	}
}
