using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{ 
    // Animator
    Animator animator;
    // Array of Currently Equipped Weapons via their Names
    List<string> lsCurrentWeaponNames;
    // Array of Weapon UI Icon Behaviours
    public WeaponUIIconBehaviour[] aWeaponIcons;
    // Array of Ammo UI Icon Behaviours
    public AmmoUIIconBehaviour[] aAmmoUIIcons;
    // Weapon Icon Transforms
    RectTransform[] artWeaponIconsTransforms;
    // Weapon Icon Initial Positions
    Vector2[] av2WeaponIconInitialPositions;
    Vector3[] av3WeaponIconInitialScales;
    // Current Ammo UI Index
    int iCurrentAmmoUI = 0;

    // Ammo UI Change Coroutine
    Coroutine ammoUICoroutine;
    public float fAmmoUIChangeDelay = 0.25f;

	// Initialisation
	void Start ()
    {
        animator = GetComponent<Animator>();
        lsCurrentWeaponNames = new List<string>();

        for (int i = 0; i < GameManager.Player.Weapons.Count; i++)
        {
            lsCurrentWeaponNames.Add(GameManager.Player.Weapons[i].Name);
        }

        GameManager.Player.aOnWeaponSwitch += SwitchWeapon;
        
        InitializeAmmoUI();
	}

    // On Enable
    void OnEnable()
    {
        CheckWeaponIcons();
    }

    /// Initializes Ammo UI by activating UI for the current weapon and disabling the others
    private void InitializeAmmoUI()
    {
        for (int i = 0; i < aAmmoUIIcons.Length; i++)
        {
            if (i == iCurrentAmmoUI)
            {
                aAmmoUIIcons[i].gameObject.SetActive(true);
                continue;
            }

            aAmmoUIIcons[i].gameObject.SetActive(false);
        }

        artWeaponIconsTransforms = new RectTransform[aWeaponIcons.Length];
        av2WeaponIconInitialPositions = new Vector2[aWeaponIcons.Length];
        av3WeaponIconInitialScales = new Vector3[aWeaponIcons.Length];

        for (int j = 0; j < aWeaponIcons.Length; j++)
        {
            aWeaponIcons[j].goReloadIcon.SetActive(false);
            artWeaponIconsTransforms[j] = aWeaponIcons[j].GetComponent<RectTransform>();
            av2WeaponIconInitialPositions[j] = artWeaponIconsTransforms[j].anchoredPosition;
            av3WeaponIconInitialScales[j] = artWeaponIconsTransforms[j].localScale;
        }
    }

    // Checks if the weapon icons need to be updated
    private void CheckWeaponIcons()
    {
        if (lsCurrentWeaponNames == null)
        {
            return;
        }

        for (int i = 0; i < GameManager.Player.Weapons.Count; i++)
        {
            if (GameManager.Player.Weapons[i].Name != lsCurrentWeaponNames[i])
            {
                aWeaponIcons[i].ResetWeaponIcon();
                aAmmoUIIcons[i].ResetAmmoUI();
            }
        }
    }

    // Trigger Swap Weapon UI Animation
    private void SwitchWeapon(int weaponIndex, bool instantSwitch)
    {
        animator.SetInteger("WeaponIndex", weaponIndex);
        animator.SetTrigger("SwitchTrigger");

        if (ammoUICoroutine != null)
        {
            StopCoroutine(ammoUICoroutine);
        }

        if (instantSwitch || !gameObject.activeInHierarchy)
        {
            ChangeActiveWeaponUI(weaponIndex);
            SetIconsManually(weaponIndex);
        }
        else
        { 
            ammoUICoroutine = StartCoroutine("ChangeActiveWeaponCouroutine", weaponIndex);
        }
    }

    // Couroutine to change the Active Ammo UI
    private IEnumerator ChangeActiveWeaponCouroutine(int weaponIndex)
    {
        yield return new WaitForSeconds(fAmmoUIChangeDelay);

        ChangeActiveWeaponUI(weaponIndex);
    }

    // Changes the Active Ammo UI
    private void ChangeActiveWeaponUI(int weaponIndex)
    {
        if ((float)(GameManager.Player.Weapons[iCurrentAmmoUI].ClipAmmo) / (float)(GameManager.Player.Weapons[iCurrentAmmoUI].weaponData.iClipSize) <= 0.25f)
        {
            aWeaponIcons[iCurrentAmmoUI].goReloadIcon.SetActive(true);
        }

        // Change active ammo UI
        aAmmoUIIcons[iCurrentAmmoUI].gameObject.SetActive(false);
        aAmmoUIIcons[weaponIndex].gameObject.SetActive(true);

        aWeaponIcons[weaponIndex].goReloadIcon.SetActive(false);

        iCurrentAmmoUI = weaponIndex;
    }

    // Sets the Icons Manuallt with out animating
    private void SetIconsManually(int weaponIndex)
    {
        for (int i = 0; i < artWeaponIconsTransforms.Length; i++)
        {
            int newIndex = (i + weaponIndex) % artWeaponIconsTransforms.Length;

            artWeaponIconsTransforms[i].anchoredPosition = av2WeaponIconInitialPositions[newIndex];
            artWeaponIconsTransforms[i].localScale = av3WeaponIconInitialScales[newIndex];

            if (i == weaponIndex)
            {
                aWeaponIcons[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                aWeaponIcons[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }
}
