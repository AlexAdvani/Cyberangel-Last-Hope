using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Rewired;

public class UITabManager : MonoBehaviour
{
	// Player Input
	Player playerInput;

	// Window Tabs
	public UITab[] agoTabs;
	// Current Active Tab Number
	int iCurrentTab = 0;

	// Initialization
	void Start ()
	{
		InitializeTabs();

		playerInput = ReInput.players.GetPlayer(0);
	}

	// On Enable
	void OnEnable()
	{
		SetTabActive(0);
	}

	// Update
	void Update()
	{
		HandleInput();
	}

	// Initialize Tab objects
	private void InitializeTabs()
	{
		for (int i = 0; i < agoTabs.Length; i++)
		{
			if (i == 0)
			{
				agoTabs[i].buttonImage.color = new Color(1, 1, 1, 1);
				agoTabs[i].goPanel.SetActive(true);
				continue;
			}

			agoTabs[i].buttonImage.color = new Color(1, 1, 1, 0.4f);
			agoTabs[i].goPanel.SetActive(false);
		}
	}

	// Handles Input for Switching Tabs
	private void HandleInput()
	{
		if (UIMenuInputMapper.bPolling || UIMenuInputMapper.bAssignWindowOpen)
		{
			return;
		}

		if (playerInput.GetButtonDown("PrevTab"))
		{
			SetPreviousTab();
		}

		if (playerInput.GetButtonDown("NextTab"))
		{
			SetNextTab();
		}
	}

	// Set Previous Tab to Active
	private void SetPreviousTab()
	{
		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 0.4f);
		agoTabs[iCurrentTab].goPanel.SetActive(false);
		iCurrentTab--;

		if (iCurrentTab < 0)
		{
			iCurrentTab = agoTabs.Length - 1;
		}

		agoTabs[iCurrentTab].goPanel.SetActive(true);
		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 1);
		EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);
	}

	// Set Next Tab to Active
	private void SetNextTab()
	{
		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 0.4f);
		agoTabs[iCurrentTab].goPanel.SetActive(false);
		iCurrentTab++;

		if (iCurrentTab >= agoTabs.Length)
		{
			iCurrentTab = 0;
		}

		agoTabs[iCurrentTab].goPanel.SetActive(true);
		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 1);
		EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);
	}

	// Sets a specific numbered tab to active
	public void SetTabActive(int tabNo)
	{
		if (tabNo < 0 || tabNo >= agoTabs.Length)
		{
			Debug.LogError("Invalid Tab Number - Out of Range");
			return;
		}

		if (tabNo == iCurrentTab)
		{
			return;
		}

		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 0.4f);
		agoTabs[iCurrentTab].goPanel.SetActive(false);
		iCurrentTab = tabNo;
		agoTabs[iCurrentTab].goPanel.SetActive(true);
		agoTabs[iCurrentTab].buttonImage.color = new Color(1, 1, 1, 1);
		EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);
	}
}

[System.Serializable]
public class UITab
{
	// Panel GameObject
	public GameObject goPanel;
	// Button Image
	public Image buttonImage;
	// First Selected UI GameObject
	public GameObject goFirstSelectedUI;
}
