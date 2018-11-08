using UnityEngine;

using TMPro;

public class VRTargetCounterUI : MonoBehaviour
{
	// Target Counter Text
	public TextMeshProUGUI counterText;
    public GameObject goDivideColon;
    public GameObject goTargetIcon;
    int iTargetsRemaining = -1;

	// Initialization
	void Start()
	{
        VRMissionModeManager.Instance.onTargetDestroy += UpdateText;
        VRMissionModeManager.Instance.onMissionRestart += ResetUI;

		UpdateText();
	}

	// Updates the Counter Text
	private void UpdateText()
	{
		iTargetsRemaining = VRMissionModeManager.Instance.Targets;

		if (iTargetsRemaining > 0)
		{
			counterText.text = iTargetsRemaining.ToString();
        }
		else
		{
			counterText.text = "Head to Goal!";
            goDivideColon.SetActive(false);
            goTargetIcon.SetActive(false);
        }
	}

    // Resets the UI back to original state
    public void ResetUI()
    {
        goDivideColon.SetActive(true);
        goTargetIcon.SetActive(true);

        UpdateText();
    }
}
