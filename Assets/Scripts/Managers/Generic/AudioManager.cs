using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

// Make static class? - Decide after researching audio mixer snapshots
public class AudioManager : SingletonBehaviour<AudioManager>
{
	// Audio Mixer
	public AudioMixer mixer;

	// Audio Source Dictionary
	Dictionary<string, AudioSource> dAudioSources;

	// Can Create New Audio Source flag
	bool bCanCreateSources = true;

	// Initialization
	public override void Awake()
	{
		base.Awake();

		dAudioSources = new Dictionary<string, AudioSource>();
	}

	#region Sound Management

	// Creates an object with an audio source and adds it to the dictionary
	private void CreateAudioSource(string audioName, AudioMixerGroup mixerGroup)
	{
		if (dAudioSources.ContainsKey(audioName))
		{
			return;
		}

		GameObject audioObj = new GameObject("Audio - " + audioName);
		audioObj.transform.parent = transform;
		AudioSource source = audioObj.AddComponent<AudioSource>();
		source.playOnAwake = false;
		source.outputAudioMixerGroup = mixerGroup;

		dAudioSources.Add(audioName, source);
	}

	// Destroys an audio source and removes it from the dictionary
	public void DestroyAudioSource(string audioName)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			return;
		}

		Destroy(dAudioSources[audioName].gameObject);
		dAudioSources.Remove(audioName);
	}

	// Destroys all audio sources
	public void DestroyAllAudioSources(bool blockSourceCreation = true)
	{
		if (dAudioSources.Count <= 0)
		{
			return;
		}

		foreach (AudioSource source in dAudioSources.Values)
		{
			Destroy(source.gameObject);
		}

		dAudioSources.Clear();

		if (blockSourceCreation)
		{
			bCanCreateSources = false;
			StartCoroutine(ReenableSourceCreation());
		}
	}

	// Waits a set amount of time before reenabling source creation to stop source being recreated unwantingly
	private IEnumerator ReenableSourceCreation()
	{
		yield return new WaitForSeconds(2f);

		bCanCreateSources = true;
	}

	// Get whether an audio source is playing
	public bool BGetIsPlaying(string audioName)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			return false;
		}

		return dAudioSources[audioName].isPlaying;
	}

	// Plays a sound with the appropriate audio source
	public void PlaySound(string audioName, AudioClip clip, AudioMixerGroup mixerGroup,
		float volume = 1, float pitch = 1, bool loop = false)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			if (bCanCreateSources)
			{
				CreateAudioSource(audioName, mixerGroup);
			}
			else
			{
				return;
			}
		}

		AudioSource source = dAudioSources[audioName];
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;

		source.clip = clip;
		source.Play();
	}

	// Pauses any sound on the appropriate audio source
	public void PauseSound(string audioName)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			Debug.LogError("Cannot pause sound " + audioName + " as the source does not exist");
			return;
		}

		dAudioSources[audioName].Pause();
	}

	// Unpauses any sound on the appropriate audio source
	public void UnpauseSound(string audioName)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			Debug.LogError("Cannot unpause sound " + audioName + " as the source does not exist");
			return;
		}

		dAudioSources[audioName].UnPause();
	}

	// Stops any sound on the appropriate audio source
	public void StopSound(string audioName)
	{
		if (!dAudioSources.ContainsKey(audioName))
		{
			Debug.LogError("Cannot stop sound " + audioName + " as the source does not exist");
			return;
		}

		dAudioSources[audioName].Stop();
	}

	// Pauses all sources
	public void PauseAllSounds()
	{
		if (dAudioSources.Count <= 0)
		{
			return;
		}

		foreach (AudioSource source in dAudioSources.Values)
		{
			source.Pause();
		}
	}

	// Unpauses all sources
	public void UnpauseAllSounds()
	{
		if (dAudioSources.Count <= 0)
		{
			return;
		}

		foreach (AudioSource source in dAudioSources.Values)
		{
			source.UnPause();
		}
	}

	// Stops all sources
	public void StopAllSounds()
	{
		if (dAudioSources.Count <= 0)
		{
			return;
		}

		foreach (AudioSource source in dAudioSources.Values)
		{
			source.Stop();
		}
	}

	#endregion

	#region Volume

	// Set Volume for a Mixer Group
	public void SetVolume(string groupName, float volume)
	{
		volume = Mathf.Clamp01(volume);
		volume = (1f - volume) * -80f;

		mixer.SetFloat(groupName, volume);
	}

	// Set Mixer Reverb
	public void SetReverb(float volume)
	{
		volume = Mathf.Clamp01(volume);
		volume = (1f - volume) * -80f;

		mixer.SetFloat("ReverbVolume", volume);
	}

	// Set Mute
	public void SetMute(bool mute)
	{
		float volume;

		if (mute)
		{
			volume = 0;
		}
		else
		{
			volume = GlobalSettings.fMasterVolume;
		}

		SetVolume("MasterVolume", volume);
	}

	#endregion

	// TO-DO ----- Research Audio Mixer management and implement when basic snapshots are made
	public void SetSnapshot()
	{
	}
}
