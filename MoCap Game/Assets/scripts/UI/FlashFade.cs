//This script causes an image to flash into view and then fade away. It does this by adjusting the alpha value of the image

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashFade : MonoBehaviour
{
	[SerializeField] Image image;										//The image to flash
	[SerializeField] Color flashColor = new Color(1f, 0f, 0f, 0.1f);	//The color to flash the image (default is red)
	[SerializeField] float flashSpeed = 5f;								//How fast to flash the image

	void Reset ()
	{
		//Grab the needed reference
		image = GetComponent<Image> ();
	}

	//Called by the PlayerHealth script when the player takes damage
	public void Flash ()
	{
		//If the image is currently flashing, stop it
		StopCoroutine("Fade");
		//Set the image to the desired color (which causes it to appear)
		image.color = flashColor;
		//Start the Fade() coroutine
		StartCoroutine("Fade");
	}

	//This coroutine fades an image out over time
	IEnumerator Fade()
	{
		//While the alpha of the image is greater than .01f (very small, we use this value instead of
		//0 because it can take a very long time for a Lerp method to reach true 0)...
		while (image.color.a > 0.01f)
		{
			//...Reduce the alpha using a Lerp (short for linear interpolation)...
			image.color = Color.Lerp(image.color, Color.clear, flashSpeed * Time.deltaTime);
			//...and then wait a frame before looping
			yield return null;
		}
		//Set the alpha of the image to 0 in case there is still a small alpha value
		image.color = Color.clear;
	}
}
