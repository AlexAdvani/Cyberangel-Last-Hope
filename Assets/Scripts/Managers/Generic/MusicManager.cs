using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : SingletonBehaviour<MusicManager>
{
	// Audio Mixer
	public AudioMixer mixer;
	// Audio Mixer Group
	public AudioMixerGroup mixerGroup;
	// Music Source
	AudioSource musicSource;

	// Repeat Track flag
	bool bRepeatTrack = true;

	#region Public Properties

	// Is Playing flag
	public bool IsPlaying
	{
		get
		{
			if (musicSource != null)
			{
				return musicSource.isPlaying;
			}
			else
			{
				return false;
			}
		}
	}

	#endregion

	// Initialization
	public override void Awake()
	{
		base.Awake();
		CreateMusicSource();
	}

	// On Destroy
	void OnDestroy()
	{
		DestroyMusicSource();
	}

	#region Source Management

	// Creates a new Music Source
	private void CreateMusicSource()
	{
		if (musicSource != null)
		{
			return;
		}

		GameObject musicObj = new GameObject("Music");
		musicObj.transform.parent = transform;
		AudioSource source = musicObj.AddComponent<AudioSource>();
		source.playOnAwake = false;
		source.outputAudioMixerGroup = mixerGroup;

		musicSource = source;
	}

	// Destroys the music source
	private void DestroyMusicSource()
	{
		Destroy(musicSource.gameObject);
		musicSource = null;
	}
		
	#endregion

	#region Music Management

	// Play Music 
	public void PlayMusic(AudioClip music, float volume = 1)
	{
		if (musicSource == null)
		{
			CreateMusicSource();
		}

		musicSource.volume = volume;
		musicSource.loop = bRepeatTrack;

		musicSource.clip = music;
		musicSource.Play();
	}

	// Pause Music
	public void PauseMusic()
	{
		if (musicSource == null)
		{
			return;
		}

		musicSource.Pause();
	}

	// Unpause Music
	public void UnpauseMusic()
	{
		if (musicSource == null)
		{
			return;
		}

		musicSource.UnPause();
	}

	// Stop Music
	public void StopMusic()
	{
		if (musicSource == null)
		{
			return;
		}

		musicSource.Stop();
	}

	#endregion
}