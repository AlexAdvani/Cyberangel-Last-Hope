using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class BarAmmoUI : MonoBehaviour
{
    // Clip Ammo Text
    public TextMeshProUGUI clipAmmoText;
    // Bar Ammo Slider
    public Slider barAmmoSlider;
    // Bar Background Image
    public Image barBackgroundImage;
    // Bar Fill Image
    public Image barFillImage;

    // Initialize Beam Ammo UI
    public void InitializeUI(float clipAmmo)
    {
        clipAmmoText.text = ((int)clipAmmo).ToString();
        barAmmoSlider.maxValue = (int)clipAmmo;
        barAmmoSlider.value = (int)clipAmmo;
    }

    // Update Beam Ammo UI
    public void UpdateUI(float clipAmmo)
    {
        clipAmmoText.text = ((int)clipAmmo).ToString();
        barAmmoSlider.value = (int)clipAmmo;
    }
}
