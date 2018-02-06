using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

public class UIMenuLevelSelectScreen : UIMenuScreenManager
{
	// Level Data Array
	public LevelData[] aLevelData;
	// Level Player Times
	string[] asLevelTimeStrings;

	// Level UI BUtton Content Panel Object
	public Transform tLevelButtonContent;
	// UI Button Prefab Object
	public GameObject goUIButtonPrefab;
	// Back Button Object
	public GameObject goBackButton;
	// Level Button List
	List<GameObject> lgoLevelButtons;
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
	// Gold Time Text
	public TextMeshProUGUI goldTime;
	// Silver Time Text
	public TextMeshProUGUI silverTime;
	// Bronze Time Text
	public TextMeshProUGUI bronzeTime;

	// Initialization
	protected override void Start()
	{
		lgoLevelButtons = new List<GameObject>();
		iCurrentLevel = 0;

		asLevelTimeStrings = new string[aLevelData.Length];

		for (int i = 0; i < asLevelTimeStrings.Length; i++)
		{
			float time = ProfileDataManager.Instance.LoadVRMissionTime(aLevelData[i].sLevelName);
			asLevelTimeStrings[i] = time == -1 ? "Best: --:--.--" : "Best: " + LevelTime.SConvertToTimeString(time);
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

		if (aLevelData.Length == 0)
		{
			return;
		}

		// Create and Initialize new buttons
		for (int i = 0; i < aLevelData.Length; i++)
		{
			GameObject buttonObj = Instantiate(goUIButtonPrefab, tLevelButtonContent);
			StartCoroutine(AssignButtonEvent(buttonObj, i));

			buttonObj.GetComponent<UISoundEventManager>().screenManager = this;
			lgoLevelButtons.Add(buttonObj);
		}
	}

	// Initializes the Button
	private IEnumerator AssignButtonEvent(GameObject button, int id)
	{
		button.GetComponent<Button>().onClick.AddListener(delegate { OpenLevel(id); });
		button.GetComponentInChildren<TextMeshProUGUI>().text = aLevelData[id].sLevelName;

		yield return null;
	}

	// Waits a frame before setting the selected object to the first level option if there is one
	private IEnumerator SetObjectSelection()
	{
		yield return new WaitForEndOfFrame();

		if (lgoLevelButtons.Count > 0)
		{
			EventSystem.current.SetSelectedGameObject(lgoLevelButtons[0]);
		}
	}

	// Checks which button is highlighted or selected to update the Level Info UI
	private void CheckButtonSelection()
	{
		if (lgoLevelButtons.Count == 0)
		{
			return;
		}

		if (goCurrentHighlightedButton != null)
		{
			if (goCurrentHighlightedButton == goCurrentSelectedButton)
			{
				return;
			}

			for (int i = 0; i < lgoLevelButtons.Count; i++)
			{
				if (goCurrentHighlightedButton == lgoLevelButtons[i])
				{
					goCurrentSelectedButton = lgoLevelButtons[i];
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

		for (int i = 0; i < lgoLevelButtons.Count; i++)
		{
			if (EventSystem.current.currentSelectedGameObject == lgoLevelButtons[i])
			{
				goCurrentSelectedButton = lgoLevelButtons[i];
				iCurrentLevel = i;
				UpdateUI();
			}
		}
	}

	// Updates the Level Info UI
	private void UpdateUI()
	{
		if (iCurrentLevel < 0 || iCurrentLevel >= aLevelData.Length)
		{
			return;
		}

		LevelData level = aLevelData[iCurrentLevel];

		levelName.text = level.sLevelName;
		levelImage.sprite = level.levelImage;

		if (level.bHasTime)
		{
			playerTime.gameObject.SetActive(true);
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
			goldTime.gameObject.SetActive(false);
			silverTime.gameObject.SetActive(false);
			bronzeTime.gameObject.SetActive(false);
		}
	}

	// Opens the Selected Level (Button Click Event)
	public void OpenLevel(int buttonID)
	{
		if (buttonID < 0 || buttonID >= aLevelData.Length)
		{
			return;
		}

		SceneLoadManager.Instance.LoadScene(aLevelData[buttonID].sSceneName);
        GameManager.DisablePause(false);
        GameManager.SetPause(false);
	}

	// Sets the current button highlighted via Mouse (Pointer Enter Event)
	private void ButtonHighlight(PointerEventData data,  int buttonID)
	{
		if (buttonID < 0 || buttonID >= lgoLevelButtons.Count)
		{
			return;
		}

		goCurrentHighlightedButton = lgoLevelButtons[buttonID];
	}

	// Unassigns the pointer highlight (Pointer Exit Event)
	private void StopHighlight(PointerEventData data)
	{
		goCurrentHighlightedButton = null;
	}
}