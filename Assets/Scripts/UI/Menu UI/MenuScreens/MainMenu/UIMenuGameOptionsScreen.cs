using TMPro;

public class UIMenuGameOptionsScreen : UIMenuScreenManager
{
    // Crosshair Dropdown
    public TMP_Dropdown crosshairDropdown;
    // Laser Sight Dropdown
    public TMP_Dropdown laserSightDropdown;
    // Slide Input Dropdown
    public TMP_Dropdown slideInputDropdown;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        InitializeSettings();
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

        if (slideInputDropdown.transform.Find("Dropdown List") != null)
        {
            slideInputDropdown.Hide();
            return true;
        }

        return false;
    }

    // Initialize Settings UI
    private void InitializeSettings()
    {
        bool settingsChanged = SettingsManager.bOptionChanged;

        if (crosshairDropdown != null)
        {
            crosshairDropdown.value = GlobalSettings.iCrosshairVisibility;
            crosshairDropdown.RefreshShownValue();
        }

        laserSightDropdown.value = GlobalSettings.iLaserVisible;
        laserSightDropdown.RefreshShownValue();

        slideInputDropdown.value = GlobalSettings.iSlideInputMethod;
        slideInputDropdown.RefreshShownValue();

        SettingsManager.bOptionChanged = settingsChanged;
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

    // Sets Slide Input Method
    public void SetSlideInputMethod(int value)
    {
        GlobalSettings.iSlideInputMethod = value;
        SettingsManager.bOptionChanged = true;
    }
}
