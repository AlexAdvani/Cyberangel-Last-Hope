using System;

using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    // Current Game State
    eGameState gameState = eGameState.NoState;

    // Player Controller - null if not present in scene or wrong game state
    static PlayerController player;

    // Pause Menu Manager - null if not present in scene or wrong game state
    static PauseMenuManager pauseMenuManager;

    // Game Paused flag
    static bool bGamePaused;
    // Pause Disabled flag
    static bool bPauseDisabled;

    // On Pause Action
    public static Action onPause;
    // On Unpause Action
    public static Action onUnpause;

    #region Public Variables

    // Game State
    public eGameState GameState
    {
        get { return gameState; }
    }

    // In Game flag
    public bool InGame
    {
        get { return (gameState == eGameState.HubGame ||
                gameState == eGameState.MainGame ||
                gameState == eGameState.TargetGame ||
                gameState == eGameState.TutorialGame); }
    }

    // Player
    public static PlayerController Player
    {
        get
        {
            if (player == null)
            {
                Instance.FindPlayer();
            }

            return player;
        }
    }

    public static PauseMenuManager PauseMenuManager
    {
        get
        {
            if (pauseMenuManager == null)
            {
                Instance.FindPauseMenuManager();
            }

            return pauseMenuManager;
        }
    }

	// Game Paused flag
	public static bool GamePaused
	{
		get{ return bGamePaused; }
	}

	// Paused Disabled flag
	public static bool PauseDisabled
	{
		get {return bPauseDisabled; }
	}

    #endregion

    // Set Current Game State
    public void SetGameState(eGameState newState)
	{
		gameState = newState;

		FindPlayer();
	}

	// Find the Player Object
	private void FindPlayer()
	{
		if (InGame)
		{
			GameObject obj = GameObject.FindGameObjectWithTag("Player");

			if (obj != null)
			{
				player = obj.GetComponent<PlayerController>();
			}
			else
			{
				player = null;
				Debug.LogError("Player not found in a Game Mode Scene. Please add a Player.");
			}
		}
		else
		{
			player = null;
		}
	}

    // Sets Pause
	public static void SetPause(bool paused)
	{
		if (bPauseDisabled)
		{
			return;
		}

        if (paused)
        {
            if (onPause != null)
            {
                onPause();
            }
        }
        else
        {
            if (onUnpause != null)
            {
                onUnpause();
            }
        }

		bGamePaused = paused;
	}

    // Disables pausing the game
	public static void DisablePause(bool disabled)
	{
		bPauseDisabled = disabled;
	}

    // Finds the Pause Menu Manager in the current scene
	private void FindPauseMenuManager()
	{
		if (InGame)
		{
			GameObject obj = GameObject.FindGameObjectWithTag("PauseManager");

			if (obj != null)
			{
				pauseMenuManager = obj.GetComponent<PauseMenuManager>();
			}
			else
			{
				pauseMenuManager = null;
				Debug.LogError("Pause Manager not found in a Game Mode Scene. Please add a Pause Manager.");
			}
		}
		else
		{
			pauseMenuManager = null;
		}
	}

    // Removes the Pause Menu Manager from the Game Manager
    public static void RemovePauseMenuManager()
    {
        pauseMenuManager = null;
    }
}

// Game State Types
public enum eGameState
{
	NoState,
	Intro,
	Loading,
	Menu,
	MainGame,
	HubGame,
	TargetGame,
	TutorialGame
}