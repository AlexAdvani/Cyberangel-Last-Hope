using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class InGameUI : MonoBehaviour
{
	// Health Bar
	public Image healthBar;
	// Health Text
	public TextMeshProUGUI healthText;
	// Dash Icon GameObject
	public GameObject goDashIcon;

	// Weapon Name Text
	public TextMeshProUGUI weaponNameText;
	// Weapon Ammo Text
	public TextMeshProUGUI weaponAmmoText;

    // Player Controller
    PlayerController playerController;
    // Player Health Manager
    HealthManager playerHealth;
	
    // Current Health
    float fCurrentHealth = -1;
    // Current Dash State - Can Player Dash
    bool bCurrentDashState = false;
    // Current Weapon Name
    string sCurrentWeaponName = string.Empty;
    // Current Weapon Clip Ammo
    float fCurrentClipAmmo = -1;
    // Current Weapon Held Ammo
    float fCurrentHeldAmmo = -1;

	// Initialization
	void Start()
	{
		playerHealth = GameManager.Player.GetComponent<HealthManager>();
		playerController = GameManager.Player.GetComponent<PlayerController>();
	}

	// Update 
	void Update ()
	{
		if (playerHealth == null || playerController == null)
		{
			return;
		}

        UpdateHealthUI();
        UpdateDashUI();
        UpdateWeaponUI();
	}

    // Updates Health UI if player health has changed
    private void UpdateHealthUI()
    {
        if (playerHealth.Health == fCurrentHealth)
        {
            return;
        }

        healthBar.fillAmount = playerHealth.Health / playerHealth.fMaxHealth;
        healthText.text = "Energy: " + playerHealth.Health;

        fCurrentHealth = playerHealth.Health;
    }

    // Updates Dash UI if player dash state has changed
    private void UpdateDashUI()
    {
        if (playerController.Motor.canDash == bCurrentDashState)
        {
            return;
        }

        goDashIcon.SetActive(playerController.Motor.canDash);

        bCurrentDashState = playerController.Motor.canDash;
    }

    // Updates Weapon UI if current weapon or ammo count has changed
    private void UpdateWeaponUI()
    {
        if (playerController.CurrentWeapon.Name != sCurrentWeaponName)
        {
            weaponNameText.text = playerController.CurrentWeapon.Name;
            weaponAmmoText.text = playerController.CurrentWeapon.HeldAmmo + " | " + playerController.CurrentWeapon.ClipAmmo;

            sCurrentWeaponName = playerController.CurrentWeapon.Name;
            fCurrentClipAmmo = playerController.CurrentWeapon.ClipAmmo;
            fCurrentHeldAmmo = playerController.CurrentWeapon.HeldAmmo;
        }
        else if (playerController.CurrentWeapon.ClipAmmo != fCurrentClipAmmo ||
            playerController.CurrentWeapon.HeldAmmo != fCurrentHeldAmmo)
        {
            weaponAmmoText.text = playerController.CurrentWeapon.HeldAmmo + " | " + playerController.CurrentWeapon.ClipAmmo;

            fCurrentClipAmmo = playerController.CurrentWeapon.ClipAmmo;
            fCurrentHeldAmmo = playerController.CurrentWeapon.HeldAmmo;
        }
    }
}
