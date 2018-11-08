using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;

public class VRMissionStartScreen : UIMenuScreenManager
{
	// Targets To Destroy Text
	public TextMeshProUGUI targetText;
	// Best Time Text (Start Screen)
	public TextMeshProUGUI bestTimeText;
    // Par Time Text (Score Only TEST)
    public TextMeshProUGUI parTimeText;
    // Gold Time Text
    public TextMeshProUGUI goldText;
	// Silver Time Text
	public TextMeshProUGUI silverText;
	// Bronze Time Text
	public TextMeshProUGUI bronzeText;

    // Buttons UI Panel
    public GameObject goButtonsPanel;
    // Info UI Panel
    public GameObject goInfoPanel;
    // View Level Panel
    public GameObject goViewLevelPanel;

    // View Level Action ID for Input Legend
    [Rewired.ActionIdProperty(typeof(RewiredActions))]
    public int iViewLevelActionID;

    // Selected UI Object Before Entering View Level
    GameObject goPreViewSelectedObject;

    // Is Viewing Level flag
    bool bViewingLevel = false;

    // Initialization
    protected override void Start ()
	{
		InitializeStartUI();

        GameManager.PauseMenuManager.SetPause(false);
		VRMissionModeManager.Instance.goInGameUI.SetActive(false);
        VRMissionModeManager.Instance.goMissionUI.SetActive(true);
        VRMissionModeManager.Instance.goInputButtonsUI.SetActive(true);
		GameManager.SetPause (true);
		GameManager.DisablePause (true);
		Time.timeScale = 1;

		UIMenuManager.bMenuInputDisabled = false;
	}

    // On Enable
    void OnEnable()
    {
        MenuManager.LegendManager.AddLegend(iViewLevelActionID, "View Level");
    }

    // On Disable
    void OnDisable()
    {
        MenuManager.LegendManager.RemoveLegend("View Level");
    }

    // Cancel Function
    public override bool HandleCancelFunction()
    {
        if (bViewingLevel)
        {
            StopViewingLevel();
            return true;
        }

        return false;
    }

    // Extra Inputs
    public override void HandleExtraInput(Rewired.Player input)
    {
        if (input.GetButtonDown("ViewLevel"))
        {
            StartViewingLevel();
        }
    }

    // Initialize Start Screen UI
    private void InitializeStartUI()
	{
        LevelData data = VRMissionModeManager.Instance.levelData;

        targetText.text = "Targets: " + VRMissionModeManager.Instance.Targets;

        if (data.bHasScore)
        {
            if (VRMissionModeManager.Instance.BestScore != -1)
            {
                bestTimeText.text = "Best: " + VRMissionModeManager.Instance.BestScore.ToString();
            }

            parTimeText.gameObject.SetActive(true);
            parTimeText.text = "Par Time: " + data.parTime.SGetTimeString();

            goldText.text = "Gold: " + data.iGoldScore.ToString();
            silverText.text = "Silver: " + data.iSilverScore.ToString();
            bronzeText.text = "Bronze: " + data.iBronzeScore.ToString();
        }
        else
        {
            if (VRMissionModeManager.Instance.BestTime != -1)
            {
                bestTimeText.text = "Best: " + LevelTime.SConvertToTimeString(VRMissionModeManager.Instance.BestTime);
            }

            goldText.text = "Gold: " + data.goldTime.SGetTimeString();
            silverText.text = "Silver: " + data.silverTime.SGetTimeString();
            bronzeText.text = "Bronze: " + data.bronzeTime.SGetTimeString();
        }		
	}

	// Quit to Main Menu
	public void QuitToMain()
	{
		VRMissionModeManager.Instance.ExitSceneDestroy();
		GoToScene("MainMenu");
	}

    // Start Viewing Level
    private void StartViewingLevel()
    {
        goButtonsPanel.SetActive(false);
        goInfoPanel.SetActive(false);
        goViewLevelPanel.SetActive(true);

        MenuManager.LegendManager.gameObject.SetActive(false);
        goPreViewSelectedObject = EventSystem.current.currentSelectedGameObject;

        bViewingLevel = true;
    }

    // Stop Viewing Level
    private void StopViewingLevel()
    {
        goButtonsPanel.SetActive(true);
        goInfoPanel.SetActive(true);
        goViewLevelPanel.SetActive(false);

        MenuManager.LegendManager.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(goPreViewSelectedObject);

        bViewingLevel = false;
    }
}
