using UnityEngine;

using TMPro;

public class VRTargetCounterUI : MonoBehaviour
{
	// Target Counter Text
	TextMeshProUGUI counterText;
    int iTargetsRemaining = -1;

	// Initialization
	void Start()
	{
		counterText = GetComponent<TextMeshProUGUI>();
		UpdateText();
	}

	// Update 
	void Update ()
	{
		UpdateText();
	}

	// Updates the Counter Text
	private void UpdateText()
	{
		if (GameManager.GamePaused)
		{
			return;
		}

        if (VRMissionModeManager.Instance.Targets == iTargetsRemaining)
        {
            return;
        }

		iTargetsRemaining = VRMissionModeManager.Instance.Targets;

		if (iTargetsRemaining > 0)
		{
			counterText.text = "Targets: " + iTargetsRemaining;
		}
		else
		{
			counterText.text = "Head to Goal!";
		}
	}
}
