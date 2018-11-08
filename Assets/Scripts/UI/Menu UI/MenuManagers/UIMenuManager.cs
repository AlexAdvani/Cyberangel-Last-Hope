using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Rewired;

public class UIMenuManager : MonoBehaviour
{
	// Last Selected UI Object
	[HideInInspector]
	public GameObject goLastSelectedObject;

	// Player Input
	protected Player playerInput;

	// Menu Screen Parent
	public Transform tMenuParent;

	// Menu Screen Dictionary
	[Inspectionary("Name", "Screen")]
	public UIMenuScreenDictionary dMenuScreens;
	// Current Panel ID
	protected string sCurrentPanel = null;
	// Stack of previous screens 
	protected Stack<string> prevScreens;
	// Stack of last selected gameobjects for use when returning to screens
	protected Stack<string> prevSelectedScreenUINames;
	// Current Panel Object
	protected GameObject goCurrentPanelObj = null;
	// Current Screen Manager
	protected UIMenuScreenManager currentScreenManager;

	// Menu Window Dictionary
	[Inspectionary("Name", "Window")]
	public UIMenuWindowDictionary dMenuWindows;
	// Current Open Windows
	protected Stack<UIMenuWindowManager> currentWindows;
	// Current Window Manager
	protected UIMenuWindowManager currentWindowManager;
	// Previous Selected Object before the last window was openede
	protected Stack<GameObject> prevSelectedWindowUIObjects;

    // Clear Menu Parent of all windows on Awake flag
    public bool bClearMenuParentOnAwake = false;

	// Start Instantiated flag
	public bool bStartInstantiated = true;
	// Start UI Selected flag
	public bool bStartNotSelected = false;
	// Disable Cancel Function flag
	public bool bDisableCancelFunction = false;
	// Store Previous Screen Selections flag
	public bool bStorePreviousSelections = true;

	// Override Normal Cancel Function flag
	public static bool bOverrideCancelFunction = false;

	// Menu Input Disabled flag
	public static bool bMenuInputDisabled = true;

	// In Screen Transition flag
	[HideInInspector]
	public bool bScreenTransition = false;

	// Button Highlight Audio Data
	public AudioEvent highlightAudio;
	// Button Confirm Audio Data
	public AudioEvent confirmAudio;
	// Button Cancel Audio Data
	public AudioEvent cancelAudio;

    // UI Input Legend Manager
    UIInputLegendManager legendManager;

	#region Public Properties

	// Current Screen Manager
	public UIMenuScreenManager CurrentScreen
	{
		get { return currentScreenManager; }
	}

    public UIInputLegendManager LegendManager
    {
        get
        {
            if (legendManager == null)
            {
                legendManager = transform.parent.Find("UIMenuInputLegends").GetComponent<UIInputLegendManager>();
            }

            return legendManager;
        }
    }


	#endregion

	// Initialization
	protected virtual void Awake()
	{
		playerInput = ReInput.players.GetPlayer(0);

        // If we need to clear on awake, then loop through all of the menu windows and destroy them
        if (bClearMenuParentOnAwake)
        {
            int windowCount = tMenuParent.childCount;

            if (windowCount > 0)
            {
                for (int i = 0; i < windowCount; i++)
                {
                    Destroy(tMenuParent.GetChild(i).gameObject);
                }
            }
        }

		// Set Current Panel if starting instantiated
		if (bStartInstantiated)
		{
			sCurrentPanel = dMenuScreens.First().Key;
			goCurrentPanelObj = Instantiate(dMenuScreens.First().Value.goScreenPanel, tMenuParent);
			currentScreenManager = goCurrentPanelObj.GetComponent<UIMenuScreenManager>();

			if (currentScreenManager != null)
			{
                currentScreenManager.SetMenuManager(this);
                currentScreenManager.gameObject.SetActive(true);

				if (!bStartNotSelected)
				{
					currentScreenManager.OpenScreen(null);
				}
			}
		}

		prevScreens = new Stack<string>();
		prevSelectedScreenUINames = new Stack<string>();

		currentWindows = new Stack<UIMenuWindowManager>();
	}

	// Update
	public virtual void Update()
	{
		HandleMenuInput();
		HandleUISelection();
	}

    // Handles Menu Input
    private void HandleMenuInput()
    {
        if (!tMenuParent.gameObject.activeInHierarchy)
        {
            return;
        }

        if (playerInput.GetButtonDown("UISubmit"))
        {
            if (!bMenuInputDisabled)
            {
                PlayConfirmSound();
            }
        }

        if (playerInput.GetButtonDown("UICancel"))
        {
            if (!bDisableCancelFunction)
            {
                if (!bOverrideCancelFunction)
                {
                    if (!bScreenTransition)
                    {
                        if (currentWindows.Count > 0)
                        {
                            CloseTopWindow();
                            PlayCancelSound();
                        }
                        else
                        {
                            if (!currentScreenManager.HandleCancelFunction())
                            {
                                ReturnToPreviousScreen();
                            }

                            PlayCancelSound();
                        }
                    }
                }
            }
        }

        if (currentScreenManager != null)
        {
            currentScreenManager.HandleExtraInput(playerInput);
        }
	}

	// Handles UI Selection if lost or changed
	private void HandleUISelection()
	{
		if (goLastSelectedObject == EventSystem.current.currentSelectedGameObject)
		{
			return;
		}

		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (goLastSelectedObject != null)
			{
				EventSystem.current.SetSelectedGameObject(goLastSelectedObject);
			}
			else if (currentScreenManager != null)
			{
				EventSystem.current.SetSelectedGameObject(currentScreenManager.goFirstSelectedUI);
			}
		}
		else
		{
			if (goLastSelectedObject != EventSystem.current.currentSelectedGameObject)
			{
				if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().navigation.mode != Navigation.Mode.None)
				{
					goLastSelectedObject = EventSystem.current.currentSelectedGameObject;
					PlayHighlightSound();
				}
			}
		}
	}

	#region Screen Management

	// Changes the Menu Screen to panel 
	public virtual void GoToScreen(string panelID)
	{
		if (!dMenuScreens.ContainsKey(panelID))
		{
			return;
		}

		if (bStorePreviousSelections)
		{
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				prevSelectedScreenUINames.Push("");
			}
			else
			{
				prevSelectedScreenUINames.Push(EventSystem.current.currentSelectedGameObject.name);
			}
		}

		UIMenuScreenManager oldScreenManager = null;
		GameObject oldScreen = goCurrentPanelObj;

		if (sCurrentPanel != null)
		{
			prevScreens.Push(sCurrentPanel);
			oldScreenManager = currentScreenManager;

			sCurrentPanel = null;
			currentScreenManager = null;
			goCurrentPanelObj = null;

			oldScreenManager.CloseScreen(() => { OpenNewScreen(panelID, null); });
		}
		else
		{
			OpenNewScreen(panelID, null);
		}
	}

	// Opens the new screen
	private void OpenNewScreen(string newPanelID, string lastSelection)
	{
		GameObject newScreen = Instantiate(dMenuScreens[newPanelID].goScreenPanel, tMenuParent);
		newScreen.SetActive(false);
		sCurrentPanel = newPanelID;
		goCurrentPanelObj = newScreen;

		currentScreenManager = newScreen.GetComponent<UIMenuScreenManager>();

		if (currentScreenManager != null)
		{
			currentScreenManager.SetMenuManager(this);
			currentScreenManager.gameObject.SetActive(true);

			currentScreenManager.OpenScreen(lastSelection);
		}
	}

    // Return to a previous screen if one is available
    public virtual void ReturnToPreviousScreen()
    {
        if (prevScreens.Count == 0)
        {
            return;
        }

		GameObject oldScreen = goCurrentPanelObj;
		UIMenuScreenManager oldScreenManager = currentScreenManager;

		string panelID = prevScreens.Pop();
		string lastSelection = "";

		if (bStorePreviousSelections)
		{
			if (prevSelectedScreenUINames.Count > 0)
			{
				lastSelection = prevSelectedScreenUINames.Pop();
			}
		}

		sCurrentPanel = null;
		currentScreenManager = null;
		goCurrentPanelObj = null;

		oldScreenManager.CloseScreen(() => { OpenNewScreen(panelID, lastSelection); });
	}

	#endregion

	#region Window Management

	// Opens a new window
	public void OpenWindow(string name)
	{
		if (!dMenuWindows.ContainsKey(name))
		{
			return;
		}

		GameObject newWindow = Instantiate(dMenuWindows[name].goWindowPanel, tMenuParent);
		currentWindowManager = newWindow.GetComponent<UIMenuWindowManager>();
		prevSelectedWindowUIObjects.Push(EventSystem.current.currentSelectedGameObject);

		if (currentWindowManager != null)
		{
			currentWindows.Push(currentWindowManager);
			currentWindowManager.SetMenuManager(this);
			EventSystem.current.SetSelectedGameObject(currentWindowManager.goFirstSelectedUI);
		}
	}

	// Closes the most recent window if one is available
	public void CloseTopWindow()
	{
		if (currentWindows.Count == 0)
		{
			return;
		}

		UIMenuWindowManager window = currentWindows.Pop();
		GameObject lastSelected = prevSelectedWindowUIObjects.Pop();

		EventSystem.current.SetSelectedGameObject(lastSelected);
		PlayCancelSound();
		Destroy(window.gameObject);
	}

	#endregion

	#region Audio

	// Play the Highlight Audio
	public void PlayHighlightSound()
	{
		highlightAudio.Play();
	}

	// Play the Confirm Audio
	public void PlayConfirmSound()
	{
		confirmAudio.Play();
	}

	// Play the Cancel Audio
	public void PlayCancelSound()
	{
		cancelAudio.Play();
	}

	#endregion
}

[System.Serializable]
public class UIMenuScreen
{
	// Panel GameObject
	public GameObject goScreenPanel;
}

[System.Serializable]
public class UIMenuWindow
{
	// Panel GameObject
	public GameObject goWindowPanel;
}

[System.Serializable]
public class UIMenuScreenDictionary : SerializableDictionary<string, UIMenuScreen> { }

[System.Serializable]
public class UIMenuWindowDictionary : SerializableDictionary<string, UIMenuWindow> { }
