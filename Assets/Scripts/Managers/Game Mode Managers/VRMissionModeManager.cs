using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;
using Com.LuisPedroFonseca.ProCamera2D;

public class VRMissionModeManager : SingletonBehaviour<VRMissionModeManager>
{
	// VR Menu Manager
	public UIMenuManager vrMenuManager;

	// Level data
	public LevelData levelData;
	// Next Mission Level Data
	public LevelData nextMissionLevelData;

	// Player Starting Position
	Vector3 v3PlayerStartPos;
	// Camera
	ProCamera2D gameCamera;
	// Number of Targets in Level
	int iTargets;
	// Target Gameobjects
	GameObject[] agoTargets;
	// Best Time (Seconds);
	float fBestTime;
	// End Mission Rank
	string sEndRank = "";

	// In Game UI
	public GameObject goInGameUI;
	// Mission Timer
	public TimerUI missionTimer;
	// Target Counter UI 
	public GameObject goTargetCounterUI;
	// End Goal GameObject
	GameObject goEndGoal;

	// Screen Fade Transition UI
	public GameObject goScreenFadeUI;

	#region Public Properties

	// Targets
	public int Targets
	{
		get { return iTargets; }
	}

	// Best Time
	public float BestTime
	{
		get { return fBestTime; }
	}

	// End Mission Rank
	public string EndRank
	{
		get { return sEndRank; }
	}

	#endregion

	// On Create Initialization
	public override void Awake()
	{
		base.Awake();

		InitializeLevelElements();

		GameManager.SetPause(false);
	}

    // Initialization
	void Start()
	{
		if (levelData != null)
		{
			if (levelData.bHasTime)
			{
				fBestTime = ProfileDataManager.Instance.LoadVRMissionTime(levelData.sLevelName);
			}
		}
	}

	// Finds and Initializes All Targets in Level
	private void InitializeLevelElements()
	{
		v3PlayerStartPos = GameObject.FindGameObjectWithTag("Player").transform.position;
		gameCamera = Camera.main.GetComponent<ProCamera2D>();
		agoTargets = GameObject.FindGameObjectsWithTag("VRTarget");
		iTargets = agoTargets.Length;
		goEndGoal = GameObject.FindGameObjectWithTag("VRGoal");

		if (goEndGoal != null)
		{
			goEndGoal.SetActive(false);
		}
	}

	// Decrements the Target Counter
	public void DecrementTargets()
	{
		iTargets--;

		if (iTargets == 0)
		{
			goEndGoal.SetActive(true);
		}
	}

	#region Mission Management

	// Starts the Mission Timer
	public void StartMissionTimer()
	{
		missionTimer.gameObject.SetActive(true);
		missionTimer.StartTimer(true);
	}

	// Ends the Mission 
	public void EndMission()
	{
		// No previous best time or beat best time
		if (fBestTime == -1 || missionTimer.CurrentTime < fBestTime)
		{
			fBestTime = missionTimer.CurrentTime;
			ProfileDataManager.Instance.SaveVRMissionTime(levelData.sLevelName, fBestTime);
		}

		// Check Time for Rank
		CheckEndRanking(missionTimer.CurrentTime);

		// Stop Timer
		StopMissionTimer();

		// Show End Screen UI
		vrMenuManager.gameObject.SetActive(true);
		GameManager.PauseMenuManager.goPauseUI.SetActive(false);
		vrMenuManager.GoToScreen("VREnd");
	}

	// Checks time and awards a rank
	private void CheckEndRanking(float time)
	{
		if (time < levelData.goldTime.FGetTimeInSeconds())
		{
			sEndRank = "Gold";
		}
		else if (time < levelData.silverTime.FGetTimeInSeconds())
		{
			sEndRank = "Silver";
		}
		else if (time < levelData.bronzeTime.FGetTimeInSeconds())
		{
			sEndRank = "Bronze";
		}
		else
		{
			sEndRank = "None";
		}
	}

	// Stops the Mission Timer
	public void StopMissionTimer()
	{
		missionTimer.StopTimer();
		missionTimer.gameObject.SetActive(false);
	}

	// Reloads the scene to retry the mission
	public void RetryMission()
	{
		// Pause Game
		GameManager.SetPause(true);
		// Make time flow normally
		Time.timeScale = 1;
		// Screen Transition
		TransitionHelper.TransitionOut(goScreenFadeUI, EndRetryTransition);
	}

	// Ends the screen transition for a mission retry
	private void EndRetryTransition()
	{
		// Player
		GameManager.Player.ResetPlayer(v3PlayerStartPos);
		// End Goal
		goEndGoal.SetActive(false);

		// Stop and Reset Mission Timer
		missionTimer.StopTimer();
		missionTimer.ResetTimer();

		// Camera
		gameCamera.CenterOnTargets();

		// Targets
		iTargets = agoTargets.Length;

		for (int i = 0; i < agoTargets.Length; i++)
		{
			agoTargets[i].SetActive(true);
			agoTargets[i].GetComponent<HealthManager>().Revive();
		}

		// Projectiles in Scene
		GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

		for (int i = 0; i < projectiles.Length; i++)
		{
			Destroy(projectiles[i]);
		}

		// Debris in Scene
		GameObject[] debris = GameObject.FindGameObjectsWithTag("Debris");

		for (int i = 0; i < debris.Length; i++)
		{
			Destroy(debris[i]);
		}

        vrMenuManager.gameObject.SetActive(true);
		vrMenuManager.GoToScreen("VRStart");
		AudioManager.Instance.StopAllSounds();

		TransitionHelper.TransitionIn(goScreenFadeUI);
	}

	// If there is a next mission, load it 
	public void NextMission()
	{
		// If next mission not available, return
		if (nextMissionLevelData == null)
		{
			return;
		}

		ExitSceneDestroy();
		SceneLoadManager.Instance.LoadScene(nextMissionLevelData.sSceneName);
	}

	// Destroys self (For Use When Exiting VR Mission Scene)
	public void ExitSceneDestroy()
	{
		DestroySingleton();
	}

	#endregion
}