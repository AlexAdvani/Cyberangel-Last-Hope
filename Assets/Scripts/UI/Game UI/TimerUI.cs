using UnityEngine;

using TMPro;

public class TimerUI : MonoBehaviour
{
    // Minutes Text UI
    public TextMeshProUGUI minutesText;
    // Seconds Text UI
    public TextMeshProUGUI secondsText;
    // Milliseconds Text UI
    public TextMeshProUGUI millisecondsText;

    // Current Time
    float fCurrentTime;

	// Minutes
	int iMinutes;
	// Seconds
	int iSeconds;
	// Milliseconds
	int iMilliseconds;

	// Timer Running flag
	bool bRunning;

    #region Public Properties

    // Current Time
    public float CurrentTime
	{
		get { return fCurrentTime; }
	}

	// Running flag
	public bool Running
	{
		get { return bRunning; }
	}

    #endregion

    // Update 
    void Update ()
	{
		if (bRunning)
		{
			fCurrentTime += Time.deltaTime;

			int minutes = Mathf.FloorToInt(fCurrentTime / 60);
			int seconds = Mathf.FloorToInt(fCurrentTime % 60);
			int milliseconds = Mathf.FloorToInt((fCurrentTime * 100) % 100);

            if (minutes != iMinutes)
            {
                iMinutes = minutes;
                minutesText.text = minutes.ToString("00");
            }

            if (seconds != iSeconds)
            {
                iSeconds = seconds;
                secondsText.text = seconds.ToString("00");
            }

            if (milliseconds != iMilliseconds)
            {
                iMilliseconds = milliseconds;
                millisecondsText.text = milliseconds.ToString("00");
            }
        }
	}

	// Get Current Time as a formatted string
	public string SGetTimeString()
	{
		return string.Format("{0:00}:{1:00}.{2:00}", iMinutes, iSeconds, iMilliseconds);
	}

	// Starts the Timer 
	public void StartTimer(bool reset)
	{
		bRunning = true;

		if (reset)
		{
			fCurrentTime = 0;
		}
	}

	// Stops the Timer
	public void StopTimer()
	{
		bRunning = false;
	}

	// Resets the Timer
	public void ResetTimer()
	{
		fCurrentTime = 0;
		iMinutes = 0;
		iSeconds = 0;
		iMilliseconds = 0;

		minutesText.text = "00";
        secondsText.text = "00";
        millisecondsText.text = "00";
    }
}
