using UnityEngine;

using TMPro;

public class DebugPlayerStateUI : MonoBehaviour
{
	PlayerController player;
	public TextMeshProUGUI stateText;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	// Update
	void Update()
	{
		if (player == null)
		{
			return;
		}

		stateText.text = "Player State: " + player.Motor.motorState + "\n Facing Left: " + player.FacingLeft;
	}
}
