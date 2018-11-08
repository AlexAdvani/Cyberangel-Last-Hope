using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;
using TMPro;

public class LoadoutScreen : UIMenuScreenManager
{
    // Player Input
    Player playerInput;
    // Pro Camera 2D Controller
    ProCamera2D proCamera;

    [Space]

    // Primary Weapon Data Array
    public WeaponData[] aPrimaryWeapons;
    // Primary Weapon Data Array
    public WeaponData[] aSecondaryWeapons;
    // Primary Weapon Data Array
    public WeaponData[] aHeavyWeapons;

    [Space]

    // Primary Loadout Menu Data
    public LoadoutMenuData primaryLoadoutMenuData;
    // Secondary Loadout Menu Data
    public LoadoutMenuData secondaryLoadoutMenuData;
    // Heavy Loadout Menu Data
    public LoadoutMenuData heavyLoadoutMenuData;
    // Menu Button Prefab GameObject
    public GameObject goMenuButtonPrefab;

    [Space]

    // Base Menu UI Object
    public GameObject goBaseMenu;
    // Weapon Info Panel UI Object
    public GameObject goWeaponInfoPanel;
    // Current Loadout Panel UI Object
    public GameObject goCurrentLoadoutPanel;
    // Primary Weapon Menu UI Object
    public GameObject goPrimaryMenu;
    // Secondary Weapon Menu UI Object
    public GameObject goSecondaryMenu;
    // Heavy Weapon Menu UI Object
    public GameObject goHeavyMenu;

    // Weapon Info Image
    public Image weaponInfoImage;
    // Weapon Info Name
    public TextMeshProUGUI weaponInfoName;
    // Weapon Info Description
    public TextMeshProUGUI weaponInfoDescription;
    // Weapon Info Ammo Capacity
    public TextMeshProUGUI weaponInfoCapacity;
    // // Weapon Info Damage Bar
    public Slider weaponInfoDamageBar;
    // Weapon Info Fire Rate Bar
    public Slider weaponInfoFireRateBar;

    // Current Primary Weapon Icon
    public Image currentPrimaryWeaponIcon;
    // Current Primary Weapon Text
    public TextMeshProUGUI currentPrimaryWeaponText;
    // Current Secondary Weapon Icon
    public Image currentSecondaryWeaponIcon;
    // Current Secondary Weapon Text
    public TextMeshProUGUI currentSecondaryWeaponText;
    // Weapon Icon Color Palette
    public Color[] acWeaponIconPalette;

    // Camera Zoom In (To Player)
    public float fCameraZoomInLevel;
    // Zoom Transition Duration
    public float fZoomDuration = 1;
    // Initial Camera Size (Half of the Camera's height in world units)
    float fInitialCamSize;

    // Weapon Menu Open flag
    bool bWeaponMenuOpen = false;

    // Initialization
    protected override void Start ()
    {
        base.Start();

        playerInput = ReInput.players.GetPlayer(0);
        proCamera = Camera.main.GetComponent<ProCamera2D>();
        fInitialCamSize = proCamera.ScreenSizeInWorldCoordinates.y * 0.5f;

        InitializeWeaponMenus();
        InitializeCurrentWeaponPanel();
        ZoomInToPlayer();
	}

    // Update
    void FixedUpdate()
    {
        if (playerInput.GetButtonDown("UICancel"))
        {
            if (UIMenuManager.bOverrideCancelFunction && bWeaponMenuOpen)
            {
                CloseWeaponMenu();
            }
        }
    
    }

    // On Destroy
    void OnDestroy()
    {
        primaryLoadoutMenuData.ClearButtonEvents();
        secondaryLoadoutMenuData.ClearButtonEvents();
        heavyLoadoutMenuData.ClearButtonEvents();
    }

    // Initialize Weapon Menus
    private void InitializeWeaponMenus()
    {
        primaryLoadoutMenuData.InitializeMenu(goMenuButtonPrefab, SetPrimaryWeapon, SetWeaponInfoPanelData, aPrimaryWeapons);
        secondaryLoadoutMenuData.InitializeMenu(goMenuButtonPrefab, SetSecondaryWeapon, SetWeaponInfoPanelData, aSecondaryWeapons);
        heavyLoadoutMenuData.InitializeMenu(goMenuButtonPrefab, SetHeavyWeapon, SetWeaponInfoPanelData, aHeavyWeapons);
    }

    // Initializes Weapon Text
    private void InitializeCurrentWeaponPanel()
    {
        currentPrimaryWeaponIcon.sprite = WeaponManager.Instance.PlayerLoadout[0].uiWeaponIcon;
        currentPrimaryWeaponText.text = WeaponManager.Instance.PlayerLoadout[0].sName;
        currentSecondaryWeaponIcon.sprite = WeaponManager.Instance.PlayerLoadout[1].uiWeaponIcon;
        currentSecondaryWeaponText.text = WeaponManager.Instance.PlayerLoadout[1].sName;

        currentPrimaryWeaponIcon.material.SetColor("_Colour1", acWeaponIconPalette[0]);
        currentPrimaryWeaponIcon.material.SetColor("_Colour2", acWeaponIconPalette[1]);
        currentSecondaryWeaponIcon.material.SetColor("_Colour1", acWeaponIconPalette[0]);
        currentSecondaryWeaponIcon.material.SetColor("_Colour2", acWeaponIconPalette[1]);
    }

    // Handles Cancel Input
    public override bool HandleCancelFunction()
    {
        if (!bWeaponMenuOpen)
        {
            SaveLoadout();
            ZoomOutFromPlayer();
        }

        return base.HandleCancelFunction();
    }

    // Zooms in to the player if there is a player in the scene
    private void ZoomInToPlayer()
    {
        if (GameManager.Player == null)
        {
            return;
        }

        if (proCamera != null)
        {
            proCamera.UpdateScreenSize(fInitialCamSize / fCameraZoomInLevel, fZoomDuration);
        }
    }

    // Zooms out from the player if there is a player in the scene
    public void ZoomOutFromPlayer()
    {
        if (GameManager.Player == null)
        {
            return;
        }

        if (proCamera != null)
        {
            proCamera.UpdateScreenSize(fInitialCamSize, fZoomDuration);
        }
    }

    // Opens Primary Weapon Menu
    public void OpenPrimaryMenu()
    {
        goBaseMenu.SetActive(false);
        goPrimaryMenu.SetActive(true);
        goWeaponInfoPanel.SetActive(true);
        goCurrentLoadoutPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(primaryLoadoutMenuData.MenuFirstSelection);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Opens Secondary Weapon Menu
    public void OpenSecondaryMenu()
    {
        goBaseMenu.SetActive(false);
        goSecondaryMenu.SetActive(true);
        goWeaponInfoPanel.SetActive(true);
        goCurrentLoadoutPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(secondaryLoadoutMenuData.MenuFirstSelection);
        GameManager.Player.EquipWeapon(1);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Opens Heavy Weapon Menu
    public void OpenHeavyMenu()
    {
        goBaseMenu.SetActive(false);
        goHeavyMenu.SetActive(true);
        goWeaponInfoPanel.SetActive(true);
        goCurrentLoadoutPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(heavyLoadoutMenuData.MenuFirstSelection);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Closes Any Open Weapon Menu
    public void CloseWeaponMenu()
    {
        goPrimaryMenu.SetActive(false);
        goSecondaryMenu.SetActive(false);
        goHeavyMenu.SetActive(false);
        goBaseMenu.SetActive(true);
        goWeaponInfoPanel.SetActive(false);
        goCurrentLoadoutPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(goFirstSelectedUI);

        if (GameManager.Player.CurrentWeaponIndex != 0)
        {
            GameManager.Player.EquipWeapon(0);
        }

        UIMenuManager.bOverrideCancelFunction = false;
        bWeaponMenuOpen = false;
    }

    // Sets Primary Weapon
    public void SetPrimaryWeapon(int weaponID)
    {
        if (weaponID < 0 || weaponID >= aPrimaryWeapons.Length)
        {
            Debug.LogError("Weapon ID: " + weaponID + " does not exist.");
            return;
        }

        WeaponManager.Instance.SetLoadoutWeapon(aPrimaryWeapons[weaponID], 0);
        currentPrimaryWeaponIcon.sprite = aPrimaryWeapons[weaponID].uiWeaponIcon;
        currentPrimaryWeaponText.text = aPrimaryWeapons[weaponID].sName;

        if (GameManager.Player != null)
        {
            GameManager.Player.SwapWeapon(0, aPrimaryWeapons[weaponID]);
        }

        CloseWeaponMenu();
    }

    // Sets Secondary Weapon
    public void SetSecondaryWeapon(int weaponID)
    {
        if (weaponID < 0 || weaponID >= aSecondaryWeapons.Length)
        {
            Debug.LogError("Weapon ID: " + weaponID + " does not exist.");
            return;
        }

        WeaponManager.Instance.SetLoadoutWeapon(aSecondaryWeapons[weaponID], 1);
        currentSecondaryWeaponIcon.sprite = aSecondaryWeapons[weaponID].uiWeaponIcon;
        currentSecondaryWeaponText.text = aSecondaryWeapons[weaponID].sName;

        if (GameManager.Player != null)
        {
            if (GameManager.Player.lWeaponData[1].sName != aSecondaryWeapons[weaponID].sName)
            {
                GameManager.Player.SwapWeapon(1, aSecondaryWeapons[weaponID]);
            }
        }

        CloseWeaponMenu();
    }

    // Sets Heavy Weapon
    public void SetHeavyWeapon(int weaponID)
    {
        if (weaponID < 0 || weaponID >= aHeavyWeapons.Length)
        {
            Debug.LogError("Weapon ID: " + weaponID + " does not exist.");
            return;
        }

        WeaponManager.Instance.SetLoadoutWeapon(aHeavyWeapons[weaponID], 0, true);
        currentPrimaryWeaponIcon.sprite = aHeavyWeapons[weaponID].uiWeaponIcon;
        currentPrimaryWeaponText.text = aHeavyWeapons[weaponID].sName;
        currentSecondaryWeaponIcon.sprite = null;
        currentSecondaryWeaponText.text = "";

        CloseWeaponMenu();
    }

    // Sets the Weapon Info Panel Data from the appropriate Weapon Data
    private void SetWeaponInfoPanelData(WeaponData[] weaponList, int id)
    {
        WeaponData data = weaponList[id];

        weaponInfoImage.sprite = data.uiWeaponIcon;
        weaponInfoName.text = data.sName;
        weaponInfoDescription.text = data.sDescriptionInfo;
        weaponInfoCapacity.text = data.iClipSize.ToString();
        weaponInfoDamageBar.value = data.iDamageInfoStat;
        weaponInfoFireRateBar.value = data.iFireRateInfoStat;
    }

    // Saves the current loadout
    public void SaveLoadout()
    {
        WeaponManager.Instance.SavePlayerLoadout();
    }
}

[Serializable]
public struct LoadoutMenuData
{
    // Menu Buttons
    Button[] aMenuButtons;

    // Content Transform
    public Transform tContent;
    // Button that links to this menu on the main loadout screen
    public Button mainLoadoutPanelButton;
    // Back Button (For Navigation Setup)
    public Button backButton;

    // First Selected GameObject for the Menu
    GameObject goMenuFirstSelection;

    #region Public Properties

    // First Selected GameObject for the Menu
    public GameObject MenuFirstSelection
    {
        get { return goMenuFirstSelection; }
    }

    #endregion

    // Initialize the Menu 
    public void InitializeMenu(GameObject buttonPrefab, Action<int> buttonAction, Action<WeaponData[], int> selectAction, WeaponData[] weaponList)
    {
        // No weapons in list, disable menu link
        if (weaponList.Length == 0)
        {
            mainLoadoutPanelButton.interactable = false;
            return;
        }

        aMenuButtons = new Button[weaponList.Length];

        // Loop through each weapon and make the respective button
        for (int i = 0; i < weaponList.Length; i++)
        {
            int index = i;

            WeaponData data = weaponList[index];
            Button button = GameObject.Instantiate(buttonPrefab, tContent).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = data.sName;
            button.onClick.AddListener(() => { buttonAction(index); });

            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((eventData) => { selectAction(weaponList, index); });
            eventTrigger.triggers.Add(enterEntry);

            EventTrigger.Entry selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            selectEntry.callback.AddListener((eventData) => { selectAction(weaponList, index); });
            eventTrigger.triggers.Add(selectEntry);

            aMenuButtons[index] = button;
        }

        // Loop through each button for navigation
        for (int j = 0; j < aMenuButtons.Length; j++)
        {
            Navigation nav = aMenuButtons[j].navigation;
            nav.mode = Navigation.Mode.Explicit;

            if (j != 0)
            {
                nav.selectOnUp = aMenuButtons[j - 1];
            }
            else
            {
                goMenuFirstSelection = aMenuButtons[j].gameObject;
            }

            if (j < aMenuButtons.Length - 1)
            {
                nav.selectOnDown = aMenuButtons[j + 1];
            }
            else
            {
                nav.selectOnDown = backButton;
            }

            aMenuButtons[j].navigation = nav;
        }

        // Back Button Navigation
        Navigation backNav = backButton.navigation;
        backNav.mode = Navigation.Mode.Explicit;
        backNav.selectOnUp = aMenuButtons[aMenuButtons.Length - 1];
        backButton.navigation = backNav;
    }

    // Clear all events from the button's Event Trigger
    public void ClearButtonEvents()
    {
        if (aMenuButtons == null || aMenuButtons.Length == 0)
        {
            return;
        }

        for (int i = 0; i < aMenuButtons.Length; i++)
        {
            aMenuButtons[i].GetComponent<EventTrigger>().triggers.Clear();
        }
    }
}