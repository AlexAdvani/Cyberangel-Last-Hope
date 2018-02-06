using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuWindowManager : MonoBehaviour
{
	// UI Menu Manager
	UIMenuManager menuManager;

	// First Selected UI GameObject
	public GameObject goFirstSelectedUI;

	// Initialization
	public virtual void Start ()
	{
		EventSystem.current.SetSelectedGameObject(goFirstSelectedUI);
	}

	// Sets the Menu Manager
	public void SetMenuManager(UIMenuManager manager)
	{
		menuManager = manager;
	}
	
	// Opens a new window
	public void OpenWindow(string name)
	{
		menuManager.OpenWindow(name);
	}

	// Closes the most recent window if one is available
	public void CloseTopWindow()
	{
		menuManager.CloseTopWindow();
	}
}
