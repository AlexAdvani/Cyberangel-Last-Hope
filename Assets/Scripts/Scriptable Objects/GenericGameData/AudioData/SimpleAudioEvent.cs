using UnityEngine;

[CreateAssetMenu(menuName = "Generic Game Data/Audio Data/Simple Audio Event")]
public class SimpleAudioEvent : AudioEvent
{
	// Play sound
	public override void Play(bool audioOverride = true)
	{
		if (aacClips.Length == 0)
		{
			return;
		}

		if (!audioOverride && AudioManager.Instance.BGetIsPlaying(sAudioSlot))
		{
			return;
		}

		int clipNo = aacClips.Length == 1 ? 0 : Random.Range(0, aacClips.Length);
		float volume = Random.Range(v2Volume.x, v2Volume.y);
		float pitch = Random.Range(v2Pitch.x, v2Pitch.y);

		AudioManager.Instance.PlaySound(sAudioSlot, aacClips[clipNo], audioMixerGroup, volume, pitch, bLoop);
	}

	// Pause Sound
	public override void Pause()
	{
		AudioManager.Instance.PauseSound(sAudioSlot);
	}

	// Unpause Sound
	public override void Unpause()
	{
		AudioManager.Instance.UnpauseSound(sAudioSlot);
	}

	// Stop Sound
	public override void Stop()
	{
		AudioManager.Instance.StopSound(sAudioSlot);
	}
}
