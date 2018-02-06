using UnityEngine;

public class GameStateSetter : MonoBehaviour
{
	// Game State to Set
	public eGameState gameState;

	// Initialization
	void Awake ()
	{
		GameManager.Instance.SetGameState(gameState);
	}
}
