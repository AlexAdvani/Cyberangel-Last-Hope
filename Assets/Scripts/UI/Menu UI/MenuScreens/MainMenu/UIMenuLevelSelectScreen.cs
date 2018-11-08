using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

public class UIMenuLevelSelectScreen : UIMenuScreenManager
{
    // Level Data Array
    public LevelListData levelList;
	// Level Player Times
	string[] asLevelTimeStrings;
    // Level Player Scores
    string[] asLevelScoreStrings;

	// Level UI BUtton Content Panel Object
	public Transform tLevelButtonContent;
	// UI Button Prefab Object
	public GameObject goUIButtonPrefab;
	// Back Button Object
	public GameObject goBackButton;
    // Number of Level Buttons in a Row
    public int iLevelButtonsInRow = 5;
    // Level Button Array
    LevelSelectButton[] aLevelButtons;
    // Currently Selected Level Button Object
    GameObject goCurrentSelectedButton;
	// Currently Highlighted Level Button Object (Mouse)
	GameObject goCurrentHighlightedButton;
	// Currently Selected Level ID
	int iCurrentLevel;

	[Space()]

	// Level Name Text
	public TextMeshProUGUI levelName;
	// Level Image
	public Image levelImage;
	// Player Time Text
	public TextMeshProUGUI playerTime;
    // Par Time Text
    public TextMeshProUGUI parTime;
	// Gold Time Text
	public TextMeshProUGUI goldTime;
	// Silver Time Text
	public TextMeshProUGUI silverTime;
	// Bronze Time Text
	public TextMeshProUGUI bronzeTime;

	// Initialization
	protected override void Start()
	{
		iCurrentLevel = 0;

		asLevelTimeStrings = new string[levelList.aLevelData.Length];
        asLevelScoreStrings = new string[levelList.aLevelData.Length];

		for (int i = 0; i < asLevelTimeStrings.Length; i++)
		{
			float time = ProfileDataManager.Instance.LoadVRMissionTime(levelList.aLevelData[i].sLevelName);
            int score = ProfileDataManager.Instance.LoadVRMissionScore(levelList.aLevelData[i].sLevelName);
			asLevelTimeStrings[i] = time == -1 ? "Best: --:--.--" : "Best: " + LevelTime.SConvertToTimeString(time);
            asLevelScoreStrings[i] = score == -1 ? "Best: -----" : "Best: " + score.ToString();
		}

		InitializeButtons();
		UpdateUI();

		StartCoroutine(SetObjectSelection());
	}

	// Update
	void Update()
	{
		CheckButtonSelection();
	}

	// Creates and Initializes UI Buttons to Select Levels
	private void InitializeButtons()
	{
		// Delete old Level UI Buttons
		foreach (Transform child in tLevelButtonContent)
		{
			Destroy(child.gameObject);
		}

		if (levelList.aLevelData.Length == 0)
		{
			return;
		}

        // Level Button Array 
        aLevelButtons = new LevelSelectButton[levelList.aLevelData.Length];
        // Number of Grid Rows
        int gridRows = aLevelButtons.Length / iLevelButtonsInRow;

        if (aLevelButtons.Length % iLevelButtonsInRow != 0)
        {
            gridRows++;
        }

        // Back Button
        Button backButton = goBackButton.GetComponent<Button>();

		// Create and Initialize new buttons
		for (int i = 0; i < levelList.aLevelData.Length; i++)
		{
			GameObject buttonObj = Instantiate(goUIButtonPrefab, tLevelButtonContent);
            aLevelButtons[i] = buttonObj.GetComponent<LevelSelectButton>();
            aLevelButtons[i].InitializeButton(levelList.aLevelData[i].levelImage, i, OpenLevel, ButtonHighlight, StopHighlight);
            buttonObj.GetComponent<UISoundEventManager>().screenManager = this;
		}

        // Navigation Loop
        for (int j = 0; j < aLevelButtons.Length; j++)
        {
            Button button = aLevelButtons[j].LevelButton;
            Navigation nav = button.navigation;
            int rowNumber = j / iLevelButtonsInRow;
            int columnNumber = j % iLevelButtonsInRow;

            nav.mode = Navigation.Mode.Explicit;

            // Horizontal Nav
            // Left
            if (columnNumber > 0)
            {
                nav.selectOnLeft = aLevelButtons[j - 1].LevelButton;
            }

            // Right
            if (j < (aLevelButtons.Length - 1))
            {
                if (columnNumber < (iLevelButtonsInRow - 1))
                {
                    nav.selectOnRight = aLevelButtons[j + 1].LevelButton;
                }
            }

            // Vertical Nav
            // Up
            if (rowNumber > 0)
            {
                nav.selectOnUp = aLevelButtons[j - iLevelButtonsInRow].LevelButton;
            }

            // Down
            if (rowNumber < (gridRows - 1))
            {
                nav.selectOnDown = aLevelButtons[j + iLevelButtonsInRow].LevelButton;
            }
            else
            {
                nav.selectOnDown = backButton;
            }

            button.navigation = nav;
        }

        // Back Button Navigation
        if (aLevelButtons.Length > 0)
        {
            Navigation backNav = backButton.navigation;

            backNav.selectOnUp = aLevelButtons[(gridRows - 1) * iLevelButtonsInRow].LevelButton;

            backButton.navigation = backNav;
        }
	}

	// Waits a frame before setting the selected object to the first level option if there is one
	private IEnumerator SetObjectSelection()
	{
		yield return new WaitForEndOfFrame();

		if (aLevelButtons.Length > 0)
		{
			EventSystem.current.SetSelectedGameObject(aLevelButtons[0].gameObject);
		}
	}

	// Checks which button is highlighted or selected to update the Level Info UI
	private void CheckButtonSelection()
	{
		if (aLevelButtons.Length == 0)
		{
			return;
		}

		if (goCurrentHighlightedButton != null)
		{
			if (goCurrentHighlightedButton == goCurrentSelectedButton)
			{
				return;
			}

			for (int i = 0; i < aLevelButtons.Length; i++)
			{
				if (goCurrentHighlightedButton == aLevelButtons[i].gameObject)
				{
					goCurrentSelectedButton = aLevelButtons[i].gameObject;
					iCurrentLevel = i;
					UpdateUI();
				}
			}

			return;
		}

		if (EventSystem.current.currentSelectedGameObject == goCurrentSelectedButton)
		{
			return;
		}

		for (int i = 0; i < aLevelButtons.Length; i++)
		{
			if (EventSystem.current.currentSelectedGameObject == aLevelButtons[i].gameObject)
			{
				goCurrentSelectedButton = aLevelButtons[i].gameObject;
				iCurrentLevel = i;
				UpdateUI();
			}
		}
	}

	// Updates the Level Info UI
	private void UpdateUI()
	{
		if (iCurrentLevel < 0 || iCurrentLevel >= levelList.aLevelData.Length)
		{
			return;
		}

		LevelData level = levelList.aLevelData[iCurrentLevel];

		levelName.text = level.sLevelName;
		levelImage.sprite = level.levelImage;

        if (level.bHasScore)
        {
            playerTime.gameObject.SetActive(true);
            parTime.gameObject.SetActive(true);
            goldTime.gameObject.SetActive(true);
            silverTime.gameObject.SetActive(true);
            bronzeTime.gameObject.SetActive(true);

            playerTime.text = asLevelScoreStrings[iCurrentLevel];
            parTime.text = level.parTime.SGetTimeString();
            goldTime.text = "Gold: " + level.iGoldScore.ToString();
            silverTime.text = "Silver: " + level.iSilverScore.ToString();
            bronzeTime.text = "Bronze: " + level.iBronzeScore.ToString();
            
        }
		else if (level.bHasTime)
		{
			playerTime.gameObject.SetActive(true);
            parTime.gameObject.SetActive(false);
            goldTime.gameObject.SetActive(true);
			silverTime.gameObject.SetActive(true);
			bronzeTime.gameObject.SetActive(true);

            playerTime.text = asLevelTimeStrings[iCurrentLevel];
            goldTime.text = "Gold: " + level.goldTime.SGetTimeString();
            silverTime.text = "Silver: " + level.silverTime.SGetTimeString();
            bronzeTime.text = "Bronze: " + level.bronzeTime.SGetTimeString();
        }
		else
		{
            playerTime.gameObject.SetActive(false);
            parTime.gameObject.SetActive(false);
            goldTime.gameObject.SetActive(false);
			silverTime.gameObject.SetActive(false);
			bronzeTime.gameObject.SetActive(false);
		}
	}

	// Opens the Selected Level (Button Click Event)
	public void OpenLevel(int buttonID)
	{
		if (buttonID < 0 || buttonID >= levelList.aLevelData.Length)
		{
			return;
		}

		SceneLoadManager.Instance.LoadScene(levelList.aLevelData[buttonID].sSceneName);
        GameManager.DisablePause(false);
        GameManager.SetPause(false);
	}

	// Sets the current button highlighted via Mouse (Pointer Enter Event)
	private void ButtonHighlight(int buttonID)
	{
		if (buttonID < 0 || buttonID >= aLevelButtons.Length)
		{
			return;
		}

        goCurrentHighlightedButton = aLevelButtons[buttonID].gameObject;
	}

	// Unassigns the pointer highlight (Pointer Exit Event)
	private void StopHighlight()
	{
		goCurrentHighlightedButton = null;
	}
}