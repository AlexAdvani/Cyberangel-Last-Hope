using System;

using UnityEngine;

[CreateAssetMenu(menuName = "This Game Data/Level Data")]
public class LevelData : ScriptableObject
{
	// Level Name
	public string sLevelName;
	// Scene Name 
	public string sSceneName;
	// Level Image
	public Sprite levelImage;

	// Level Has Time flag
	public bool bHasTime = true;

	// Gold Time Requirement
	public LevelTime goldTime;
	// Silver Time Requirement
	public LevelTime silverTime;
	// Bronze Time Requirement
	public LevelTime bronzeTime;
}

// Time For Level in Minutes, Seconds and Milliseconds
[Serializable]
public class LevelTime
{
	// Time 
	DateTime time;

	// Minutes
	public int iMinutes;
	// Seconds
	public int iSeconds;
	// Milliseconds
	public int iMilliseconds;

	// Converts a time in seconds into a formatted string 
	public static string SConvertToTimeString(float time)
	{
		float minutes = Mathf.Floor(time / 60);
		float seconds = Mathf.Floor(time % 60);
		float milliseconds = Mathf.Floor((time * 100) % 100);

		return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
	}

	// Return the time as a string
	public string SGetTimeString()
	{
		time = new DateTime();

		time = time.AddMinutes(iMinutes);
		time = time.AddSeconds(iSeconds);
		time = time.AddMilliseconds(iMilliseconds);

		string timeString = time.ToString("mm:ss");
		timeString += "." + time.Millisecond.ToString("00");

		return timeString;
	}

	// Return the time in seconds (For Easy Realtime Comparison)
	public float FGetTimeInSeconds()
	{
		return (iMinutes * 60f) + iSeconds + (iMilliseconds / 1000f);
	}
}
