using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;

public class MainMenuManager : UIMenuManager
{
	// Menu Input Panel
	public GameObject goMenuInputPanel;

    // Initialization
	void Start()
	{
		bMenuInputDisabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
	}

	// Update
	public override void Update()
	{
		base.Update();

		HandleMenuInput();
	}

	// Handles Menu Input
	private void HandleMenuInput()
	{
		if (UIMenuInputMapper.bPolling || bMenuInputDisabled)
		{
			return;
		}

		if (playerInput.GetButtonDown("MenuStart"))
		{
			if (sCurrentPanel == "Start")
			{
				GoToScreen("MainMenu");
				TransitionHelper.TransitionIn(goMenuInputPanel);
			}
		}
	}

	// Return to previous screen
	public override void ReturnToPreviousScreen()
	{
		if (sCurrentPanel == "MainMenu")
		{
			TransitionHelper.TransitionOut(goMenuInputPanel);
		}
		else if (sCurrentPanel == "Options")
		{
			SettingsManager.Instance.SaveSettings();
		}

		base.ReturnToPreviousScreen();
	}
}
