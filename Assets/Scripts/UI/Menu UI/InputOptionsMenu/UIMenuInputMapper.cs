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
    // Tab Manager
    public UITabManager tabManager;
    // Tab Number
    public int iTabNo;

    // Input Actions Data
    public InputMenuActionsData aInputActionsData;
	// Primary Input Map for Current Control Type 
	ControllerMap controlMap;
	// Secondary Input Map for Current Control Type
	ControllerMap altControlMap;
    // Element Map Array
    ActionElementMap[] aElementMaps;
    // Alt Element Map Array
    ActionElementMap[] aAltElementMaps;

	// Input Image Data
	public InputElementToImageData buttonImageData;
    // Input Row Prefab 
    public GameObject goInputRowPrefab;
    // Input Row Parent Transform
    public RectTransform rtInputRowParent;
    // Selectable UI underneath the input fields
    public Selectable downSelectable;

	// Controller Type - Keyboard, Mouse, Joystick, Custom
	public ControllerType controlType;

    // UI Input Rows
    UIInputRow[] aUIInputRows;
    // Current UI Input Row
    UIInputRow currentUIInputRow;

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

	// Current Gamepad Type
	eGamepadButtonType currentGamepadType;

    // Initialization
    void Awake()
    {
        aUIInputRows = null;

        if (controlType == ControllerType.Joystick)
        { 
            ReInput.ControllerConnectedEvent += ControllerConnectionChangedEvent;
            ReInput.ControllerDisconnectedEvent += ControllerConnectionChangedEvent;

            if (ReInput.controllers.joystickCount == 0)
            {
                InitializeInputSettings();
            }
        }
        else 
        {
            InitializeInputSettings();
        }

        tabManager.agoTabs[iTabNo].onTabActivate += SetInitialUISelection;
        
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

    // On Destroy
    void OnDestroy()
    {
        if (controlType == ControllerType.Joystick)
        {
            ReInput.ControllerConnectedEvent -= ControllerConnectionChangedEvent;
            ReInput.ControllerDisconnectedEvent -= ControllerConnectionChangedEvent;
        }
    }

    // If controller is connected or disconnected, reinitialize the input fields
    private void ControllerConnectionChangedEvent(ControllerStatusChangedEventArgs args)
	{
        InitializeInputSettings();
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
            }
            else
            {
                bUnassigned = false;
            }
        }
        else
        {
            bUnassigned = false;
        }

        if (!bUnassigned)
        {
            controlMap = playerInput.controllers.maps.GetFirstMapInCategory(controlType, controllerID, 3);
            altControlMap = playerInput.controllers.maps.GetFirstMapInCategory(controlType, controllerID, 4);

            aElementMaps = controlMap.AllMaps.ToArray();
            aAltElementMaps = altControlMap.AllMaps.ToArray();
        }

        if (aUIInputRows == null)
        {
            InitializeFields();
        }
        else
        {
            UpdateFields();
        }
    }

    // Initialize Input Fields
    public void InitializeFields()
    {
        aUIInputRows = new UIInputRow[aInputActionsData.aInputActions.Length];

        // Loop through each field and assign each field's text 
        for (int i = 0; i < aUIInputRows.Length; i++)
        {
            InputAction inputData = aInputActionsData.aInputActions[i];

            ActionElementMap map = null;
            ActionElementMap altMap = null;
            int mapID = -1;
            int altMapID = -1;

            if (!bUnassigned)
            {
                map = GetActionMap(inputData, false, out mapID);
                altMap = GetActionMap(inputData, true, out altMapID);
            }

            int index = i;

            UIInputRow row = Instantiate(goInputRowPrefab, rtInputRowParent).GetComponent<UIInputRow>();
            row.InitializeRow(inputData.sName, map, altMap, mapID, altMapID, controlType, buttonImageData, bUnassigned);
            row.primaryButton.onClick.AddListener(() => { OpenWindowForPrimary(index); });
            row.secondaryButton.onClick.AddListener(() => { OpenWindowForSecondary(index); });

            aUIInputRows[i] = row;
        }

        // Loop through and add navigation
        for (int j = 0; j < aUIInputRows.Length; j++)
        {
            Navigation primNav = new Navigation();
            Navigation secNav = new Navigation();

            primNav.mode = Navigation.Mode.Explicit;
            secNav.mode = Navigation.Mode.Explicit;

            if (j != 0)
            {
                primNav.selectOnUp = aUIInputRows[j - 1].primaryButton;
                secNav.selectOnUp = aUIInputRows[j - 1].secondaryButton;
            }

            if (j != aUIInputRows.Length - 1)
            {
                primNav.selectOnDown = aUIInputRows[j + 1].primaryButton;
                secNav.selectOnDown = aUIInputRows[j + 1].secondaryButton;
            }
            else
            {
                primNav.selectOnDown = downSelectable;
                secNav.selectOnDown = downSelectable;
            }

            primNav.selectOnRight = aUIInputRows[j].secondaryButton;
            secNav.selectOnLeft = aUIInputRows[j].primaryButton;

            aUIInputRows[j].primaryButton.navigation = primNav;
            aUIInputRows[j].secondaryButton.navigation = secNav;
        }

        Navigation nav = downSelectable.navigation;
        nav.selectOnUp = aUIInputRows.Last().primaryButton;
        downSelectable.navigation = nav;
    }

    // Update Input Fields
    public void UpdateFields()
    {
        if (aUIInputRows == null)
        {
            return;
        }

        for (int i = 0; i < aUIInputRows.Length; i++)
        {
            UIInputRow row = aUIInputRows[i];
            row.UpdateRow(aElementMaps[row.PrimaryActionID], aAltElementMaps[row.SecondaryActionID], bUnassigned);
        }
    }

    // Set the UI Selection to the first input field button
    public void SetInitialUISelection()
    {
        if (aUIInputRows != null && !bUnassigned)
        {
            EventSystem.current.SetSelectedGameObject(aUIInputRows[0].primaryButton.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(downSelectable.gameObject);
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
		currentUIInputRow = aUIInputRows[fieldID];
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

        aElementMaps = controlMap.AllMaps.ToArray();
        aAltElementMaps = altControlMap.AllMaps.ToArray();

        bAltMap = false;
		bAssignWindowOpen = false;
		bActive = false;
		currentActiveMapper = null;

		goAssignPanel.SetActive(false);
	}

	// Checks the current action map for an existing input
	private void CheckReplaceInput()
	{
        // Current Map
        ActionElementMap map = bAltMap ? aAltElementMaps[currentUIInputRow.PrimaryActionID] :
            aElementMaps[currentUIInputRow.SecondaryActionID]; 

        // If the map is empty, start polling for input
        if (map.elementIdentifierName == "None" || map.elementIdentifierName == string.Empty ||
            (!bAltMap && currentUIInputRow.primaryButtonText.gameObject.activeSelf && currentUIInputRow.primaryButtonText.text == "None") ||
            (bAltMap && currentUIInputRow.secondaryButtonText.gameObject.activeSelf && currentUIInputRow.secondaryButtonText.text == "None"))
        {
			StartCoroutine(EnablePolling());
			assignPanelText.text = "Please assign a new input for " + currentUIInputRow.inputNameText.text + ".";
			panelCountdownText.gameObject.SetActive(true);
		}
		else // Otherwise, ask player is they want to replace or remove the input or cancel the operation
		{
			assignPanelText.text = "There is already an input assigned to " + currentUIInputRow.inputNameText.text + ".";
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
        ActionElementMap map = bAltMap ? aAltElementMaps[currentUIInputRow.SecondaryActionID] :
            aElementMaps[currentUIInputRow.PrimaryActionID];

		// Replace Element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(map.id, map.actionId, map.axisContribution, pollingInfo.keyboardKey, ModifierKeyFlags.None);
			currentUIInputRow.secondaryButtonText.text = pollingInfo.elementIdentifierName;
		}
		else
		{
			controlMap.ReplaceElementMap(map.id, map.actionId, map.axisContribution, pollingInfo.keyboardKey, ModifierKeyFlags.None);
			currentUIInputRow.primaryButtonText.text = pollingInfo.elementIdentifierName;
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
        ActionElementMap map = bAltMap ? aAltElementMaps[currentUIInputRow.SecondaryActionID] :
            aElementMaps[currentUIInputRow.PrimaryActionID];
        
        // Assignment Data 
        ElementAssignment assignment = new ElementAssignment(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange,
			pollingInfo.keyboardKey, ModifierKeyFlags.None, map.actionId, map.axisContribution, false, map.id);

        string elementName = pollingInfo.elementIdentifierName;

        if (controlType == ControllerType.Mouse)
        {
            if (elementName == "Left Mouse Button")
            {
                elementName = "Left Click";
            }
            else if (elementName == "Right Mouse Button")
            {
                elementName = "Right Click";
            }
        }

        // Replace element based on which map is being assigned to
        if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);
			currentUIInputRow.secondaryButtonText.text = elementName + SGetAxisDir(pollingInfo);
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);
			currentUIInputRow.primaryButtonText.text = elementName + SGetAxisDir(pollingInfo);
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
		ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, 
            playerInput.controllers.GetLastActiveController().id);

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
        ActionElementMap map = bAltMap ? aAltElementMaps[currentUIInputRow.SecondaryActionID] :
            aElementMaps[currentUIInputRow.PrimaryActionID];
        // Assignment Data
        ElementAssignment assignment = new ElementAssignment(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange,
			pollingInfo.keyboardKey, ModifierKeyFlags.None, map.actionId, map.axisContribution, false, map.id);

		// Replace element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);

			if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic)
			{
                currentUIInputRow.secondaryButtonText.gameObject.SetActive(false);
                currentUIInputRow.secondaryButtonIcon.gameObject.SetActive(true);
                currentUIInputRow.secondaryButtonIcon.sprite = 
                    buttonImageData.GetImage(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange, pollingInfo.axisPole);
            }
			else
			{
                currentUIInputRow.secondaryButtonText.gameObject.SetActive(true);
                currentUIInputRow.secondaryButtonIcon.gameObject.SetActive(false);
                currentUIInputRow.secondaryButtonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
            }
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);

			if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic)
			{
                currentUIInputRow.primaryButtonText.gameObject.SetActive(false);
                currentUIInputRow.primaryButtonIcon.gameObject.SetActive(true);
                currentUIInputRow.primaryButtonIcon.sprite = 
                    buttonImageData.GetImage(controlType, pollingInfo.elementType, pollingInfo.elementIdentifierId, map.axisRange, pollingInfo.axisPole);
            }
			else
			{
                currentUIInputRow.primaryButtonText.gameObject.SetActive(true);
                currentUIInputRow.primaryButtonIcon.gameObject.SetActive(false);
                currentUIInputRow.primaryButtonText.text = pollingInfo.elementIdentifierName + SGetAxisDir(pollingInfo);
            }
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Removes an input from an action map
	public void RemoveInput()
	{
        // Current Action Map
        ActionElementMap map = bAltMap ? aAltElementMaps[currentUIInputRow.PrimaryActionID] :
            aElementMaps[currentUIInputRow.SecondaryActionID];
        // Assignment Data
        ElementAssignment assignment = new ElementAssignment(controlType, ControllerElementType.Button, -1, map.axisRange,
			KeyCode.None, ModifierKeyFlags.None, map.actionId, map.axisContribution, false, map.id);

		// Remove element based on which map is being assigned to
		if (bAltMap)
		{
			altControlMap.ReplaceElementMap(assignment);

			currentUIInputRow.secondaryButtonText.gameObject.SetActive(true);

			if (controlType == ControllerType.Joystick)
			{
				currentUIInputRow.secondaryButtonIcon.gameObject.SetActive(false);
				currentUIInputRow.secondaryButtonIcon.sprite = null;
			}

			currentUIInputRow.secondaryButtonText.text = "None";
		}
		else
		{
			controlMap.ReplaceElementMap(assignment);

			currentUIInputRow.primaryButtonText.gameObject.SetActive(true);

			if (controlType == ControllerType.Joystick)
			{
				currentUIInputRow.primaryButtonIcon.gameObject.SetActive(false);
				currentUIInputRow.primaryButtonIcon.sprite = null;
			}

			currentUIInputRow.primaryButtonText.text = "None";
		}

		SettingsManager.bOptionChanged = true;
		CloseAssignmentWindow();
	}

	// Sets up an input to replaced and starts polling
	public void ReplaceInput()
	{
		goPanelButtons.SetActive(false);
		assignPanelText.text = "Please assign a new input for " + currentUIInputRow.inputNameText + ".";
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

		SettingsManager.bOptionChanged = true;
	}

	// Gets the axis direction and returns a suffix for that direction
	private string SGetAxisDir(ControllerPollingInfo info)
	{
		if (info.elementType == ControllerElementType.Axis)
		{
			if (aElementMaps[currentUIInputRow.PrimaryActionID].axisRange != AxisRange.Full)
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

    // Get Action Map from ActionID
    private ActionElementMap GetActionMap(int actionID, Pole axis, bool alt, out int id)
    {
        ActionElementMap[] elementArray = alt ? aAltElementMaps : aElementMaps;

        for (int i = 0; i < elementArray.Length; i++)
        {
            if (elementArray[i].actionId == actionID)
            {
                if (elementArray[i].axisContribution == axis)
                {
                    id = i;

                    return elementArray[i];
                }
            }
        }

        id = -1;

        return null; 
    }

    // Get Action Map from InputAction (With Map ID)
    private ActionElementMap GetActionMap(InputAction inputData, bool alt, out int id)
    {
        ActionElementMap[] elementArray = alt ? aAltElementMaps : aElementMaps;

        for (int i = 0; i < elementArray.Length; i++)
        {
            if (elementArray[i].actionId == inputData.iActionID)
            {
                if (elementArray[i].axisContribution == inputData.axisDirection)
                {
                    id = i;

                    return elementArray[i];
                }
            }
        }

        id = -1;

        return null;
    }

    // Assigns a gamepad to Player 1 and reinitializes the input settings and fields
    public void SetNewGamepad(int gamepad)
	{
		ControllerStatusManager.iGamepadNo = gamepad - 1;

		InitializeInputSettings();
	}
}