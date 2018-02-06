using System.Collections;

using UnityEngine.EventSystems;

using Rewired;

public class UIMenuPauseScreen : UIMenuScreenManager
{
	// Player Input
	Player playerInput;

	// Initialization
	protected override void Start()
	{
		playerInput = ReInput.players.GetPlayer(0);
	}

	// Update
	void Update()
	{
		if (playerInput.GetButtonDown("Cancel"))
		{
			ResumeGame();
		}
	}

	// On Enable
	void OnEnable()
	{
		StartCoroutine(SetSelection());
	}

	// Frame Delay before selection
	private IEnumerator SetSelection()
	{
		EventSystem.current.SetSelectedGameObject(null);

		yield return WaitFor.Frames(1);

		EventSystem.current.SetSelectedGameObject(goFirstSelectedUI);
	}

	// Resumes the Game
	public void ResumeGame()
	{
        GameManager.PauseMenuManager.SetPause(false);
    }

	// Retries the current VR mission
	public void RetryVRMission()
	{
        GameManager.PauseMenuManager.SetPause(false);
        VRMissionModeManager.Instance.RetryMission();
    }

	// Returns to the Main Menu
	public void ReturnToMenu()
	{
		SceneLoadManager.Instance.LoadScene("MainMenu");
	}

	// Destroys the VR Mission Manager
	public void LeaveVRMissionMode()
	{
		VRMissionModeManager.Instance.ExitSceneDestroy();
	}
}
