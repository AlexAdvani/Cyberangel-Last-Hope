using UnityEngine;

using Rewired;
using TMPro;

public class InputLegendSprite : MonoBehaviour
{
    // Player Input
    Player playerInput;

    //  Input Image Data
    public InputElementToImageData inputImageData;

	// Button Image
	public SpriteRenderer buttonSprite;
	// Button Text
	public TextMeshPro buttonText;
    // Descriptive Text
    public TextMeshPro descriptiveText;

    // Abbreviate Text Prefix flag
    public bool bAbbrevPrefixes = true;

	// Input Action ID
	[ActionIdProperty(typeof(RewiredActions))]
	public int iInputActionID;
    // Axis Pole
    public Pole axisPole;
    // Full Axis Range flag
    public bool bFullAxisRange = false;

    // Alt Map flag
    public bool bAltMap = false;
    // Primary Map ID
    public int iPrimMapID = 3;
    // Alt Map ID
    public int iAltMapID = 4;

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

        // Map ID (Primary or Alt)
        int mapID = bAltMap ? iAltMapID : iPrimMapID;

        // Current map
        ActionElementMap map = null;
        // Mouse map
        ActionElementMap mouseMap = null;

        if (bFullAxisRange)
        {
            map = playerInput.controllers.maps.GetFirstElementMapWithAction(controller, iInputActionID, false);
            mouseMap = playerInput.controllers.maps.GetFirstElementMapWithAction(ControllerType.Mouse, iInputActionID, false);
        }
        else
        {
            ActionElementMap[] maps = playerInput.controllers.maps.GetFirstMapInCategory(controller.type, controllerID, mapID).GetElementMapsWithAction(iInputActionID);
            ActionElementMap[] mouseMaps = playerInput.controllers.maps.GetFirstMapInCategory(ControllerType.Mouse, controllerID, mapID).GetElementMapsWithAction(iInputActionID);

            for (int i = 0; i < maps.Length; i++)
            {
                if (maps[i].axisContribution == axisPole)
                {
                    map = maps[i];
                }
            }

            for (int j = 0; j < mouseMaps.Length; j++)
            {
                if (mouseMaps[j].axisContribution == axisPole)
                {
                    mouseMap = mouseMaps[j];
                }
            }
        }
        if (!bFullAxisRange)
        {
            if (axisPole == Pole.Positive)
            {
                map.axisRange = AxisRange.Positive;
                mouseMap.axisRange = AxisRange.Positive;
            }
            else
            {
                map.axisRange = AxisRange.Negative;
                mouseMap.axisRange = AxisRange.Negative;
            }
        }

        // If the gamepad is generic or gamepad type is set to generic, set text
        if (controlType == ControllerType.Joystick &&
            ControllerStatusManager.currentGamepadType == eGamepadButtonType.Generic)
        {
            buttonSprite.gameObject.SetActive(false);
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
			buttonSprite.gameObject.SetActive(true);
			buttonText.gameObject.SetActive(false);

			if (controlType == ControllerType.Keyboard)
			{
				if (map.elementIdentifierName == "None" || map.elementIdentifierName == string.Empty)
				{
					buttonSprite.sprite = inputImageData.GetImage(ControllerType.Mouse, mouseMap.elementType, mouseMap.elementIdentifierId, mouseMap.axisRange, axisPole);
				}
				else
				{
					buttonSprite.sprite = inputImageData.GetImage(ControllerType.Keyboard, ControllerElementType.Button, (int)map.keyCode, map.axisRange, axisPole);
				}
			}
			else
			{
                if (bIsAxis2D)
                {
                    buttonSprite.sprite = inputImageData.GetImageAxis2D(ControllerType.Joystick, iInputAxis2DImageID);
                }
                else
                {
                    buttonSprite.sprite = inputImageData.GetImage(ControllerType.Joystick, map.elementType, map.elementIdentifierId, map.axisRange, map.axisContribution);
                }
			}
		}

		currentControlType = ControllerStatusManager.currentControlType;
		currentGamepadType = ControllerStatusManager.currentGamepadType;
	}
}
