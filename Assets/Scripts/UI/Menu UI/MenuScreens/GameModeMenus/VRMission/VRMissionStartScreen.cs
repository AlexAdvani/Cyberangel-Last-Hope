using UnityEngine;

using TMPro;

public class VRMissionStartScreen : UIMenuScreenManager
{
	// Targets To Destroy Text
	public TextMeshProUGUI targetText;
	// Best Time Text (Start Screen)
	public TextMeshProUGUI bestTimeText;
	// Gold Time Text
	public TextMeshProUGUI goldText;
	// Silver Time Text
	public TextMeshProUGUI silverText;
	// Bronze Time Text
	public TextMeshProUGUI bronzeText;

	// Initialization
	protected override void Start ()
	{
		InitializeStartUI();

		VRMissionModeManager.Instance.goInGameUI.SetActive(false);
		GameManager.PauseMenuManager.SetPause(false);
		GameManager.SetPause (true);
		GameManager.DisablePause (true);
		Time.timeScale = 1;

		UIMenuManager.bMenuInputDisabled = false;
	}

	// Initialize Start Screen UI
	private void InitializeStartUI()
	{
		targetText.text = "Targets: " + VRMissionModeManager.Instance.Targets;
		goldText.text = "Gold: " + VRMissionModeManager.Instance.levelData.goldTime.SGetTimeString();
		silverText.text = "Silver: " + VRMissionModeManager.Instance.levelData.silverTime.SGetTimeString();
		bronzeText.text = "Bronze: " + VRMissionModeManager.Instance.levelData.bronzeTime.SGetTimeString();

		if (VRMissionModeManager.Instance.BestTime != -1)
		{
			bestTimeText.text = "Best: " + LevelTime.SConvertToTimeString(VRMissionModeManager.Instance.BestTime);
		}
	}

	// Quit to Main Menu
	public void QuitToMain()
	{
		VRMissionModeManager.Instance.ExitSceneDestroy();
		GoToScene("MainMenu");
	}
}
