using System;

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
    // Camera Starting Position
    Vector3 v3CameraStartPos;
	// Number of Targets in Level
	int iTargets;
	// Target Gameobjects
	GameObject[] agoTargets;
	// Best Time (Seconds);
	float fBestTime;
    // Best Score
    int iBestScore;
    // Par Time (Seconds);
    float fParTime;
    // Time Bonus
    int iTimeBonus;
    // Time Bonus Score
    public float fTimeBonusScore;
    // End Mission Rank
    string sEndRank = "";

	// In Game UI
	public GameObject goInGameUI;
    // Mission UI
    public GameObject goMissionUI;
    // Input Buttons UI
    public GameObject goInputButtonsUI;
	// Mission Timer
	public TimerUI missionTimer;
    // Mission Score
    public ScoreUI missionScore;
    // End Goal GameObject
    GameObject goEndGoal;

	// Screen Fade Transition UI
	public GameObject goScreenFadeUI;

    // On Target Destroy Action
    public Action onTargetDestroy;
    // On Mission Restart Action
    public Action onMissionRestart;

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

    // Par Time
    public int BestScore
    {
        get { return iBestScore; }
    }

    // End Mission Rank
    public string EndRank
	{
		get { return sEndRank; }
	}

    // Time Bonus
    public int TimeBonus
    {
        get { return iTimeBonus; }
    }


	#endregion

	// On Create Initialization
	public override void Awake()
	{
		base.Awake();

		InitializeLevelElements();

		GameManager.SetPause(false);
        GameManager.onPause += HideVRModeUI;
        GameManager.onUnpause += ShowVRModeUI;

        HideVRModeUI();
    }

    // Initialization
	void Start()
	{
		if (levelData != null)
		{
            if (levelData.bHasScore)
            {
                iBestScore = ProfileDataManager.Instance.LoadVRMissionScore(levelData.sLevelName);
            }
            else if (levelData.bHasTime)
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
        v3CameraStartPos = gameCamera.transform.position;
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

        if (onTargetDestroy != null)
        {
            onTargetDestroy();
        }
	}

    // Show VR Mode UI
    public void ShowVRModeUI()
    {
        if (goMissionUI != null)
        {
            goMissionUI.SetActive(true);
        }
    }

    // Hide VR Mode UI
    public void HideVRModeUI()
    {
        if (goMissionUI != null)
        {
            goMissionUI.SetActive(false);
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
        // Check Time for Rank
        if (levelData.bHasScore)
        {
            CalculateTimeBonus();
            CheckEndRankingScore(missionScore.Score);
        }
        else if (levelData.bHasTime)
        {
            CheckEndRanking(missionTimer.CurrentTime);
        }

		// Stop Timer
		StopMissionTimer();

        if (levelData.bHasScore)
        {
            if (iBestScore == -1 || missionScore.Score > iBestScore)
            {
                iBestScore = missionScore.Score;
                ProfileDataManager.Instance.SaveVRMissionScore(levelData.sLevelName, iBestScore);
            }
        }
        else if (levelData.bHasTime)
        {
            // No previous best time or beat best time
            if (fBestTime == -1 || missionTimer.CurrentTime < fBestTime)
            {
                fBestTime = missionTimer.CurrentTime;
                ProfileDataManager.Instance.SaveVRMissionTime(levelData.sLevelName, fBestTime);
            }
        }

        // Disable Mission UI
        goMissionUI.SetActive(false);

        // Show End Screen UI
        vrMenuManager.gameObject.SetActive(true);
		GameManager.PauseMenuManager.goPauseUI.SetActive(false);
		vrMenuManager.GoToScreen("VREnd");
	}

    // Calculates the time bonus from difference between the mission time and the par time
    private void CalculateTimeBonus()
    {
        float difference = levelData.parTime.FGetTimeInSeconds() - missionTimer.CurrentTime;

        if (difference > 0)
        {
            iTimeBonus = Mathf.RoundToInt(difference * fTimeBonusScore);
        }
        else
        {
            iTimeBonus = 0;
        }

        missionScore.AddScore(iTimeBonus);
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

    // Checks time and awards a rank
    private void CheckEndRankingScore(float score)
    {
        if (score > levelData.iGoldScore)
        {
            sEndRank = "Gold";
        }
        else if (score > levelData.iSilverScore)
        {
            sEndRank = "Silver";
        }
        else if (score < levelData.iBronzeScore)
        {
            sEndRank = "Bronze";
        }
        else
        {
            sEndRank = "None";
        }
    }

    // Add Score to the Mission Score
    public void AddScore(int amount, bool multiplied = false, bool increaseMultiplier = false)
    {
        if (missionScore == null)
        {
            return;
        }

        missionScore.AddScore(amount, multiplied, increaseMultiplier);
    }

    // Take Score from the Mission Score
    public void TakeScore(int amount, bool multiplied = false, bool decreaseMultiplier = false)
    {
        if (missionScore == null)
        {
            return;
        }

        missionScore.TakeScore(amount, multiplied, decreaseMultiplier);
    }

    // Reset Mission Score
    public void ResetScore(bool resetMultiplier = false)
    {
        if (missionScore == null)
        {
            return;
        }

        missionScore.ResetScore(resetMultiplier);
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

        // Reset Score if it exists
        if (missionScore != null)
        {
            missionScore.ResetScore(true);
        }

        // Disable Mission UI
        goMissionUI.SetActive(false);

        // Camera
        gameCamera.Reset();
        gameCamera.transform.position = v3CameraStartPos;

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
            projectiles[i].SetActive(false);
		}

		// Debris in Scene
		GameObject[] debris = GameObject.FindGameObjectsWithTag("Debris");

		for (int i = 0; i < debris.Length; i++)
		{
            debris[i].SetActive(false);
		}

        vrMenuManager.gameObject.SetActive(true);
		vrMenuManager.GoToScreen("VRStart");
		AudioManager.Instance.StopAllSounds();

        if (onMissionRestart != null)
        {
            onMissionRestart();
        }

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