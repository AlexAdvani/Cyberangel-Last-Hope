using UnityEngine.UI;

using Rewired;
using TMPro;

public class UIMenuControlsOptionsScreen : UIMenuScreenManager
{
	// Player Input
	Player playerInput;

	// Gamepad Selection Dropdown
	public TMP_Dropdown gamepadDropdown;
	// Vibration Toggle
	public Toggle vibrationToggle;

	// Gamepad Calibrator
	public UIInputGamepadCalibrator padCalibrator;

	// Initialization
	protected override void Start ()
	{
		playerInput = ReInput.players.GetPlayer(0);

		// Assign to Controller Connect and Disconnect Events
		ReInput.ControllerConnectedEvent += ControllerEventChange;
		ReInput.ControllerDisconnectedEvent += ControllerEventChange;

		GetGamepads();

		vibrationToggle.isOn = GlobalSettings.bVibration;
	}

	// Update
	void Update ()
	{
		if (UIMenuManager.bOverrideCancelFunction)
		{
			HandleCancelInput();
		}
	}

	// Handle Cancel Input
	private void HandleCancelInput()
	{
		if (playerInput.GetButtonDown("Cancel"))
		{
			if (UIMenuInputMapper.bAssignWindowOpen && UIMenuInputMapper.currentActiveMapper != null)
			{
				if (!UIMenuInputMapper.bPolling)
				{
					UIMenuInputMapper.currentActiveMapper.CloseAssignmentWindow();
				}
			}
			else if (padCalibrator.bAxisOpen)
			{
				padCalibrator.CloseAxisWindow();
			}
			else if (padCalibrator.bCalibrationOpen)
			{
				padCalibrator.CloseCalibrationWindow();
			}
		}
	}

	// Get the current connected gamepads when a controller is connected or disconnected
	private void ControllerEventChange(ControllerStatusChangedEventArgs args)
	{
		GetGamepads();
	}

	// Gets the Currently Connected Gamepads 
	private void GetGamepads()
	{
		gamepadDropdown.ClearOptions();
		gamepadDropdown.options.Add(new TMP_Dropdown.OptionData("None"));
		gamepadDropdown.RefreshShownValue();

		if (gamepadDropdown.options.Count == 1)
		{
			Toggle option = gamepadDropdown.GetComponentInChildren<Toggle>(true);

			if (option != null)
			{
				Navigation nav = new Navigation()
				{
					mode = Navigation.Mode.None
				};
				option.navigation = nav;
			}
		}

		Joystick[] gamepads = ReInput.controllers.GetJoysticks();

		if (gamepads != null && gamepads.Length > 0)
		{
			Joystick playerPad = null;

			if (playerInput.controllers.joystickCount > 0)
			{
				playerPad = ReInput.controllers.GetController<Joystick>(playerInput.controllers.GetController<Joystick>(0).id);
			}

			int padArrayPos = 0;
			int connectedPads = 0;

			for (int i = 0; i < gamepads.Length; i++)
			{
				Joystick gamepad = gamepads[i];

				gamepadDropdown.options.Add(new TMP_Dropdown.OptionData(gamepad.name));

				if (gamepad.isConnected)
				{
					connectedPads++;
				}

				if (playerPad != null && gamepad == playerPad)
				{
					padArrayPos = i;
				}
			}

			if (connectedPads > 0)
			{
				if (ReInput.configuration.autoAssignJoysticks)
				{
					gamepadDropdown.value = padArrayPos + 1;
				}
				else
				{
					gamepadDropdown.value = 0;
				}

				gamepadDropdown.RefreshShownValue();
			}
		}
	}

	// Assigns a gamepad to Player 1 based on Gamepad ID
	public void SetGamepad(int gamepad)
	{
		playerInput.controllers.RemoveController<Joystick>(0);

		if (gamepad > 0)
		{
			Controller newGamepad = ReInput.controllers.GetController(ControllerType.Joystick, gamepad - 1);

			if (newGamepad != null)
			{
				playerInput.controllers.AddController(newGamepad, true);
			}
		}
	}

	// Sets Vibration
	public void SetVibration(bool vibration)
	{
		GlobalSettings.bVibration = vibration;
	}
}
