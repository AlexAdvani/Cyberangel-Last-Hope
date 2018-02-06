using UnityEngine;
using UnityEngine.UI;

using Rewired;
using TMPro;

public class UIInputElementImage : MonoBehaviour
{
	//  Input Image Data
	public InputElementToImageData inputImageData;

	// Button Image
	public Image buttonImage;
	// Button Text
	public TextMeshProUGUI buttonText;

	// Input Action ID
	[ActionIdProperty(typeof(RewiredActions))]
	public int iInputActionID;
	// Player Input
	Player playerInput;

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
		ActionElementMap map = playerInput.controllers.maps.GetFirstElementMapWithAction(controller, iInputActionID, false);
		// Mouse map
		ActionElementMap mouseMap = playerInput.controllers.maps.GetFirstElementMapWithAction(ControllerType.Mouse, iInputActionID, false);

		// If the gamepad is generic or gamepad type is set to generic, set text
		if (controlType == ControllerType.Joystick && 
			ControllerStatusManager.currentGamepadType == eGamepadButtonType.Generic)
		{
			buttonImage.gameObject.SetActive(false);
			buttonText.gameObject.SetActive(true);

			string prefix = map.elementType == ControllerElementType.Axis ? "A" : "B";
			buttonText.text = prefix + map.elementIndex;

			if (map.elementType == ControllerElementType.Axis)
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
		else // Otherwise, set image
		{
			buttonImage.gameObject.SetActive(true);
			buttonText.gameObject.SetActive(false);

			if (controlType == ControllerType.Keyboard)
			{
				if (map.elementIdentifierName == "None" || map.elementIdentifierName == string.Empty)
				{
					buttonImage.sprite = inputImageData.GetImage(ControllerType.Mouse, mouseMap.elementType, mouseMap.elementIdentifierId, mouseMap.axisRange);
				}
				else
				{
					buttonImage.sprite = inputImageData.GetImage(ControllerType.Keyboard, ControllerElementType.Button, (int)map.keyCode);
				}
			}
			else
			{
				buttonImage.sprite = inputImageData.GetImage(ControllerType.Joystick, map.elementType, map.elementIdentifierId, map.axisRange, map.axisContribution);
			}
		}

		currentControlType = ControllerStatusManager.currentControlType;
		currentGamepadType = ControllerStatusManager.currentGamepadType;
	}
}
