using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Rewired;
using TMPro;

public class UIInputGamepadCalibrator : MonoBehaviour
{
    // Calibration Menu Panel
    public GameObject goMenuPanel;
    // Axis Test Panel
    public GameObject goAxisPanel;

    // Axis Marker Zone
    public RectTransform rtMarkerZone;
    // Axis Position Marker
    public RectTransform rtAxisPositionMarker;
    // Calibrated Position Marker
    public RectTransform rtCalibratedPositionMarker;
    // Zero Marker
    public RectTransform rtZeroMarker;
    // Deadzone Area
    public RectTransform rtDeadZoneArea;

    // Scroll View 
    public Transform tScrollView;
    // Axis Selection Button Prefab
    public GameObject goAxisSelectButtonPrefab;

    // Deadzone Slider 
    public Slider deadzoneSlider;
    // Zero Slider
    public Slider zeroSlider;
    // Sensitivity Slider
    public Slider sensitivitySlider;
    // Deadzone Value Text
    public TextMeshProUGUI deadzoneValueText;
    // Zero Value Text
    public TextMeshProUGUI zeroValueText;
    // Sensitivity Value Text
    public TextMeshProUGUI sensitivityValueText;
    // Invert Toggle
    public Toggle invertToggle;
    // Calibrate Menu Back Button
    public Button goCalibrateMenuBackButton;

    // Calibrate Menu Button
    public Button calibrateMenuButton;
    // Calibration Controls Parent
    public GameObject goCalibrationControlsParent;
    // No Axis Selected Text
    public GameObject goNoAxisSelectedText;
    // Default Axis Settings Button
    public Button defaultAxisButton;
    // Test Axis Menu Button
    public Button testAxisMenuButton;
    // Current Selected Axis Text
    public TextMeshProUGUI currentSelectedAxisText;

    // Calibration Panel Back Button
    public GameObject goCalibrationPanelBackButton;
    // Axis Test Panel Back Button
    public GameObject goAxisBackButton;

    // Player Input
    Player playerInput;

    // Current Gamepad
    Joystick gamepad;
    // Calibration Map
    CalibrationMap calibrationMap;
    // Current Axis Calibration
    AxisCalibration axisCalibration;
    // Currently Selected Axis
    int iSelectedAxis = -1;

    // List of Axis Buttons
    List<Button> lAxisButtons;

    // Is Active flag
    [HideInInspector]
    public bool bActive = false;
    // Calibration Panel Open flag
    [HideInInspector]
    public bool bCalibrationOpen = false;
    // Test Axis Panel Open flag
    [HideInInspector]
    public bool bAxisOpen = false;

    // Initialization
    void Awake()
    {
        playerInput = ReInput.players.GetPlayer(0);
        lAxisButtons = new List<Button>();

        // Assign to Controller Event
        ReInput.ControllerPreDisconnectEvent += GamepadDisconnectEvent;
    }

    // Update
    void Update()
    {
        HandleCancelInput();

        if (bActive)
        {
            UpdateAxisMarkers();
        }
    }

    // Closes the window if a controller is being disconnected
    private void GamepadDisconnectEvent(ControllerStatusChangedEventArgs args)
    {
        CloseCalibrationWindow();
        CloseAxisWindow();
        gamepad = null;
    }

    // Handle Cancel Input
    private void HandleCancelInput()
    {
        if (playerInput.GetButtonDown("UICancel"))
        {
            if (bAxisOpen)
            {
                CloseAxisWindow();
            }
            else if (bCalibrationOpen)
            {
                GameObject currentObj = EventSystem.current.currentSelectedGameObject;

                if (currentObj == deadzoneSlider.gameObject ||
                    currentObj == zeroSlider.gameObject ||
                    currentObj == sensitivitySlider.gameObject ||
                    currentObj == invertToggle.gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(lAxisButtons[0].gameObject);
                }
                else
                {
                    CloseCalibrationWindow();
                }
            }
        }
    }

	// Opens the Calibration Window
	public void OpenCalibrationWindow()
	{
		// If there are no gamepads connected or no gamepad is selected
		if (ControllerStatusManager.iGamepadNo == -1 || playerInput.controllers.joystickCount == 0)
		{
			return;
		}

		// Setup calibration menu
		iSelectedAxis = -1;
		axisCalibration = null;
		goCalibrationControlsParent.SetActive(false);
		goNoAxisSelectedText.SetActive(true);

		gamepad = ReInput.controllers.GetController<Joystick>(ControllerStatusManager.iGamepadNo);
		calibrationMap = gamepad.calibrationMap;
		SetupAxisButtons();
		ResetUIValues();

		goMenuPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(goCalibrationPanelBackButton);
		testAxisMenuButton.interactable = false;

		UIMenuManager.bOverrideCancelFunction = true;
		bCalibrationOpen = true;
	}

	// Closes the Calibration Window
	public void CloseCalibrationWindow()
	{
		goMenuPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(calibrateMenuButton.gameObject);

		UIMenuManager.bOverrideCancelFunction = false;
		bCalibrationOpen = false;
	}

	// Opens the Axis Text Window
	public void OpenAxisWindow()
	{
		goAxisPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(goAxisBackButton);

		bAxisOpen = true;
		bActive = true;
	}

	// Closes the Axis Text Window
	public void CloseAxisWindow()
	{
		goAxisPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(testAxisMenuButton.gameObject);

		bAxisOpen = false;
		bActive = false;
	}

	// Resets all axis calibrations to default
	public void ResetToDefault()
	{
		// If there is no current gamepad
		if (gamepad == null)
		{
			return;
		}

		gamepad.calibrationMap.Reset();
		SetupUIValues(iSelectedAxis);
		RedrawUI();
	}

	// Detects all axes for the current gamepad and sets up selection buttons for each axis
	private void SetupAxisButtons()
	{
		// Loop through the old buttons and destroy them
		foreach (Button button in lAxisButtons)
		{
			Destroy(button.gameObject);
		} 

		UIMenuScreenManager currentScreenManager = (UIMenuScreenManager)FindObjectOfType(typeof(UIMenuScreenManager));
		lAxisButtons.Clear();

		// Loop through each axis and instantiate a button for each
		for (int i = 0; i < gamepad.axisCount; i++)
		{
			int index = i;
			GameObject buttonObj = (GameObject)Instantiate(goAxisSelectButtonPrefab, tScrollView);
			Button button = buttonObj.GetComponent<Button>();
			TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

			buttonText.text = gamepad.AxisElementIdentifiers[i].name;
			button.onClick.AddListener(() => SelectAxis(index));

			buttonObj.GetComponent<UISoundEventManager>().screenManager = currentScreenManager;

			lAxisButtons.Add(button);
		}

		// If there are more than 1 axis
		if (lAxisButtons.Count > 1)
		{
			// Re-loop through each axis and manually set the navigation explicitly
			for (int i = 0; i < lAxisButtons.Count; i++)
			{
				Navigation buttonNav = lAxisButtons[i].navigation;
				buttonNav.mode = Navigation.Mode.Explicit;

				if (i > 0)
				{
					buttonNav.selectOnUp = lAxisButtons[i - 1];
				}

				if (i < lAxisButtons.Count - 1)
				{
					buttonNav.selectOnDown = lAxisButtons[i + 1];
				}
				else
				{
					buttonNav.selectOnDown = goCalibrationPanelBackButton.GetComponent<Button>();
				}

				lAxisButtons[i].navigation = buttonNav;
			}
		}

		// Set the navigation for the back button
		Button backButton = goCalibrationPanelBackButton.GetComponent<Button>();
		Navigation backNav = backButton.navigation;
		backNav.selectOnUp = lAxisButtons[gamepad.axisCount - 1];
		backButton.navigation = backNav;
	}

    // Selects a gamepad axis to calibrate
    public void SelectAxis(int axis)
    {
        // If a prev axis was selected then set that button back to interactable
        if (iSelectedAxis != -1)
        {
            lAxisButtons[iSelectedAxis].interactable = true;
            SetAxisButtonNavigation(iSelectedAxis, false);
        }

        // Set the current axis and set the corresponding axis button to inactive
        iSelectedAxis = axis;
        lAxisButtons[axis].interactable = false;
        SetAxisButtonNavigation(axis, true);

        // Set up the calibration controls
        goCalibrationControlsParent.SetActive(true);
        goNoAxisSelectedText.SetActive(false);
        currentSelectedAxisText.text = "Selected Axis: " + gamepad.AxisElementIdentifiers[axis].name;

        axisCalibration = calibrationMap.GetAxis(axis);
        SetupUIValues(axis);

        EventSystem.current.SetSelectedGameObject(deadzoneSlider.gameObject);

        if (!testAxisMenuButton.interactable)
        {
            Navigation nav = defaultAxisButton.navigation;
            nav.selectOnRight = testAxisMenuButton;
            defaultAxisButton.navigation = nav;
        }

        testAxisMenuButton.interactable = true;
    }

    // Sets the Nacigation for the axis button based on if it selected or not
    private void SetAxisButtonNavigation(int axis, bool selected)
    {
        // Back Button 
        Button backButton = goCalibrateMenuBackButton.GetComponent<Button>();

        // Selected Button
        if (selected)
        {
            // If button is not at the top
            if (axis > 0)
            {
                Navigation prevNav = lAxisButtons[axis - 1].navigation;

                if (axis < lAxisButtons.Count - 1)
                {
                    prevNav.selectOnDown = lAxisButtons[axis + 1];
                }
                else
                {
                    prevNav.selectOnDown = backButton;
                }

                lAxisButtons[axis - 1].navigation = prevNav;
            }

            // If the button is not at the bottom
            if (axis < lAxisButtons.Count - 1)
            {
                Navigation nextNav = lAxisButtons[axis + 1].navigation;

                if (axis > 0)
                {
                    nextNav.selectOnUp = lAxisButtons[axis - 1];
                }
                else
                {
                    nextNav.selectOnUp = null;
                }

                lAxisButtons[axis + 1].navigation = nextNav;
            }
            else // If we are at the bottom
            {
                Navigation nextNav = backButton.navigation;

                if (axis > 0)
                {
                    nextNav.selectOnUp = lAxisButtons[axis - 1];
                }
                else
                {
                    nextNav.selectOnUp = null;
                }

                backButton.navigation = nextNav;
            }
        }
        else
        {
            if (axis > 0)
            {
                Navigation prevNav = lAxisButtons[axis - 1].navigation;
                prevNav.selectOnDown = lAxisButtons[axis];

                lAxisButtons[axis - 1].navigation = prevNav;
            }

            if (axis < lAxisButtons.Count - 1)
            {
                Navigation nextNav = lAxisButtons[axis + 1].navigation;
                nextNav.selectOnUp = lAxisButtons[axis];

                lAxisButtons[axis + 1].navigation = nextNav;
            }
            else
            {
                Navigation nextNav = backButton.navigation;
                nextNav.selectOnUp = lAxisButtons[axis];

                backButton.navigation = nextNav;
            }
        }
    }

	// Updates the Axis Markers based on the value of the current axis
	private void UpdateAxisMarkers()
	{
		// If not active or no axis is selected, set position to neutral
		if (iSelectedAxis == -1)
		{
			rtAxisPositionMarker.anchoredPosition = new Vector2(0, rtAxisPositionMarker.anchoredPosition.y);
			rtCalibratedPositionMarker.anchoredPosition = new Vector2(0, rtCalibratedPositionMarker.anchoredPosition.y);
			return;
		}

		// Get current value and calibrated value
		float value = gamepad.GetAxis(iSelectedAxis);
		float rawValue = Mathf.Clamp(gamepad.GetAxisRaw(iSelectedAxis), -1.0f, 1.0f);

		// Set positions
		rtAxisPositionMarker.anchoredPosition = new Vector2(rtMarkerZone.rect.width * 0.5f * rawValue, rtAxisPositionMarker.anchoredPosition.y);
		rtCalibratedPositionMarker.anchoredPosition = new Vector2(rtMarkerZone.rect.width * 0.5f * value, rtCalibratedPositionMarker.anchoredPosition.y);
	}

	// Updates the zero marker
	private void UpdateZeroMarker()
	{
		// If not active or no axis is selected
		if (iSelectedAxis == -1)
		{
			return;
		}

		// Set Position
		rtZeroMarker.anchoredPosition = new Vector2(axisCalibration.calibratedZero * rtMarkerZone.rect.width / 2, rtZeroMarker.anchoredPosition.y);
		// Update Deadzone Marker
		UpdateDeadZoneMarker();
	}

	// Updates the Deadzone Marker
	private void UpdateDeadZoneMarker()
	{
		// If not active or no axis is selected
		if (iSelectedAxis == -1)
		{
			return;
		}

		float width = rtMarkerZone.rect.width * axisCalibration.deadZone;
		rtDeadZoneArea.sizeDelta = new Vector2(width, rtDeadZoneArea.sizeDelta.y);
		rtDeadZoneArea.anchoredPosition = new Vector2(0, rtDeadZoneArea.anchoredPosition.y);
	}

	// Redraws the UI
	private void RedrawUI()
	{
		UpdateAxisMarkers();
		UpdateZeroMarker();
	}

	#region UI Methods

	// Sets the UI Values based on the axis data
	private void SetupUIValues(int axis)
	{
		AxisCalibrationData axisData = calibrationMap.GetAxisData(axis);

		deadzoneSlider.value = axisData.deadZone * 100;
		deadzoneValueText.text = (axisData.deadZone).ToString();
		zeroSlider.value = axisData.zero * 100;
		zeroValueText.text = (axisData.zero).ToString();
		sensitivitySlider.value = axisData.sensitivity * 100;
		sensitivityValueText.text = Mathf.CeilToInt(axisData.sensitivity * 25).ToString();
		invertToggle.isOn = axisData.invert;

		UpdateZeroMarker();
	}

	// Resets axis data back to default
	private void ResetUIValues()
	{
		deadzoneSlider.value = 20f;
		deadzoneValueText.text = "20";
		zeroSlider.value = 0;
		zeroValueText.text = "0";
		sensitivitySlider.value = 100;
		sensitivityValueText.text = "25";
		invertToggle.isOn = false;
	}

	// Sets the deadzone to a new size
	public void SetDeadzone(float deadzone)
	{
		deadzoneValueText.text = (deadzone / 100).ToString();

		if (iSelectedAxis == -1)
		{
			return;
		}

		axisCalibration.deadZone = deadzone / 100f;

		if (deadzone == 0)
		{
			axisCalibration.deadZone = 0.001f;
		}

		UpdateZeroMarker();
	}

	// Sets the zero to a new position
	public void SetZero(float zero)
	{
		zeroValueText.text = (zero / 100).ToString();

		if (iSelectedAxis == -1)
		{
			return;
		}

		axisCalibration.calibratedZero = zero / 100;

		UpdateZeroMarker();
	}

	// Sets the sensitivity to a new position
	public void SetSensitivity(float sensitivity)
	{
		sensitivityValueText.text = Mathf.CeilToInt(sensitivity / 4).ToString();

		if (iSelectedAxis == -1)
		{
			return;
		}

		axisCalibration.sensitivity = sensitivity / 100;
	}

	// Sets the axis invert
	public void InvertAxis(bool invert)
	{
		if (!bActive || iSelectedAxis == -1)
		{
			return;
		}

		axisCalibration.invert = invert;
	}

	#endregion
}
