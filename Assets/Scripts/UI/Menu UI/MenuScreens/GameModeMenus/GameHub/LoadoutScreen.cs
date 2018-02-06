using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using Rewired;
using TMPro;

public class LoadoutScreen : UIMenuScreenManager
{
    Player playerInput;

    // Primary Weapon Data Array
    public WeaponData[] aPrimaryWeapons;
    // Primary Weapon Data Array
    public WeaponData[] aSecondaryWeapons;
    // Primary Weapon Data Array
    public WeaponData[] aMountedWeapons;

    // Base Menu UI Object
    public GameObject goBaseMenu;
    // Primary Weapon Menu UI Object
    public GameObject goPrimaryMenu;
    // Secondary Weapon Menu UI Object
    public GameObject goSecondaryMenu;
    // Mounted Weapon Menu UI Object
    public GameObject goMountedMenu;

    // Primary Weapon Menu First Selected Object
    public GameObject goPrimaryFirstSelectedUI;
    // Secondary Weapon Menu First Selected Object
    public GameObject goSecondaryFirstSelectedUI;
    // Mounted Weapon Menu First Selected Object
    public GameObject goMountedFirstSelectedUI;

    // Current Primary Weapon Text
    public TextMeshProUGUI currentPrimaryWeaponText;
    // Current Secondary Weapon Text
    public TextMeshProUGUI currentSecondaryWeaponText;

    // Weapon Menu Open flag
    bool bWeaponMenuOpen = false;

    // Initialization
    protected override void Start ()
    {
        base.Start();

        playerInput = ReInput.players.GetPlayer(0);
        InitializeWeaponText();
	}

    // Update
    void Update()
    {
        if (UIMenuManager.bOverrideCancelFunction)
        {
            HandleCancelInput();
        }
        else
        {
            if (playerInput.GetButtonDown("UICancel"))
            {
                SaveLoadout();
            }
        }
    }

    // Initializes Weapon Text
    private void InitializeWeaponText()
    {
        currentPrimaryWeaponText.text = WeaponManager.Instance.PlayerLoadout[0].sName;
        currentSecondaryWeaponText.text = WeaponManager.Instance.PlayerLoadout[1].sName;
    }

    // Handles Cancel Input 
    private void HandleCancelInput()
    {
        if (bWeaponMenuOpen)
        {
            if (playerInput.GetButtonDown("UICancel"))
            {
                CloseWeaponMenu();
            }
        }
    }

    // Opens Primary Weapon Menu
    public void OpenPrimaryMenu()
    {
        goBaseMenu.SetActive(false);
        goPrimaryMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(goPrimaryFirstSelectedUI);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Opens Secondary Weapon Menu
    public void OpenSecondaryMenu()
    {
        goBaseMenu.SetActive(false);
        goSecondaryMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(goSecondaryFirstSelectedUI);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Opens Arm-Mounted Weapon Menu
    public void OpenMountedMenu()
    {
        goBaseMenu.SetActive(false);
        goMountedMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(goMountedFirstSelectedUI);

        UIMenuManager.bOverrideCancelFunction = true;
        bWeaponMenuOpen = true;
    }

    // Closes Any Open Weapon Menu
    public void CloseWeaponMenu()
    {
        goPrimaryMenu.SetActive(false);
        goSecondaryMenu.SetActive(false);
        goMountedMenu.SetActive(false);
        goBaseMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(goFirstSelectedUI);

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

    // Sets Mounted Weapon
    public void SetMountedWeapon(int weaponID)
    {
        if (weaponID < 0 || weaponID >= aMountedWeapons.Length)
        {
            Debug.LogError("Weapon ID: " + weaponID + " does not exist.");
            return;
        }

        WeaponManager.Instance.SetLoadoutWeapon(aMountedWeapons[weaponID], 0, true);
        currentPrimaryWeaponText.text = aMountedWeapons[weaponID].sName;
        currentSecondaryWeaponText.text = "";

        CloseWeaponMenu();
    }

    // Saves the current loadout
    public void SaveLoadout()
    {
        WeaponManager.Instance.SavePlayerLoadout();
    }
}