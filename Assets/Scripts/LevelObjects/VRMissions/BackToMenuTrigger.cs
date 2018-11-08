using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BackToMenuTrigger : MonoBehaviour
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
        // Laser Sight
        player.CurrentWeapon.laserSight.EnableLaser(false);

        VRMissionModeManager.Instance.ExitSceneDestroy();
        SceneLoadManager.Instance.LoadScene("MainMenu");
    }
}
