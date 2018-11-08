using TMPro;

public class UIMenuGameOptionsScreen : UIMenuScreenManager
{
    // Crosshair Dropdown
    public TMP_Dropdown crosshairDropdown;
    // Laser Sight Dropdown
    public TMP_Dropdown laserSightDropdown;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        bool settingsChanged = SettingsManager.bOptionChanged;

        if (crosshairDropdown != null)
        {
            crosshairDropdown.value = GlobalSettings.iCrosshairVisibility;
            crosshairDropdown.RefreshShownValue();
        }

        laserSightDropdown.value = GlobalSettings.iLaserVisible;
        laserSightDropdown.RefreshShownValue();

        SettingsManager.bOptionChanged = settingsChanged;
	}

    // Cancel Function
    public override bool HandleCancelFunction()
    {
        if (crosshairDropdown != null)
        {
            if (crosshairDropdown.transform.Find("Dropdown List") != null)
            {
                crosshairDropdown.Hide();
                return true;
            }
        }

        if (laserSightDropdown.transform.Find("Dropdown List") != null)
        {
            laserSightDropdown.Hide();
            return true;
        }

        return false;
    }

    // Set Crosshair Visibility
    public void SetCrosshairVisibility(int value)
    {
        GlobalSettings.iCrosshairVisibility = value;
        SettingsManager.bOptionChanged = true;
    }

    // Sets Laser Sight Visibility
    public void SetLaserSightVisibility(int value)
    {
        GlobalSettings.iLaserVisible = value;
        SettingsManager.bOptionChanged = true;
    }
}
