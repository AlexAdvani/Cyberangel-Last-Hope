using System;

using UnityEngine;
using UnityEngine.EventSystems;

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
                agoTabs[i].buttonAnimator.SetTrigger("Active");
                agoTabs[i].goPanel.SetActive(true);

                if (agoTabs[i].onTabActivate != null)
                {
                    agoTabs[i].onTabActivate();
                }

                continue;
			}

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
        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Inactive");
        agoTabs[iCurrentTab].goPanel.SetActive(false);
        iCurrentTab--;

		if (iCurrentTab < 0)
		{
			iCurrentTab = agoTabs.Length - 1;
		}

        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Active");
        agoTabs[iCurrentTab].goPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);

        if (agoTabs[iCurrentTab].onTabActivate != null)
        {
            agoTabs[iCurrentTab].onTabActivate();
        }
	}

	// Set Next Tab to Active
	private void SetNextTab()
	{
        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Inactive");
        agoTabs[iCurrentTab].goPanel.SetActive(false);
        iCurrentTab++;

		if (iCurrentTab >= agoTabs.Length)
		{
			iCurrentTab = 0;
		}

        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Active");
        agoTabs[iCurrentTab].goPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);

        if (agoTabs[iCurrentTab].onTabActivate != null)
        {
            agoTabs[iCurrentTab].onTabActivate();
        }
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

        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Inactive");
        agoTabs[iCurrentTab].goPanel.SetActive(false);
        iCurrentTab = tabNo;
        agoTabs[iCurrentTab].buttonAnimator.SetTrigger("Active");
        agoTabs[iCurrentTab].goPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(agoTabs[iCurrentTab].goFirstSelectedUI);

        if (agoTabs[iCurrentTab].onTabActivate != null)
        {
            agoTabs[iCurrentTab].onTabActivate();
        }
    }

    // Mouse Enter event
    public void MouseEnter(int tabID)
    {
        if (tabID == iCurrentTab)
        {
            return;
        }

        agoTabs[tabID].buttonAnimator.SetTrigger("Hovered");
    }

    // Mouse Exit event
    public void MouseExit(int tabID)
    {
        if (tabID == iCurrentTab)
        {
            agoTabs[tabID].buttonAnimator.SetTrigger("Active");
        }
        else
        {
            agoTabs[tabID].buttonAnimator.SetTrigger("Inactive");
        }
    }
}

[System.Serializable]
public class UITab
{
    // Tab Panel GameObject
    public GameObject goPanel;
	// Button Animator
	public Animator buttonAnimator;
	// First Selected UI GameObject
	public GameObject goFirstSelectedUI;

    // On Tab Activate Action
    public Action onTabActivate;
}
