using System;

using UnityEngine;

using Rewired;
using Rewired.Data.Mapping;

public class ControllerStatusManager : SingletonBehaviour<ControllerStatusManager>
{
	Player playerInput;

	public static ControllerType currentControlType;
	public static eGamepadButtonType currentGamepadType;
	public static eGamepadButtonType nativeGamepadType;
	public static int iGamepadID = 0;

	public HardwareJoystickMap[] aControllerGuids;

	// Initialization
	public override void Awake()
	{
		base.Awake();

		playerInput = ReInput.players.GetPlayer(0);

		ReInput.ControllerConnectedEvent += SetPadXml;
		ReInput.ControllerConnectedEvent += GamepadConnectEvent;

		currentControlType = ControllerType.Keyboard;
		currentGamepadType = eGamepadButtonType.Xbox360;

		CheckCurrentGamepadForType();
	}

	// Update
	void Update()
	{
		if (!ReInput.isReady)
		{
			return;
		}

		Controller controller = ReInput.controllers.GetLastActiveController();

		if (controller == null)
		{
			controller = playerInput.controllers.Keyboard;
		}

		if (currentControlType == controller.type)
		{
			return;
		}

		if (controller.type == ControllerType.Mouse)
		{
			if (playerInput.GetAxis("Mouse Horizontal") != 0 ||
				playerInput.GetAxis("Mouse Vertical") != 0)
			{
				return;
			}
		}

		currentControlType = controller.type;
	}

	private void SetPadXml(ControllerStatusChangedEventArgs args)
	{
		if (!SettingsManager.bPadInputAssigned)
		{
			playerInput.controllers.maps.AddMapFromXml(ControllerType.Joystick, 0, SettingsManager.sPadInputXml);
			SettingsManager.bPadInputAssigned = true;
		}
	}

	private void GamepadConnectEvent(ControllerStatusChangedEventArgs args)
	{
		CheckCurrentGamepadForType();
	}

	public void CheckGamepadType(int padID)
	{
		if (padID == 0)
		{
			currentControlType = ControllerType.Keyboard;
		}

		if (playerInput == null)
		{
			playerInput = ReInput.players.GetPlayer(0);
		}

		Joystick gamepad = playerInput.controllers.GetController<Joystick>(padID - 1);

		if (gamepad == null)
		{
			return;
		}

		for (int i = 0; i < aControllerGuids.Length; i++)
		{
			if (gamepad.hardwareTypeGuid != aControllerGuids[i].Guid)
			{
				continue;
			}

			currentGamepadType = (eGamepadButtonType)i;
			nativeGamepadType = currentGamepadType;
			return;
		}

		currentGamepadType = eGamepadButtonType.Generic;
		nativeGamepadType = currentGamepadType;
	}

	private void CheckCurrentGamepadForType()
	{
		if (playerInput.controllers.joystickCount == 0)
		{
			return;
		}

		Joystick gamepad = (Joystick)playerInput.controllers.GetController(ControllerType.Joystick, 0);

		if (gamepad == null)
		{
			return;
		}

		for (int i = 0; i < aControllerGuids.Length; i++)
		{
			if (gamepad.hardwareTypeGuid != aControllerGuids[i].Guid)
			{
				continue;
			}

			currentGamepadType = (eGamepadButtonType)i;
			nativeGamepadType = currentGamepadType;
			return;
		}

		currentGamepadType = eGamepadButtonType.Generic;
		nativeGamepadType = currentGamepadType;
	}
}

public enum eGamepadButtonType
{
	Xbox360 = 0,
	XboxOne = 1,
	XboxOneW10 = 2,
	PlayStation3 = 3,
	PlayStation4 = 4,
	Generic = 5
}
