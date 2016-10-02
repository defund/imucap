//This script disabled an animator component when it starts. This is needed to allow an animation
//to turn off its animator (and animation cannot normally turn off its animator, but it can turn
//this script on, which will then turn the animator off). This is used for the camera so that it can 
//follow the player after its character selection animation

using UnityEngine;

public class AnimatorDisabler : MonoBehaviour
{
	[SerializeField] Animator animator;		//Reference to an animator component

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab the local animator component
		animator = GetComponent<Animator> ();
	}

	void Start ()
	{
		//Turn off the animator
		animator.enabled = false;
	}
}

