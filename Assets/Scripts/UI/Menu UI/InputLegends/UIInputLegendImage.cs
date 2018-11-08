using UnityEngine;
using UnityEngine.UI;

using Rewired;
using TMPro;

public class UIInputLegendImage : MonoBehaviour
{
    // Player Input
    Player playerInput;

    //  Input Image Data
    public InputElementToImageData inputImageData;

	// Button Image
	public Image buttonImage;
	// Button Text
	public TextMeshProUGUI buttonText;
    // Descriptive Text
    public TextMeshProUGUI descriptiveText;

    // Abbreviate Text Prefix flag
    public bool bAbbrevPrefixes = true;

	// Input Action ID
	[ActionIdProperty(typeof(RewiredActions))]
	public int iInputActionID;
    // Axis Pole
    public Pole axisPole;
    // Full Axis Range flag
    public bool bFullAxisRange = true;

    // Alt Map flag
    public bool bAltMap = false;

    // Axis 2D Input Image ID
    public int iInputAxis2DImageID;
    // Primary Axis Action ID (For Axis 2D)
    [ActionIdProperty(typeof(RewiredActions))]
    public int iPrimaryAxis2DActionID;
    // Secondary Axis Action ID (For Axis 2D)
    [ActionIdProperty(typeof(RewiredActions))]
    public int iSecondaryAxis2DActionID;
    // Is Axis 2D flag
    public bool bIsAxis2D = false;

	// Current Control Type
	ControllerType currentControlType;
	// Current Gamepad Type
	eGamepadButtonType currentGamepadType;

	// Initialization
	void Start ()
	{
		currentControlType = ControllerStatusManager.currentControlType;
		currentGamepadType = ControllerStatusManager.currentGamepadType;

		playerInput = ReInput.players.GetPlayer(0);

		SetControllerImage();
	}
	
	// Update
	void Update ()
	{
		// If the control type and gamepad type are unchanged
		if (currentControlType == ControllerStatusManager.currentControlType && currentGamepadType == ControllerStatusManager.currentGamepadType)
		{
			return;
		}

		SetControllerImage();
	}

	// Sets the controller image based on control type, gamepad type and element id
	private void SetControllerImage()
	{
        if (iInputActionID == 0)
        {
            return;
        }

		ControllerType controlType = ControllerType.Keyboard;
		int controllerID = 0;

		// Check control type
		if (ControllerStatusManager.currentControlType == ControllerType.Keyboard ||
			ControllerStatusManager.currentControlType == ControllerType.Mouse)
		{
			controlType = ControllerType.Keyboard;
		}
		else if (ControllerStatusManager.currentControlType == ControllerType.Joystick)
		{
			if (playerInput.controllers.joystickCount > 0)
			{
				controlType = ControllerType.Joystick;
				controllerID = ControllerStatusManager.iGamepadID;
			}
		}

		// Current controller
		Controller controller = ReInput.controllers.GetController(controlType, controllerID);

		// If there is no controller, set the controller to the keyboard
		if (controller == null)
		{
			controller = ReInput.controllers.GetController<Keyboard>(0);
			controlType = ControllerType.Keyboard;
		}

        // Current map
        ActionElementMap map = playerInput.controllers.maps.GetFirstElementMapWithAction(controller, iInputActionID, false); ;
        // Mouse map
        ActionElementMap mouseMap = playerInput.controllers.maps.GetFirstElementMapWithAction(ControllerType.Mouse, iInputActionID, false);
        
        // If the gamepad is generic or gamepad type is set to generic, set text
        if (controlType == ControllerType.Joystick &&
            ControllerStatusManager.currentGamepadType == eGamepadButtonType.Generic)
        {
            buttonImage.gameObject.SetActive(false);
            buttonText.gameObject.SetActive(true);

            string prefix;

            if (!bIsAxis2D)
            {
                string axis = bAbbrevPrefixes ? "A" : "Axis ";
                string button = bAbbrevPrefixes ? "B" : "Button ";

                prefix = map.elementType == ControllerElementType.Axis ? axis : button;
                buttonText.text = prefix + map.elementIndex;
            }

			if (bIsAxis2D || map.elementType == ControllerElementType.Axis)
			{
                if (bIsAxis2D)
                {
                    ActionElementMap primaryMap = playerInput.controllers.maps.GetFirstElementMapWithAction(controller, iPrimaryAxis2DActionID, false);
                    ActionElementMap secondMap = playerInput.controllers.maps.GetFirstElementMapWithAction(controller, iSecondaryAxis2DActionID, false);

                    buttonText.text = "Axis " + primaryMap.elementIndex + " / " + "Axis "+ secondMap.elementIndex;
                }
                else
                {
                    if (map.axisRange == AxisRange.Positive)
                    {
                        buttonText.text += "+";
                    }
                    else if (map.axisRange == AxisRange.Negative)
                    {
                        buttonText.text += "-";
                    }
                }
			}
		}
		else // Otherwise, set image
		{
			buttonImage.gameObject.SetActive(true);
			buttonText.gameObject.SetActive(false);

			if (controlType == ControllerType.Keyboard)
			{
				if (map.elementIdentifierName == "None" || map.elementIdentifierName == string.Empty)
				{
					buttonImage.sprite = inputImageData.GetImage(ControllerType.Mouse, mouseMap.elementType, mouseMap.elementIdentifierId, mouseMap.axisRange, axisPole);
				}
				else
				{
					buttonImage.sprite = inputImageData.GetImage(ControllerType.Keyboard, ControllerElementType.Button, (int)map.keyCode, map.axisRange, axisPole);
				}
			}
			else
			{
                if (bIsAxis2D)
                {
                    buttonImage.sprite = inputImageData.GetImageAxis2D(ControllerType.Joystick, iInputAxis2DImageID);
                }
                else
                {
                    buttonImage.sprite = inputImageData.GetImage(ControllerType.Joystick, map.elementType, map.elementIdentifierId, map.axisRange, map.axisContribution);
                }
			}
		}

		currentControlType = ControllerStatusManager.currentControlType;
		currentGamepadType = ControllerStatusManager.currentGamepadType;
	}
}
