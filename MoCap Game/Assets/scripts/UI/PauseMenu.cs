//This script handles pausing the game as well as controlling some game options such as audio and quitting

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

//If we are running this project in the editor, include UnityEditor
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
	[HideInInspector] public bool IsPaused = false;					//Is the game currently paused?

	[Header ("UI")]
	[SerializeField] GameObject pausePanel;							//A reference to the UI panel with all the controls on it
	[SerializeField] Slider effectsSlider = null;					//A slider for sound effects volume
	[SerializeField] Slider musicSlider = null;						//A slider for music volume

	[Header ("Audio")]
	[SerializeField] AudioMixer masterMixer = null;					//Reference to the main audio mixer
	[SerializeField] AudioMixerSnapshot mutedSnapshot = null;		//Reference to the muted snapshot
	[SerializeField] AudioMixerSnapshot unMutedSnapshot = null;		//Reference to the unmuted snapshot

	float originalTimeScale;										//The timescale of the game before pausing it

	void Awake()
	{
		//Record the original timescale
		originalTimeScale = Time.timeScale;

		//Declare a float variable named value
		float value;
		//If the mixer parameter sfxVol exists, save it's value in the variable named value...
		if (masterMixer.GetFloat("sfxVol", out value))
		{
			//...and set the value of the effects slider to it
			effectsSlider.value = value;
		}
		//If the mixer parameter musicVol exists, save it's value in the variable named value...
		if (masterMixer.GetFloat("musicVol", out value))
		{
			//...and set the value of the music slider to it
			musicSlider.value = value;
		}
	}

	//This method handles pausing and unpausing the game
	public void Pause ()
	{
		//Flip the value of IsPaused (true becomes false and vice versa)
		IsPaused = !IsPaused;

		//If the game is now paused...
		if (IsPaused)
		{
			//...activate the UI panel...
			pausePanel.SetActive(true);
			//...record the current timescale
			originalTimeScale = Time.timeScale;
			//...and set the timescale to 0 (which freezes time)
			Time.timeScale = 0f;
		}
		//Otherwise...
		else
		{
			//...hide the UI panel...
			pausePanel.SetActive(false);
			//...and set the timescale back to its original value
			Time.timeScale = originalTimeScale;
		}
	}

	//Called by the UI quit button
	public void Quit ()
	{
		//Application.Quit() doesn't work in the editor, so if we are in the editor...
#if UNITY_EDITOR
		//...quit the application be leaving play mode
		EditorApplication.isPlaying = false;
#else
		//Otherwise just quit the application
		Application.Quit();
#endif
	}

	//Called by the effects slider UI element
	public void SetEffectsLevel (float level)
	{
		//Set the effects volume of the mixer
		masterMixer.SetFloat ("sfxVol", level);
	}

	//Called by the music slider UI element
	public void SetMusicLevel (float level)
	{
		//Set the music volume of the mixer
		masterMixer.SetFloat ("musicVol", level);
	}

	//Called by the audio toggle UI element
	public void ToggleVolume(bool soundOn)
	{
		//If sound is being turned on, transition to the unmuted snapshot
		if (soundOn)
			unMutedSnapshot.TransitionTo(0f);
		//Otherwise, transition to the muted snapshot
		else
			mutedSnapshot.TransitionTo(0f);
	}
}
