using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class AmmoUIIconBehaviour : MonoBehaviour
{
    // Player Weapon to get data from
    WeaponBehaviour playerWeapon;
    // Rect Transform
    RectTransform rectTransform;

    // Ammo Icons
    Image[] aAmmoIcons;
    // Ammo UI Prefab
    public GameObject goAmmoUIPrefab;
    // Beam Ammo UI Prefab
    public GameObject goBarAmmoUIPrefab;
    // Ammo UI Row
    public GameObject goAmmoUIRow;
    // Ammo UI Row Parent Transform
    public RectTransform rtAmmoUIRowParent;
    // Reserve Ammo Text
    public TextMeshProUGUI reserveAmmoText;
    // Infinite Ammo Icon for Reserve Ammo
    public GameObject goReserveInfinityIcon;
    // Reload Icon
    public Image reloadIcon;
    // Bullet in Chamber Text
    public TextMeshProUGUI chamberIndicatorText;
    // Current Colour Palette
    Color[] acCurrentColourPalette;
    // Beam Ammo UI Behaviour
    BarAmmoUI barAmmoUI;

    // Is Bar Weapon
    bool bBarWeapon;
    // Is Temporary Weapon flag(Any weapon that may picked up temporarily)
    public bool bTemporaryWeapon;
    // Player Weapon Index (Position in player weapon array)
    public int iPlayerWeaponIndex;

    // Standard Icon Colours
    public Color[] acStandardColours;
    // Low Ammo Icon Colours
    public Color[] acLowColours;
    // No Ammo Icon Colours
    public Color[] acEmptyColours;
    // Ammo Material
    public Material ammoMaterial;

    // Initialization
    void Start()
    {
        ResetAmmoUI();

        if (playerWeapon != GameManager.Player.CurrentWeapon)
        {
            gameObject.SetActive(false);
        }

        chamberIndicatorText.gameObject.SetActive(false);
    }

    // Resets the Ammo UI to the current Player Weapon ammo
    public void ResetAmmoUI()
    {
        if (!bTemporaryWeapon)
        {
            playerWeapon = GameManager.Player.Weapons[iPlayerWeaponIndex];
        }

        if (playerWeapon.weaponData.uiAmmoIcon == null)
        {
            InitializeBarAmmoUI();
        }
        else
        {
            InitializeAmmoUI();
        }

        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtAmmoUIRowParent.rect.width);

        // Reset Weapon Events
        playerWeapon.onWeaponFire = null;
        playerWeapon.onWeaponEmpty = null;
        playerWeapon.onWeaponRefill = null;
        playerWeapon.onWeaponReload = null;

        // Set Weapon Events
        playerWeapon.onWeaponFire += OnWeaponFire;
        playerWeapon.onWeaponEmpty += OnWeaponEmpty;
        playerWeapon.onWeaponRefill += OnWeaponRefill;
        playerWeapon.onWeaponReload += OnWeaponReload;
    }

    // Initialize Ammo Icons
    private void InitializeAmmoUI()
    {
        // If there are already icons then remove them
        if (rtAmmoUIRowParent.childCount > 0)
        {
            foreach (Transform child in rtAmmoUIRowParent)
            {
                Destroy(child.gameObject);
            }
        }

        // Weapon Data
        WeaponData weaponData = playerWeapon.weaponData;
        // Ammo Icons
        aAmmoIcons = new Image[weaponData.iClipSize];
        // Bullets per UI Row
        int bulletsPerRow = weaponData.iClipSize / weaponData.iUIRowCount;

        // Spawn Bullets
        for (int r = 0; r < weaponData.iUIRowCount; r++)
        {
            Transform row = Instantiate(goAmmoUIRow, rtAmmoUIRowParent).transform;
            HorizontalLayoutGroup rowGroup = row.GetComponent<HorizontalLayoutGroup>();
            float aspectRatio;
            float spacing;

            // Resize icons based on number of rows
            if (weaponData.iUIRowCount == 1)
            {
                aspectRatio = 0.5f;
                spacing = 2f;
            }
            else if (weaponData.iUIRowCount == 2)
            {
                aspectRatio = 0.675f;
                spacing = 1.25f;
            }
            else
            {
                aspectRatio = 0.85f;
                spacing = 0.75f;
            }

            rowGroup.spacing = spacing;

            for (int c = 0; c < bulletsPerRow; c++)
            {
                Image newIcon = Instantiate(goAmmoUIPrefab, row).GetComponent<Image>();
                newIcon.sprite = playerWeapon.weaponData.uiAmmoIcon;
                newIcon.material = ammoMaterial;

                AspectRatioFitter ratioFitter = newIcon.GetComponent<AspectRatioFitter>();
                ratioFitter.aspectRatio = aspectRatio;

                aAmmoIcons[(r * bulletsPerRow) + c] = newIcon;
            }
        }

        Canvas.ForceUpdateCanvases();

        bBarWeapon = false;

        // Set Text and Text Colours
        if (playerWeapon.bInfiniteAmmo)
        {
            goReserveInfinityIcon.SetActive(true);
            reserveAmmoText.gameObject.SetActive(false);
        }
        else
        {
            goReserveInfinityIcon.SetActive(false);
            reserveAmmoText.gameObject.SetActive(true);
            reserveAmmoText.text = playerWeapon.weaponData.iMaxAmmo.ToString();
        }

        reloadIcon.gameObject.SetActive(false);

        SetAllColours(acStandardColours);
    }

    // Initializes Bar Ammo UI
    private void InitializeBarAmmoUI()
    {
        // If there are already icons then remove them
        if (rtAmmoUIRowParent.childCount > 0)
        {
            foreach (Transform child in rtAmmoUIRowParent)
            {
                Destroy(child.gameObject);
            }
        }

        // Weapon Data
        WeaponData weaponData = playerWeapon.weaponData;

        barAmmoUI = Instantiate(goBarAmmoUIPrefab, rtAmmoUIRowParent).GetComponent<BarAmmoUI>();
        barAmmoUI.barAmmoSlider.maxValue = weaponData.iClipSize;
        barAmmoUI.barAmmoSlider.value = weaponData.iClipSize;
        barAmmoUI.clipAmmoText.text = weaponData.iClipSize.ToString();
        bBarWeapon = true;

        // Set Text and Text Colours
        if (playerWeapon.bInfiniteAmmo)
        {
            goReserveInfinityIcon.SetActive(true);
            reserveAmmoText.gameObject.SetActive(false);
        }
        else
        {
            goReserveInfinityIcon.SetActive(false);
            reserveAmmoText.gameObject.SetActive(true);
            reserveAmmoText.text = playerWeapon.weaponData.iMaxAmmo.ToString();
        }

        reloadIcon.gameObject.SetActive(false);

        SetAllColours(acStandardColours);

        Canvas.ForceUpdateCanvases();
    }

    // On Weapon Fire
    private void OnWeaponFire(int clipAmmo)
    {
        if (clipAmmo < 0 || clipAmmo >= playerWeapon.weaponData.iClipSize)
        {
            chamberIndicatorText.gameObject.SetActive(false);
            return;
        }

        if (bBarWeapon)
        {
            if (barAmmoUI.barAmmoSlider.value != clipAmmo)
            {
                barAmmoUI.barAmmoSlider.value = clipAmmo;
                barAmmoUI.clipAmmoText.text = clipAmmo.ToString();
            }
        }
        else
        {
            SetIconTint(aAmmoIcons[clipAmmo], Color.grey, 150);
        }

        if ((float)clipAmmo / (float)(playerWeapon.weaponData.iClipSize) <= 0.25f)
        {
            SetIconColours(acLowColours);
            acCurrentColourPalette = acLowColours;

            reloadIcon.gameObject.SetActive(true);

            if (barAmmoUI != null)
            {
                SetTextColours(barAmmoUI.clipAmmoText, acLowColours);
            }
        }
    }

    // On Weapon Reload
    private void OnWeaponReload(int clipAmmo)
    {
        if (bBarWeapon)
        {
            barAmmoUI.barAmmoSlider.value = clipAmmo;
            barAmmoUI.clipAmmoText.text = clipAmmo.ToString();
        }
        else
        {
            // Bullet icons to reactivate
            int reactivateBullets = clipAmmo;

            // If bullet in chamber, then avoid array out of range
            if (clipAmmo > playerWeapon.weaponData.iClipSize)
            {
                chamberIndicatorText.gameObject.SetActive(true);
                reactivateBullets = playerWeapon.weaponData.iClipSize;
            }

            // Reactivate bullet icons
            for (int i = 0; i < reactivateBullets; i++)
            {
                SetIconTint(aAmmoIcons[i], Color.white, 255);
            }
        }

        if ((float)clipAmmo / (float)(playerWeapon.weaponData.iClipSize) <= 0.25f)
        {
            SetIconColours(acLowColours);
            acCurrentColourPalette = acLowColours;

            reloadIcon.gameObject.SetActive(true);
        }
        else if (acCurrentColourPalette != acStandardColours)
        {
            SetIconColours(acStandardColours);
            SetTextColours(chamberIndicatorText, acStandardColours);
            acCurrentColourPalette = acStandardColours;

            reloadIcon.gameObject.SetActive(false);
        }

        if ((float)playerWeapon.HeldAmmo / (float)playerWeapon.weaponData.iMaxAmmo <= 0.25f)
        {
            SetTextColours(reserveAmmoText, acLowColours);
        }

        if (barAmmoUI != null)
        {
            SetTextColours(barAmmoUI.clipAmmoText, acCurrentColourPalette);
        }

        reserveAmmoText.text = playerWeapon.HeldAmmo.ToString();
    }

    // Set the icon tint colour and alpha 
    private void SetIconTint(Image icon, Color tint, int alpha)
    {
        tint.a = alpha;
        icon.color = tint;
    }

    // On Weapon Empty - Set UI Colours to Empty Colours
    private void OnWeaponEmpty()
    {
        SetAllColours(acEmptyColours);
    }

    // On Weapon Refill - Set UI Colours to Standard Colours
    private void OnWeaponRefill()
    {
        if (playerWeapon.ClipAmmo > 0)
        {
            OnWeaponReload(playerWeapon.ClipAmmo);
        }

        if ((float)playerWeapon.HeldAmmo/ (float)(playerWeapon.weaponData.iMaxAmmo) <= 0.25f)
        {
            SetTextColours(reserveAmmoText, acLowColours);
        }
        else if (playerWeapon.HeldAmmo > 0)
        {
            SetTextColours(reserveAmmoText, acStandardColours);
        }
    }

    // Set Colours for all Ammo UI
    private void SetAllColours(Color[] colours)
    {
        SetIconColours(colours);
        SetTextColours(chamberIndicatorText, colours);
        SetTextColours(reserveAmmoText, colours);

        if (barAmmoUI != null)
        {
            SetTextColours(barAmmoUI.clipAmmoText, colours);
        }

        acCurrentColourPalette = colours;
    }

    // Sets the icon colours in the image shader
    private void SetIconColours(Color[] colours)
    {
        if (bBarWeapon)
        {
            barAmmoUI.barBackgroundImage.color = colours[0] * Colors.SlateGray;
            barAmmoUI.barFillImage.color = colours[1];
            reloadIcon.material.SetColor("_Colour1", colours[0]);
            reloadIcon.material.SetColor("_Colour2", colours[1]);
        }
        else
        {
            for (int i = 0; i < aAmmoIcons.Length; i++)
            {
                Image icon = aAmmoIcons[i];

                icon.material.SetColor("_Colour1", colours[0]);
                icon.material.SetColor("_Colour2", colours[1]);
            }
        }
    }

    // Sets the Reserve Ammo text colours
    private void SetTextColours(TextMeshProUGUI text, Color[] colours)
    {
        text.color = colours[1];
        text.outlineColor = colours[0] * Colors.SlateGray;
    }
}


