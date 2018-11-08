using UnityEngine;
using UnityEngine.UI;

public class WeaponUIIconBehaviour : MonoBehaviour
{
    // Player Weapon to get data from
    WeaponBehaviour playerWeapon;

    // Weapon Image
    Image weaponImage;

    // Standard Icon Colours
    public Color[] acStandardColours;
    // Low Ammo Icon Colours
    public Color[] acLowColours;
    // No Ammo Icon Colours
    public Color[] acEmptyColours;

    // Reload Icon
    public GameObject goReloadIcon;

    // Is Temporary Weapon flag (Any weapon that may picked up temporarily)
    public bool bTemporaryWeapon;
    // Player Weapon Index (Position in player weapon array)
    public int iPlayerWeaponIndex;

    // Initialization
    void Start()
    {
        playerWeapon = GameManager.Player.Weapons[iPlayerWeaponIndex];

        weaponImage = GetComponent<Image>();
        ResetWeaponIcon();

        SetIconColours(acStandardColours);

        playerWeapon.onWeaponReload += UpdateIcon;
        playerWeapon.onWeaponEmpty += UpdateIcon;
        playerWeapon.onWeaponRefill += UpdateIcon;
    }

    // Set New Weapon Icon
    public void ResetWeaponIcon()
    {
        playerWeapon = GameManager.Player.Weapons[iPlayerWeaponIndex];
        weaponImage.sprite = playerWeapon.weaponData.uiWeaponIcon;
        Rect imageBounds = weaponImage.sprite.textureRect;
        float width = weaponImage.rectTransform.rect.height * (imageBounds.width / imageBounds.height);
        weaponImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    // Update Icon overload method
    private void UpdateIcon()
    {
        UpdateIcon(0);
    }

    // Updates the Weapon Icon Colours based on remaining ammo count
    private void UpdateIcon(int temp)
    {
        if (playerWeapon.TotalAmmo == 0)
        {
            SetIconColours(acEmptyColours);
        }
        else if ((float)playerWeapon.HeldAmmo / (float)playerWeapon.weaponData.iMaxAmmo <= 0.25f)
        {
            SetIconColours(acLowColours);
        }
    }

    // Sets the icon colours in the image shader
    private void SetIconColours(Color[] colours)
    {
        weaponImage.material.SetColor("_Colour1", colours[0]);
        weaponImage.material.SetColor("_Colour2", colours[1]);
    }
}
