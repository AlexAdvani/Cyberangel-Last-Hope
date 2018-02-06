using System.Collections;

using UnityEngine;

public class PauseMenuManager : UIMenuManager
{
	// In Game UI
	public GameObject goInGameUI;
	// Pause Menu UI
	public GameObject goPauseUI;
	// Is Paused flag
	bool bPaused = false;

	#region Public Properties

	// Paused flag
	public bool Paused
	{
		get { return bPaused; }
	}

	#endregion

	// Initialization
	protected override void Awake()
	{
		base.Awake();
		bMenuInputDisabled = true;

        SettingsManager.CheckInstanceExists();

		goPauseUI.SetActive(false);
	}

	// Update
	public override void Update()
	{
		base.Update();

		HandlePauseInput();
	}

	// On Destroy
	void OnDestroy()
	{
        GameManager.RemovePauseMenuManager();
	}

	// Handles Pause Input
	private void HandlePauseInput()
	{
		if (playerInput.GetButtonDown ("Pause"))
		{
			if (!GameManager.PauseDisabled)
			{
				if (!bPaused)
				{
					PauseGame();
				}
				else if (sCurrentPanel == "PauseMenu")
				{
					StartCoroutine(UnpauseGame());
				}
			}
		}
	}

	// Toggles the activation of the Pause Menu
	public void TogglePause()
	{
		if (bPaused)
		{
			StartCoroutine(UnpauseGame());
		}
		else
		{
			PauseGame();
		}
	}

	// Sets the activation of the Pause Menu
	public void SetPause(bool paused)
	{
		if (paused)
		{
			PauseGame();
		}
		else
		{
			StartCoroutine(UnpauseGame());
		}
	}

	// Pauses the game
	private void PauseGame()
	{
		if (bPaused)
		{
			return;
		}

		bPaused = true;
		GameManager.SetPause(true);
		GameManager.Player.SetDisabledMovement(true);
		GameManager.Player.SetPlayerControlDisabled(true);
		Time.timeScale = 0;
		goInGameUI.SetActive(false);
		goPauseUI.SetActive(true);

		bMenuInputDisabled = false;
		AudioManager.Instance.PauseAllSounds();
		MusicManager.Instance.PauseMusic();
		GoToScreen("PauseMenu");

        // Force Garbage Collection
        System.GC.Collect();
	}

	// Unpauses the game
	private IEnumerator UnpauseGame()
	{
		bPaused = false;
		GameManager.SetPause(false);
		StartCoroutine(ReenablePlayerControl());
		Time.timeScale = 1;
		goInGameUI.SetActive(true);

		if (currentScreenManager != null)
		{
			currentScreenManager.CloseScreen(null);

			sCurrentPanel = null;
			currentScreenManager = null;
			goCurrentPanelObj = null;
		}

		yield return new WaitForSeconds(0.1f);

		goPauseUI.SetActive(false);
		bMenuInputDisabled = true;
		AudioManager.Instance.UnpauseAllSounds();
		MusicManager.Instance.UnpauseMusic();

		yield return null;
	}

	// Reenables Player Control and Movement
	private IEnumerator ReenablePlayerControl()
	{
		yield return WaitFor.Frames(1);

		GameManager.Player.SetDisabledMovement(false);
		GameManager.Player.SetPlayerControlDisabled(false);
	}

    // Return to previous screen
    public override void ReturnToPreviousScreen()
    {
        if (sCurrentPanel == "Options")
        {
            SettingsManager.Instance.SaveSettings();
        }

        base.ReturnToPreviousScreen();
    }
}
