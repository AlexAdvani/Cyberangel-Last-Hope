using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIMenuGraphicsOptionsScreen : UIMenuScreenManager
{
    // Resolution Dropdown
	public TMP_Dropdown resolutionDropdown;
    // Display Mode Dropdown
	public TMP_Dropdown displayModeDropdown;
    // Active Monitor Dropdown
	public TMP_Dropdown monitorDropdown;
    // VSync Toggle
	public Toggle vSyncToggle;

	// Initialization
	protected override void Start ()
	{
		base.Start();

        bool settingsChanged = SettingsManager.bOptionChanged;

        resolutionDropdown.value = GlobalSettings.iResolution;
        resolutionDropdown.RefreshShownValue();
		displayModeDropdown.value = GlobalSettings.iDisplayMode;
        displayModeDropdown.RefreshShownValue();
        InitializeMonitorDropdown();
		vSyncToggle.isOn = GlobalSettings.bVSync;

        SettingsManager.bOptionChanged = settingsChanged;
	}

	// Initialize Monitor Dropdown Value
	private void InitializeMonitorDropdown()
	{
		monitorDropdown.ClearOptions();

		int unityMonitor = PlayerPrefs.GetInt("");
		int currentMonitor = unityMonitor == 0 ? GlobalSettings.iMonitor : unityMonitor;

		for (int i = 0; i < Display.displays.Length; i++)
		{
			monitorDropdown.options.Add(new TMP_Dropdown.OptionData("Monitor " + (i + 1)));
		}

		if (currentMonitor < Display.displays.Length)
		{
			monitorDropdown.value = currentMonitor;
		}
		else
		{
			monitorDropdown.value = 0;
			GlobalSettings.iMonitor = 0;
		}

		if (monitorDropdown.options.Count == 1)
		{
			Toggle option = monitorDropdown.GetComponentInChildren<Toggle>(true);

			if (option != null)
			{
				Navigation nav = new Navigation()
				{
					mode = Navigation.Mode.None
				};
				option.navigation = nav;
			}
		}

		monitorDropdown.RefreshShownValue();
	}

	// Changes the Resolution Based on ID Value
	public void ChangeResolution(int resolutionID)
	{
		int width = 0;
		int height = 0;

		switch (resolutionID)
		{
			case 0: // 800 x 600
				width = 800;
				height = 600;
				break;

			case 1: // 1024 x 768
				width = 1024;
				height = 768;
				break;

			case 2: // 1280 x 720
				width = 1280;
				height = 720;
				break;

			case 3: // 1280 x 768
				width = 1280;
				height = 768;
				break;

			case 4: // 1280 x 800
				width = 1280;
				height = 800;
				break;

			case 5: // 1366 x 768
				width = 1366;
				height = 768;
				break;

			case 6: // 1440 x 900
				width = 1440;
				height = 900;
				break;

			case 7: // 1600 x 900
				width = 1600;
				height = 900;
				break;

			case 8: // 1680 x 1050
				width = 1680;
				height = 1050;
				break;

			case 9: // 1920 x 1080
				width = 1920;
				height = 1080;
				break;

			case 10: // 1920 x 1200
				width = 1920;
				height = 1200;
				break;

			case 11: // 2560 x 1080
				width = 2560;
				height = 1080;
				break;

			case 12: // 2560 x 1440
				width = 2560;
				height = 1440;
				break;

			case 13: // 3440 x 1440
				width = 3440;
				height = 1440;
				break;

			case 14: // 3840 x 2160
				width = 3840;
				height = 2160;
				break;
		}

		// If width or height are equal to 0, then resolution was not found
		if (width == 0 || height == 0)
		{
			return;
		}

		Screen.SetResolution(width, height, Screen.fullScreen);
		GlobalSettings.iResolution = resolutionID;
		SettingsManager.bOptionChanged = true;
	}

	// Change Display Mode Based on ID Value
	public void ChangeDisplayMode(int displayID)
	{
		switch (displayID)
		{
			case 0: // Windowed
				Screen.fullScreen = false;
				break;

			case 1: // Fullscreen
				Screen.fullScreen = true;
				break;
		}

		GlobalSettings.iDisplayMode = displayID;
		SettingsManager.bOptionChanged = true;
	}

	// Change Monitor That Game Is Shown On
	public void ChangeMonitor(int monitor)
	{
		StartCoroutine(SetMonitor(monitor));
		GlobalSettings.iMonitor = monitor;
		SettingsManager.bOptionChanged = true;
	}

	// Sets the Monitor and then refreshes the screen to render correctly
	private IEnumerator SetMonitor(int monitor)
	{
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;

		PlayerPrefs.SetInt("UnitySelectMonitor", monitor);
		Screen.SetResolution(800, 600, Screen.fullScreen);

		yield return null;

		Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
	}

	// Set VSync
	public void SetVSync(bool vSync)
	{
		if (vSync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}

		GlobalSettings.bVSync = vSync;
		SettingsManager.bOptionChanged = true;
	}
}
