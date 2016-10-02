//This script controls the slider that acts as a visible countdown timer for the player's attacks

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
	[SerializeField] Slider slider;									//Reference to the slider component
		
	float timeOfCountdownFinish;									//The time when the countdown will be complete
	static WaitForSeconds updateDelay = new WaitForSeconds(.25f);	//The delay between updating the slider (for efficiency). 

	void Reset ()
	{
		//Grab a reference to the slider component
		slider = GetComponent<Slider> ();
	}

	void Awake()
	{
		//Start off by reducing the slider values
		slider.minValue = 0f;
		slider.maxValue = 0f;
	}

	//Called by PlayerAttack script when an attack with a cooldown has been made
	public void BeginCountdown(float cooldown)
	{
		//Record the time the cooldown will be complete
		timeOfCountdownFinish = Time.time + cooldown;

		//Set the max value and current value of the slider to the total cooldown amount
		slider.maxValue = cooldown;
		slider.value = cooldown;
		//Start the UpdateCountdownBar() coroutine
		StartCoroutine("UpdateCountdownBar");
	}

	//This coroutine reduces the value of the slider so the player can see when their attack will be ready
	IEnumerator UpdateCountdownBar()
	{
		//While the value of the slider is greater than 0...
		while (slider.value > 0f)
		{
			//...reduce the value of the slider...
			slider.value = timeOfCountdownFinish - Time.time;
			//...wait a period of time before looping
			yield return updateDelay;
		}
		//Once the countdown is finished, set the max value to 0 so the slider dissapears
		slider.maxValue = 0f;
	}
}

