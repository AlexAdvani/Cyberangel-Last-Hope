using System.Collections;

using UnityEngine;

using TMPro;

public class VRMissionCountdownScreen : UIMenuScreenManager
{
	// Countdown Time to Mission Start (Seconds)
	public int iCountdownTime = 3;
	// Countdown Time Remaining (Seconds)
	int iCountdownRemaining;
	// Countdown Text
	public TextMeshProUGUI countdownText;

	// Initialization
	protected override void Start ()
	{
        VRMissionModeManager.Instance.goInputButtonsUI.SetActive(false);
        StartCoroutine(CountdowntoStart());
	}

	// Countdown to Start of the Mission
	private IEnumerator CountdowntoStart()
	{
		iCountdownRemaining = iCountdownTime;

		while (iCountdownRemaining > 0)
		{
			countdownText.text = iCountdownRemaining.ToString();
			yield return new WaitForSecondsRealtime(1);

			iCountdownRemaining--;
		}

		GameManager.SetPause(false);
		GameManager.Player.SetPlayerControlDisabled(false);
		GameManager.Player.SetDisabledMovement(false);

		VRMissionModeManager.Instance.StartMissionTimer();
        VRMissionModeManager.Instance.goInGameUI.SetActive(true);
        VRMissionModeManager.Instance.goMissionUI.SetActive(true);
        GameManager.DisablePause(false);
		GameManager.SetPause (false);
		UIMenuManager.bMenuInputDisabled = true;

		MenuManager.gameObject.SetActive(false);
	}
}
