//This script tracks the location of the mouse in 3D space. It does this by taking the location
//of the mouse on the screen (2D space). It then draws a line from the camera, through the mouse's 
//position on the screen into the world. Finally, it determines where this line hits a collider in
//the 3D scene. 

//This line of code is special and its purpose is to disable warning 0414 in this script. That warning
//writes to the console and tells you that this script has a variable that is created but never used. The IDE
//thinks we don't use the variable isTouchAiming because it is wrapped in platform specific code. Therefore
//when we are on PC we don't use that variable, but when we are mobile we do. Instead of having this warning
//constantly in the console window, this line simply turns that warning off (for this script only)
#pragma warning disable 0414
using UnityEngine;

public class MouseLocation : MonoBehaviour
{
	public static MouseLocation Instance;			//A reference to a MouseLacation object. This allows the class to have a public reference to itself to other scripts can 
													//access it without having a reference to it. 

	[HideInInspector] public Vector3 MousePosition;	//Location in 3D space of the mouse cursor	
	[HideInInspector] public bool IsValid;			//Is the mouse location valid?

	[SerializeField] LayerMask whatIsGround;		//A LayerMask indicating what is considered to be ground when determining the mouse's location

	Ray mouseRay;									//A ray that will be used to find the mouse
	RaycastHit hit;									//A RaycastHit which will store information about a raycast
	Vector2 screenPosition;							//Where the mouse is on the screen
	bool isTouchAiming;								//Are we using touch to aim? This will be used if we are on a mobile device

	void Awake()
	{
		//This is a common approach to handling a class with a reference to itself.
		//If instance variable doesn't exist, assign this object to it
		if (Instance == null)
			Instance = this;
		//Otherwise, if the instance variable does exist, but it isn't this object, destroy this object.
		//This is useful so that we cannot have more than one MouseLocation object in a scene at a time.
		else if (Instance != this)
			Destroy(this);
	}

	void Update()
	{
		//Assume the mouse location isn't valid
		IsValid = false;

		//This is platform specific code. Any code that isn't in the appropriate section
		//is effectively turned into a comment (essentialy doesn't exist when the project is built).
		//If this is a mobile platform (Android, iOS, or WP8)... 
		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
				//...and if it isn't using touch aiming, leave
				if (!isTouchAiming)
					return;
		#else
				//...otherwise, record the mouse's position to the screenPosition variable
				screenPosition = Input.mousePosition;
		#endif

		//Create a ray that extends from the main camera, through the mouse's position on the screen
		//into the scene
		mouseRay = Camera.main.ScreenPointToRay(screenPosition);

		//If the ray from our camera hits something that is ground...
		if (Physics.Raycast(mouseRay, out hit, 100f, whatIsGround))
		{
			//...the mouse position is valid...
			IsValid = true;
			//...and record the point in 3D space that the ray hit the ground
			MousePosition = hit.point;
		}
	}

	//This method starts touch aiming (will be called from the Touchpad script)
	public void StartTouchAim(Vector2 position)
	{
		//We are now touch aiming
		isTouchAiming = true;
		//Record the position of the screen touch
		screenPosition = position;
	}

	//This method tells the MouseLocation script where the player is touching (will be called from the Touchpad script)
	public void UpdatePosition(Vector2 position)
	{
		screenPosition = position;
	}

	//This method stops touch aiming (will be called from the Touchpad script)
	public void StopTouchAim()
	{
		//We are no longer touch aiming
		isTouchAiming = false;
	}
}
