using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIMenuAudioOptionsScreen : UIMenuScreenManager
{
	public Slider masterVolSlider;
	public Slider soundVolSlider;
	public Slider musicVolSlider;
	public Toggle muteToggle;

	public TextMeshProUGUI masterVolumeText;
	public TextMeshProUGUI soundVolumeText;
	public TextMeshProUGUI musicVolumeText;

	// Initialization
	protected override void Start ()
	{
		base.Start();

		masterVolSlider.value = GlobalSettings.fMasterVolume * 100;
		musicVolSlider.value = GlobalSettings.fMusicVolume * 100;
		soundVolSlider.value = GlobalSettings.fSoundVolume * 100;
		muteToggle.isOn = GlobalSettings.bMute;
	}

	// Set Master Volume
	public void SetMasterVolume(float volume)
	{
		GlobalSettings.fMasterVolume = volume / 100;
		masterVolumeText.text = volume.ToString();

		AudioManager.Instance.SetVolume("MasterVolume", GlobalSettings.fMasterVolume);
		SettingsManager.bOptionChanged = true;
	}

	// Set Sound Volume
	public void SetSoundVolume(float volume)
	{
		GlobalSettings.fSoundVolume = volume / 100;
		soundVolumeText.text = volume.ToString();

		AudioManager.Instance.SetVolume("SoundVolume", GlobalSettings.fSoundVolume);
		SettingsManager.bOptionChanged = true;
	}

	// Set Music Volume
	public void SetMusicVolume(float volume)
	{
		GlobalSettings.fMusicVolume = volume / 100;
		musicVolumeText.text = volume.ToString();

		AudioManager.Instance.SetVolume("MusicVolume", GlobalSettings.fMusicVolume);
		SettingsManager.bOptionChanged = true;
	}

	// Set Mute
	public void SetMute(bool mute)
	{
		GlobalSettings.bMute = mute;

		AudioManager.Instance.SetMute(mute);
		SettingsManager.bOptionChanged = true;
	}
}
