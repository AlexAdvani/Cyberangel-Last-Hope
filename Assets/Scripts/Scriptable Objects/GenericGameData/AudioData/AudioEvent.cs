using UnityEngine;
using UnityEngine.Audio;

public abstract class AudioEvent : ScriptableObject
{
	// Audio Name 
	public string sAudioSlot;

	// Audio Clips
	public AudioClip[] aacClips;

	// Audio Mixer Group
	public AudioMixerGroup audioMixerGroup;

	// Volume Range
	public Vector2 v2Volume = Vector2.one;
	// Pitch Range
	public Vector2 v2Pitch = Vector2.one;

	// Loop flag
	public bool bLoop;

	// Play Sound
	public abstract void Play(bool audioOverride = true);
	// Pause Sound
	public abstract void Pause();
	// Unpause Sound
	public abstract void Unpause();
	// Stop Sound
	public abstract void Stop();
}
