using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using BeautifulTransitions.Scripts.Transitions.Components;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;

public class UIMenuScreenManager : MonoBehaviour
{
	// UI Menu Manager
	protected UIMenuManager menuManager;

	// Transition
	public TransitionBase transition;

	// First Sleected UI GameObject
	public GameObject goFirstSelectedUI;

	Selectable[] aSelectableUI;

	// Initialization
	protected virtual void Start ()
	{
	}

	// Sets the Menu Manager
	public void SetMenuManager(UIMenuManager manager)
	{
		menuManager = manager;
	}

	// Sets the initial menu selection for the screen
	private void SetMenuSelection(string lastSelectionName)
	{
		if (lastSelectionName == null)
		{
			EventSystem.current.SetSelectedGameObject(goFirstSelectedUI);
		}
		else
		{
			EventSystem.current.SetSelectedGameObject(GameObject.Find(lastSelectionName));
		}
		menuManager.bScreenTransition = false;
	}

	#region Screen Management

	// Opens the Screen and transitions in
	public void OpenScreen(string lastSelectionName)
	{
        if (transition != null)
        {
			transition.TransitionInConfig.OnTransitionComplete.AddListener((TransitionStep transitionStep) => { SetMenuSelection(lastSelectionName); });
            transition.TransitionIn();
            menuManager.bScreenTransition = true;
        }
        else
        {
            SetMenuSelection(lastSelectionName);
        }
    }

	// Closes the screen and transitions out
	public void CloseScreen(UnityAction openScreen)
	{
        if (transition != null)
        {
            if (openScreen != null)
            {
                transition.TransitionOutConfig.OnTransitionComplete.AddListener((TransitionStep transitionStep) => { openScreen(); });
            }

            transition.TransitionOutConfig.OnTransitionComplete.AddListener((TransitionStep transitionStep) => { DestroyScreen(); });
            transition.TransitionOut();
            menuManager.bScreenTransition = true;
        }
        else
        {
            if (openScreen != null)
            {
                openScreen();
            }

            DestroyScreen();
        }
    }

	// Destroy the screen gameobject
	public void DestroyScreen()
	{
		Destroy(gameObject);
	}

	// Changes the Menu Screen
	public void GoToScreen(string panelID)
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.GoToScreen(panelID);
	}

	// Returns to a previous screen if one is available
	public void ReturnToPreviousScreen()
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.ReturnToPreviousScreen();
	}

	#endregion

	#region Window Management

	// Opens a new window
	public void OpenWindow(string name)
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.OpenWindow(name);
	}

	// Closes the most recent window if one is avaiable
	public void CloseTopWindow()
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.CloseTopWindow();
	}

	#endregion

	// Saves current game settings
	public void SaveSettings()
	{
		SettingsManager.Instance.SaveSettings();
	}

	// Go to a new scene
	public void GoToScene(string sceneName)
	{
		SceneLoadManager.Instance.LoadScene(sceneName);
	}

	#region Audio

	// Plays the highlight sound for mouse over if ui is interactive
	public void MouseHighlightSound()
	{
		PlayHighlightSound();
	}

	// Play the Highlight Audio
	public void PlayHighlightSound()
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.PlayHighlightSound();
	}

	// Play the Confirm Audio
	public void PlayConfirmSound()
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.PlayConfirmSound();
	}

	// Play the Cancel Audio
	public void PlayCancelSound()
	{
		if (menuManager == null)
		{
			return;
		}

		menuManager.PlayCancelSound();
	}

	#endregion
}