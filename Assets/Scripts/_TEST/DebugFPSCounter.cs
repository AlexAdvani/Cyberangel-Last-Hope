using UnityEngine;
using TMPro;

public class DebugFPSCounter : MonoBehaviour
{
	public TextMeshProUGUI FPSText;

	int iFPS;
	float fTimeThisSecond;
	int iFramesThisSecond;

	// Initialization
	void Awake ()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	// Update
	void Update ()
	{
		CountFPS();
	}

    // Increment the frame counter and update the FPS text if a second has elapsed
	private void CountFPS()
	{
		iFramesThisSecond++;
		fTimeThisSecond += Time.unscaledDeltaTime;

		if (fTimeThisSecond >= 1f)
		{
			iFPS = iFramesThisSecond;
			FPSText.text = "FPS: " + iFPS;

			fTimeThisSecond -= 1f;
			iFramesThisSecond = 0;
		}
	}
}
