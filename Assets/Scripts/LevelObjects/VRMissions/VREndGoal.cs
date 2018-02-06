using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VREndGoal : MonoBehaviour
{
	// Trigger Enter
	void OnTriggerEnter2D(Collider2D other)
	{
		// Reset Player
		PlayerController player = GameManager.Player;
		// Movement
		player.Motor.normalizedXMovement = 0;
		player.Motor.normalizedYMovement = 0;
		player.SetDisabledMovement(true);
		player.SetPlayerControlDisabled(true);
		// Aiming
		player.SetAiming(false);
		player.animator.SetArmBusy(false);

		// End the Mission
		VRMissionModeManager.Instance.EndMission();
	}
}
