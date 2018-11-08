using Rewired;
using Rewired.Data.Mapping;

public class ControllerStatusManager : SingletonBehaviour<ControllerStatusManager>
{
	Player playerInput;

	public static ControllerType currentControlType;
	public static eGamepadButtonType currentGamepadType;
	public static eGamepadButtonType nativeGamepadType;
	public static int iGamepadNo = 0;
    public static int iGamepadID = 0;

	public HardwareJoystickMap[] aControllerGuids;

	// Initialization
	public override void Awake()
	{
		base.Awake();

		playerInput = ReInput.players.GetPlayer(0);

		ReInput.ControllerConnectedEvent += SetPadXml;
		ReInput.ControllerConnectedEvent += GamepadConnectEvent;

#if UNITY_EDITOR || UNITY_STANDALONE
        currentControlType = ControllerType.Keyboard;
#else 
        currentControlType = ControllerType.Joystick;
#endif

        currentGamepadType = eGamepadButtonType.Xbox360;

		CheckCurrentGamepadForType(-1);
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
		CheckCurrentGamepadForType(args.controllerId);
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

        if (playerInput.controllers.joystickCount == 0)
        {
            return;
        }

		Joystick gamepad = playerInput.controllers.Joysticks[padID - 1];

		if (gamepad == null)
		{
			return;
		}

		for (int i = 0; i < aControllerGuids.Length; i++)
		{
            if (aControllerGuids[i] == null)
            {
                continue;
            }

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

	private void CheckCurrentGamepadForType(int controllerID)
	{
		if (playerInput.controllers.joystickCount == 0)
		{
			return;
		}

        if (controllerID == -1)
        {
            if (iGamepadID == -1)
            {
                controllerID = playerInput.controllers.Joysticks[0].id;
            }

            controllerID = iGamepadID;
        }
        else if (iGamepadID != controllerID)
        {
            iGamepadID = controllerID;
        }

        Joystick gamepad = (Joystick)playerInput.controllers.GetController(ControllerType.Joystick, controllerID);

		if (gamepad == null)
		{
			return;
		}

		for (int i = 0; i < aControllerGuids.Length; i++)
		{
            if (aControllerGuids[i] == null)
            {
                continue;
            }

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
