using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class VRMissionEndScreen : UIMenuScreenManager
{
	// Next Mission Button
	public Button nextMissionButton;
	// End Time Text
	public TextMeshProUGUI endTimeText;
    // Par Time Text
    public TextMeshProUGUI parTimeText;
    // Time Bonus Text
    public TextMeshProUGUI timeBonusText;
	// Best Time Text (End Screen)
	public TextMeshProUGUI endBestTimeText;
	// End Rank Text
	public TextMeshProUGUI endRankText;

	// Gold Rank Text Colour
	public Color goldRankColour;
	// Silver Rank Text Colour
	public Color silverRankColour;
	// Bronze Rank Text Colour
	public Color bronzeRankColour;
	// No Rank Text Colour
	public Color noRankColour;

	// Initialization
	protected override void Start()
	{
		VRMissionModeManager.Instance.goInGameUI.SetActive(false);
        VRMissionModeManager.Instance.goMissionUI.SetActive(false);
		GameManager.SetPause(true);
		GameManager.DisablePause(true);
		UIMenuManager.bMenuInputDisabled = false;

		InitializeUI();
	}

	// Initializes end screen UI
	private void InitializeUI()
	{
		if (VRMissionModeManager.Instance.nextMissionLevelData == null)
		{
			nextMissionButton.interactable = false;
		}

        if (VRMissionModeManager.Instance.levelData.bHasScore)
        {
            endTimeText.text = "Score: " + VRMissionModeManager.Instance.missionScore.Score.ToString();

            parTimeText.gameObject.SetActive(true);
            parTimeText.text = "Par Time: " + VRMissionModeManager.Instance.levelData.parTime.SGetTimeString();

            timeBonusText.gameObject.SetActive(true);
            timeBonusText.text = "Time Bonus: " + VRMissionModeManager.Instance.TimeBonus.ToString();

            endBestTimeText.text = "Best: " + VRMissionModeManager.Instance.BestScore.ToString();
            endRankText.text = "Rank: " + VRMissionModeManager.Instance.EndRank;
        }
        else
        {
            endTimeText.text = "Time: " + LevelTime.SConvertToTimeString(VRMissionModeManager.Instance.missionTimer.CurrentTime);
            endBestTimeText.text = "Best: " + LevelTime.SConvertToTimeString(VRMissionModeManager.Instance.BestTime);
            endRankText.text = "Rank: " + VRMissionModeManager.Instance.EndRank;
        }

		switch (VRMissionModeManager.Instance.EndRank)
		{
			case "Gold":
				endRankText.color = goldRankColour;
				break;

			case "Silver":
				endRankText.color = silverRankColour;
				break;

			case "Bronze":
				endRankText.color = bronzeRankColour;
				break;

			default:
				endRankText.color = noRankColour;
				break;
		}
	}

	// Retry the mission
	public void RetryMission()
	{
		VRMissionModeManager.Instance.RetryMission();
	}

	// Go to next mission
	public void NextMission()
	{
		if (VRMissionModeManager.Instance.nextMissionLevelData == null)
		{
			return;
		}

		GoToScene(VRMissionModeManager.Instance.nextMissionLevelData.sSceneName);
	}

	// Quit to Main Menu
	public void QuitToMain()
	{
		VRMissionModeManager.Instance.ExitSceneDestroy();
		GoToScene("MainMenu");
	}
}
