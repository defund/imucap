//This script is used to player a particle effect and audio effect (AV being short for Audio Visual). This is useful because it allows us to 
//play effects one time without needing to worry about turning game objects off at the right time. It is
//basically "fire and forget" without needing extra component references in other scripts.

using UnityEngine;

public class AVPlayer : MonoBehaviour
{
	[SerializeField] ParticleSystem particleEffect;	//Reference to a particle system component
	[SerializeField] AudioSource audioEffect;		//Reference to an audio source component

	//Reset() defines the default values for properties in the inspector
	void Reset()
	{
		//Grab references to the needed components
		particleEffect = GetComponent<ParticleSystem>();
		audioEffect = GetComponent<AudioSource>();
	}

	//This method plays the particle effect and audio source if they exist
	public void Play()
	{
		if (particleEffect != null)
			particleEffect.Play(true);	//Play() is called with 'true' which means that any particle systems nested under this one will also play

		if (audioEffect != null)
			audioEffect.Play();
	}
}
