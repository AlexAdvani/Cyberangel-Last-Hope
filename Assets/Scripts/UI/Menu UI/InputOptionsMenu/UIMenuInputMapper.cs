using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Rewired;
using TMPro;

public class UIMenuInputMapper : MonoBehaviour
{
	// Player Input
	Player playerInput;

	// Primary Input Map for Current Control Type 
	ControllerMap controlMap;
	// Secondary Input Map for Current Control Type
	ControllerMap altControlMap;
	// Primary Action Element Maps
	ActionElementMap[] aElementMaps;
	// Secondary Action Element Maps
	ActionElementMap[] aAltElementMaps;

	// Input Image Data
	public InputElementToImageData buttonImageData;

	// Controller Type - Keyboard, Mouse, Joystick, Custom
	public ControllerType controlType;

	// Input Assignment Panel
	public GameObject goAssignPanel;
	// Assignment Panel Buttons Group Object
	public GameObject goPanelButtons;
	// Assignment Panel Text
	public TextMeshProUGUI assignPanelText;
	// Assignment Panel Countdown Text
	public TextMeshProUGUI panelCountdownText;

	// Assign Panel Cancel Button
	public GameObject goAssignPanelCancelButton;
	// Last Selection Before the Assign Panel is Opened
	GameObject goPreAssignPanelSelection;

	// Countdown Timer 
	int iCountdownTimer = 5;

	// Is Assignment Window Open flag
	public static bool bAssignWindowOpen = false;
	// Is Polling flag
	public static bool bPolling = false;
	// Is Affecting Alt Map flag
	bool bAltMap = false;
	// Is Active flag
	[HideInInspector]
	public bool bActive = false;
	// Controller Map Unassigned flag
	bool bUnassigned = false;
	// Current Active Mapper Instance
	public static UIMenuInputMapper currentActiveMapper;

	// Array of UI Input Fields
	public UIInputMapField[] aInputFields;
	// Input Field that is currently being assigned to
	UIInputMapField currentInputField;

	// Current Gamepad Type
	eGamepadButtonType currentGamepadType;

	// Initialization
	void Awake()
	{
		InitializeInputSettings();
		InitializeFields();

		if (controlType == ControllerType.Joystick)
		{
			ReInput.ControllerConnectedEvent += ControllerConnectionChangedEvent;
			ReInput.ControllerDisconnectedEvent += ControllerConnectionChangedEvent;
		}
	}

	// Update
	void Update()
	{
		// If polling, then poll for input
		if (bPolling)
		{
			PollForInput();
		}
	}

	// If controller is connected or disconnected, reinitialize the input fields
	private void ControllerConnectionChangedEvent(ControllerStatusChangedEventArgs args)
	{
		InitializeInputSettings();
		InitializeFields();
	}

	#region Initialization

	// Initialize Input
	private void InitializeInputSettings()
	{
		playerInput = ReInput.players.GetPlayer(0);
		int controllerID = 0;

		// Control Type Joystick
		if (controlType == ControllerType.Joystick)
		{
			controllerID = ControllerStatusManager.iGamepadID;

			// If there are no controllers connected or no controller selected then set all gamepad fields to 'None'
			if (playerInput.controllers.joystickCount == 0 || controllerID == -1)
			{
				bUnassigned = true;

				for (int i = 0; i < aInputFields.Length; i++)
				{
					UIInputMapField inputField = aInputFields[i];

					inputField.buttonText.gameObject.SetActive(true);
					inputField.altButtonText.gameObject.SetActive(true);

					if (inputField.buttonImage != null)
					{
						inputField.buttonImage.gameObject.SetActive(false);
					}

					if (inputField.altButtonImage != null)
					{
						inputField.altButtonImage.gameObject.SetActive(false);
					}

					inputField.buttonText.text = "None";
					inputField.altButtonText.text = "None";
				}

				return;
			}
			else 
			{
				bUnassigned = false;
			}
		}

		// Get control maps and element maps for control type
		controlMap = playerInput.controllers.maps.GetFirstMapInCategory(controlType, controllerID, 3);
		altControlMap = playerInput.controllers.maps.GetFirstMapInCategory(controlType, controllerID, 4);
		aElementMaps = controlMap.AllMaps.ToArray();
		aAltElementMaps = altControlMap.AllMaps.ToArray();
	}

	// Initialize Input Button Text
	public void InitializeFields()
	{
		// If Gamepad Unassigned
		if (bUnassigned)
		{
			return;
		}

		// Loop through each field and assign each field's text 
		for (int i = 0; i < aInputFields.Length; i++)
		{
			UIInputMapField inputField = aInputFields[i];
			ActionElementMap map = GetActionMap(inputField.iActionID, inputField.axis, false);
			ActionElementMap altMap = GetActionMap(inputField.iActionID, inputField.axis, true);

			if (controlType == ControllerType.Joystick)
			{
				if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic &&
					ControllerStatusManager.nativeGamepadType != eGamepadButtonType.Generic)
				{
					if (map == null || map.elementIdentifierName == string.Empty)
					{
						inputField.buttonText.gameObject.SetActive(true);
						inputField.buttonImage.gameObject.SetActive(false);
						inputField.buttonText.text = "None";
					}
					else
					{
						inputField.buttonText.gameObject.SetActive(false);
						inputField.buttonImage.gameObject.SetActive(true);

						inputField.buttonImage.sprite = buttonImageData.GetImage(controlType, map.elementType, map.elementIdentifierId, map.axisRange, map.axisContribution);
					}

					if (inputField.buttonImage.sprite == null)
					{
						inputField.buttonText.gameObject.SetActive(true);
						inputField.buttonImage.gameObject.SetActive(false);
						inputField.buttonText.text = "None";
					}

					if (altMap == null || altMap.elementIdentifierName == string.Empty)
					{
						inputField.altButtonText.gameObject.SetActive(true);
						inputField.altButtonImage.gameObject.SetActive(false);
						inputField.altButtonText.text = "None";
					}
					else if (altMap.elementIdentifierId == map.elementIdentifierId)
					{
						inputField.altButtonText.gameObject.SetActive(true);
						inputField.altButtonImage.gameObject.SetActive(false);
						inputField.altButtonText.text = "None";
					}
					else
					{
						inputField.altButtonText.gameObject.SetActive(false);
						inputField.altButtonImage.gameObject.SetActive(true);
						inputField.altButtonImage.sprite = buttonImageData.GetImage(controlType, altMap.elementType, altMap.elementIdentifierId, altMap.axisRange, map.axisContribution);
					}

					if (inputField.altButtonImage.sprite == null)
					{
						inputField.altButtonText.gameObject.SetActive(true);
						inputField.altButtonImage.gameObject.SetActive(false);
						inputField.altButtonText.text = "None";
					}

					continue;
				}
			}

			inputField.buttonText.gameObject.SetActive(true);
			inputField.altButtonText.gameObject.SetActive(true);

			if (inputField.buttonImage != null)
			{
				inputField.buttonImage.gameObject.SetActive(false);
			}

			if (inputField.altButtonImage != null)
			{
				inputField.altButtonImage.gameObject.SetActive(false);
			}

			if (map == null || map.elementIdentifierName == string.Empty)
			{
				inputField.buttonText.text = "None";
			}
			else
			{
				inputField.buttonText.text = map.elementIdentifierName;
			}

			if (altMap == null || altMap.elementIdentifierName == string.Empty)
			{
				inputField.altButtonText.text = "None";
			}
			else
			{
				if (controlType == ControllerType.Keyboard)
				{
					if (altMap.keyCode == map.keyCode)
					{
						inputField.altButtonText.text = "None";
						continue;
					}
				}
				else
				{
					if (altMap.elementIdentifierId == map.elementIdentifierId)
					{
						inputField.altButtonText.text = "None";
						continue;
					}
				}

				inputField.altButtonText.text = altMap.elementIdentifierName;
			}
		}
	}

	#endregion

	#region Input Assignment

	// Opens Assign Panel for Primary Input
	public void OpenWindowForPrimary(int fieldID)
	{
		bAltMap = false;
		OpenAssignmentWindow(fieldID);
	}

	// Opens Assign Panel for Secondary Input
	public void OpenWindowForSecondary(int fieldID)
	{
		bAltMap = true;
		OpenAssignmentWindow(fieldID);
	}

	// Opens the Assignment Window
	private void OpenAssignmentWindow(int fieldID)
	{
		if (bActive || bUnassigned)
		{
			return;
		}

		goPreAssignPanelSelection = EventSystem.current.currentSelectedGameObject;
		goAssignPanel.SetActive(true);

		bAssignWindowOpen = true;
		bActive = true;
		UIMenuManager.bOverrideCancelFunction = true;
		currentActiveMapper = this;

		// Get the current input field and check if it needs to be replaced
		currentInputField = aInputFields[fieldID];
		CheckReplaceInput();
	}

	// Closes the Assignment Window
	public void CloseAssignmentWindow()
	{
		goPanelButtons.SetActive(false);
		panelCountdownText.gameObject.SetActive(false);
		EventSystem.current.SetSelectedGameObject(goPreAssignPanelSelection);

		iCountdownTimer = 5;
		panelCountdownText.text = iCountdownTimer.ToString();
		StopAllCoroutines();
		UIMenuManager.bOverrideCancelFunction = false;

		bAltMap = false;
		bAssignWindowOpen = false;
		bActive = false;
		currentActiveMapper = null;

		goAssignPanel.SetActive(false);
	}

	// Checks the current action map for an existing input
	private void CheckReplaceInput()
	{
		ActionElementMap map = GetActionMap(currentInputField.iActionID, currentInputField.axis, bAltMap);

		// If the map is empty, start polling for input
		if (map.elementIdentifierName == "None" || map.elementIdentifierName == string.Empty ||
			(!bAltMap && currentInputField.buttonText.gameObject.activeSelf && currentInputField.buttonText.text == "None") ||
			(bAltMap && currentInputField.altButtonText.gameObject.activeSelf && currentInputField.altButtonText.text == "None"))
		{
			StartCoroutine(EnablePolling());
			assignPanelText.text = "Please assign a new input for " + currentInputField.sInputName + ".";
			panelCountdownText.gameObject.SetActive(true);
		}
		else // Otherwise, ask player is they want to replace or remove the input or cancel the operation
		{
			assignPanelText.text = "There is already an input assigned to " + currentInputField.sInputName + ".";
			goPanelButtons.SetActive(true);
			EventSystem.current.SetSelectedGameObject(goAssignPanelCancelButton);
		}
	}

	// Starts Polling
	private IEnumerator EnablePolling()
	{
		yield return new WaitForSecondsRealtime(0.1f);

		bPolling = true;
		StartCoroutine(UpdateTimer());

		yield return null;
	}

	// Polls for Input Based on Control Type
	private void PollForInput()
	{
		switch (controlType)
		{
			case ControllerType.Keyboard:
				KeyboardPoll();
				break;

			case ControllerType.Mouse:
				MousePoll();
				break;

			case ControllerType.Joystick:
				GamepadPoll();
				break;
		}
	}

	// Polls for Keyboard Input
	private void KeyboardPoll()
	{
		ControllerPollingInfo pollingInfo = new ControllerPollingInfo();

		foreach (ControllerPollingInfo info in ReInput.controllers.Keyboard.PollForAllKeys())
		{
			KeyCode key = info.keyboardKey;

			// If Key is AltGr, Command, Windows or Apple, skip it as these are keys are not supported
			if (key == KeyCode.AltGr)
			{
				continue;
			}

			if (key == KeyCode.LeftCommand || key == KeyCode.LeftWindows || key == KeyCode.LeftApple)
			{
				continue;
			}

			if (key == KeyCode.RightCommand || key == KeyCode.RightWindows || key == KeyCode.RightApple)
			{
				continue;
			}

			if (key != KeyCode.None)
			{
				// If the key is not pressed this frame
				if (!ReInput.controllers.Keyboard.GetKeyDown(key))
				{
					return;
				}

				pollingInfo = info;
				break;
			}
		}

		// If unsuccessful
		if (!pollingInfo.success)
		{
			return;
		}

		// Stop polling
		bPolling = false;

		// Current Action Map
		ActionElementMap map = GetActionMap(currentInputField.iActionID, currentInputField.axis, bAltMap);

		// Replace Element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(map.id, map.actionId, map.axisContribution, pollingInfo.keyboardKey, ModifierKeyFlags.None);
			currentInputField.altButtonText.text = pollingInfo.elementIdentifierName;
			aAltElementMaps = altControlMap.AllMaps.ToArray();
		}
		else
		{
			controlMap.ReplaceElementMap(map.id, map.actionId, map.axisContribution, pollingInfo.keyboardKey, ModifierKeyFlags.None);
			currentInputField.buttonText.text = pollingInfo.elementIdentifierName;
			aElementMaps = controlMap.AllMaps.ToArray();
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Polls for Mouse Input
	private void MousePoll()
	{
		ControllerPollingInfo pollingInfo = new ControllerPollingInfo();

		// Loop through each polling info to find which one is in use
		foreach (ControllerPollingInfo info in ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, 0))
		{
			if (info.elementType == ControllerElementType.Axis)
			{
				// If Mouse Move X or Y, then continue
				if (info.elementIndex == 0 || info.elementIndex == 1)
				{
					continue;
				}
			}

			pollingInfo = info;
		}

		// If unsuccessful
		if (!pollingInfo.success)
		{
			return;
		}

		// Stop Polling
		bPolling = false;

		// Current Action Map
		ActionElementMap map = GetActionMap(currentInputField.iActionID, currentInputField.axis, bAltMap);
		// Assignment Data 
		ElementAssignment assignment = new ElementAssignment(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, currentInputField.axisRange,
			pollingInfo.keyboardKey, ModifierKeyFlags.None, currentInputField.iActionID, map.axisContribution, false, map.id);

		// Replace element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);
			currentInputField.altButtonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
			aAltElementMaps = altControlMap.AllMaps.ToArray();
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);
			currentInputField.buttonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
			aElementMaps = controlMap.AllMaps.ToArray();
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Polls for Gamepad Input
	private void GamepadPoll()
	{
		// If no controllers connected
		if (playerInput.controllers.joystickCount == 0)
		{
			return;
		}

		// Get current polling info in use
		ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, playerInput.controllers.GetLastActiveController().id);

		// If Guide or PS Button
		if (pollingInfo.elementIdentifierName == "Guide" || pollingInfo.elementIdentifierName == "PS Button")
		{
			return;
		}

		// If unsuccessful
		if (!pollingInfo.success)
		{
			return;
		}

		bPolling = false;

		// Current Action Map
		ActionElementMap map = GetActionMap(currentInputField.iActionID, currentInputField.axis, bAltMap);
		// Assignment Data
		ElementAssignment assignment = new ElementAssignment(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, currentInputField.axisRange,
			pollingInfo.keyboardKey, ModifierKeyFlags.None, currentInputField.iActionID, map.axisContribution, false, map.id);

		// Replace element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);

			if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic)
			{
				currentInputField.altButtonText.gameObject.SetActive(false);
				currentInputField.altButtonImage.gameObject.SetActive(true);
				currentInputField.altButtonImage.sprite = buttonImageData.GetImage(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange, pollingInfo.axisPole);
			}
			else
			{
				currentInputField.altButtonText.gameObject.SetActive(true);
				currentInputField.altButtonImage.gameObject.SetActive(false);
				currentInputField.altButtonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
			}

			aAltElementMaps = altControlMap.AllMaps.ToArray();
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);

			if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic)
			{
				currentInputField.buttonText.gameObject.SetActive(false);
				currentInputField.buttonImage.gameObject.SetActive(true);
				currentInputField.buttonImage.sprite = buttonImageData.GetImage(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange, pollingInfo.axisPole);
			}
			else
			{
				currentInputField.buttonText.gameObject.SetActive(true);
				currentInputField.buttonImage.gameObject.SetActive(false);
				currentInputField.buttonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
			}

			aElementMaps = controlMap.AllMaps.ToArray();
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Removes an input from an action map
	public void RemoveInput()
	{
		// Current Action Map
		ActionElementMap map = GetActionMap(currentInputField.iActionID, currentInputField.axis, bAltMap);
		// Assignment Data
		ElementAssignment assignment = new ElementAssignment(controlType, ControllerElementType.Button, -1, currentInputField.axisRange,
			KeyCode.None, ModifierKeyFlags.None, currentInputField.iActionID, map.axisContribution, false, map.id);

		// Remove element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);

			currentInputField.altButtonText.gameObject.SetActive(true);

			if (controlType == ControllerType.Joystick)
			{
				currentInputField.altButtonImage.gameObject.SetActive(false);
				currentInputField.altButtonImage.sprite = null;
			}
			currentInputField.altButtonText.text = "None";

			aAltElementMaps = altControlMap.AllMaps.ToArray();
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);

			currentInputField.buttonText.gameObject.SetActive(true);

			if (controlType == ControllerType.Joystick)
			{
				currentInputField.buttonImage.gameObject.SetActive(false);
				currentInputField.buttonImage.sprite = null;
			}

			currentInputField.buttonText.text = "None";

			aElementMaps = controlMap.AllMaps.ToArray();
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Sets up an input to replaced and starts polling
	public void ReplaceInput()
	{
		goPanelButtons.SetActive(false);
		assignPanelText.text = "Please assign a new input for " + currentInputField.sInputName + ".";
		panelCountdownText.gameObject.SetActive(true);

		StartCoroutine(EnablePolling());
	}

	// Sets all input for the control type to defaults
	public void ResetToDefaults()
	{
		if (controlType == ControllerType.Joystick && playerInput.controllers.joystickCount == 0)
		{
			return;
		}

		playerInput.controllers.maps.LoadDefaultMaps(controlType);
		InitializeInputSettings();
		InitializeFields();

		SettingsManager.bOptionChanged = true;
	}

	// Gets the axis direction and returns a suffix for that direction
	private string SGetAxisDir(ControllerPollingInfo info)
	{
		if (info.elementType == ControllerElementType.Axis)
		{
			if (currentInputField.axisRange != AxisRange.Full)
			{
				if (info.axisPole == Pole.Positive)
				{
					return " +";
				}
				else if (info.axisPole == Pole.Negative)
				{
					return " -";
				}
			}
		}

		return "";
	}

	// Checks the element array for a map based on Action ID
	private ActionElementMap GetActionMap(int actionID, Pole axis, bool alt)
	{
		ActionElementMap[] elementArray = alt ? aAltElementMaps : aElementMaps;

		for (int i = 0; i < elementArray.Length; i++)
		{
			if (elementArray[i].actionId == actionID)
			{
				if (elementArray[i].axisContribution == axis)
				{
					return elementArray[i];
				}
			}
		}

		return null;
	}

	#endregion

	// Updates the Countdown Timer
	private IEnumerator UpdateTimer()
	{
		while (iCountdownTimer >= 0)
		{
			yield return new WaitForSecondsRealtime(1);

			iCountdownTimer -= 1;
			panelCountdownText.text = iCountdownTimer.ToString();
		}

		bPolling = false;
	
		CloseAssignmentWindow();
		yield return null;
	}

	// Assigns a gamepad to Player 1 and reinitializes the input settings and fields
	public void SetNewGamepad(int gamepad)
	{
		ControllerStatusManager.iGamepadID = gamepad - 1;

		InitializeInputSettings();
		InitializeFields();
	}
}

// Input Map Field
[System.Serializable]
public class UIInputMapField
{
	// Input Name
	public string sInputName;
	// Action ID
	public int iActionID;
	// Button Text
	public TextMeshProUGUI buttonText;
	// Alt Button Text
	public TextMeshProUGUI altButtonText;
	// Button Image
	public Image buttonImage;
	// Alt Button Image
	public Image altButtonImage;
	// Axis Direction
	public Pole axis;
	// Axis Range
	public AxisRange axisRange;
}