using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class HealthUI : MonoBehaviour
{
    // Player Health Manager
    HealthManager playerHealth;

    // Health Bar Image
    public Image healthBar;
    // Damage Bar Image
    public Image damageBar;
    // Health Text
    public TextMeshProUGUI healthText;
    // Energy Text
    public TextMeshProUGUI energyText;
    // Current Health value
    int iCurrentHealth;

    // Health Transition Coroutine
    Coroutine transitionCoroutine;
    // Delay before Health Transition Occurs
    public float fHealthTransitionDelay;
    // Speed of Health Transition
    public float fHealthTransitionSpeed = 5f;

    // Standard Icon Colours
    public Color[] acStandardColours;
    // Low Ammo Icon Colours
    public Color[] acLowColours;
    // No Ammo Icon Colours
    public Color[] acEmptyColours;

    // Damage Bar Colour
    public Color cDamageBarColour;
    // Heal Bar Colour
    public Color cHealBarColour;

    // Current Colour Palette
    Color[] acCurrentColours;

	// Initialization
	void Start ()
    {
        playerHealth = GameManager.Player.GetComponent<HealthManager>();

        iCurrentHealth = (int)playerHealth.fMaxHealth;
        healthText.text = iCurrentHealth.ToString();
        healthBar.fillAmount = 1;
        damageBar.fillAmount = 1;

        ChangeColourPalette(acStandardColours);

        playerHealth.onHealthChanged += OnHealthChanged;
	}

    // On Health Changed
    private void OnHealthChanged(float health)
    {
        if (health == iCurrentHealth)
        {
            return;
        }

        int oldHealth = iCurrentHealth;
        iCurrentHealth = (int)health;

        Image affectedBar;

        if (oldHealth < iCurrentHealth)
        {
            affectedBar = damageBar;
            damageBar.color = cHealBarColour;
        }
        else
        {
            affectedBar = healthBar;
            damageBar.color = cDamageBarColour;
        }

        healthText.text = iCurrentHealth.ToString();
        affectedBar.fillAmount = iCurrentHealth / playerHealth.fMaxHealth;

        if (iCurrentHealth == 0)
        {
            energyText.text = "Energy Depleted";
            ChangeColourPalette(acEmptyColours);
        }
        else if (iCurrentHealth / playerHealth.fMaxHealth <= 0.25f)
        {
            energyText.text = "Energy Low";
            ChangeColourPalette(acLowColours);
        }
        else
        {
            energyText.text = "Energy";
            ChangeColourPalette(acStandardColours);
        }

        if (transitionCoroutine != null)
        {
            oldHealth = (int)(damageBar.color == cDamageBarColour ? damageBar.fillAmount * playerHealth.fMaxHealth :
                healthBar.fillAmount * playerHealth.fMaxHealth);
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionHealth(oldHealth));
    }

    // Transitions the health bar when health is changed
    private IEnumerator TransitionHealth(int oldHealth)
    {
        yield return new WaitForSeconds(fHealthTransitionDelay);

        float transitionTime = (float)(Mathf.Abs(oldHealth - iCurrentHealth)) / fHealthTransitionSpeed;
        float transitionTimer = 0f;

        float oldHealthPercentage = oldHealth / playerHealth.fMaxHealth;
        float currentHealthPercentage = iCurrentHealth / playerHealth.fMaxHealth;

        Image transitionBar = oldHealth < iCurrentHealth ? healthBar : damageBar;

        while (transitionTimer < transitionTime)
        {
            transitionTimer += Time.deltaTime;
            transitionBar.fillAmount = Mathf.Lerp(oldHealthPercentage, currentHealthPercentage, transitionTimer / transitionTime);

            yield return null;
        }
    }

    // Change UI colours to new colours
    private void ChangeColourPalette(Color[] colours)
    {
        // If colours are the same as the current palette, then return
        if (acCurrentColours == colours)
        {
            return;
        }

        healthBar.material.SetColor("_Colour1", colours[0]);
        healthBar.material.SetColor("_Colour2", colours[1]);

        healthText.color = colours[0];
        healthText.outlineColor = colours[1] * Colors.SlateGray;

        energyText.color = colours[0];
        energyText.outlineColor = colours[1] * Colors.SlateGray;

        acCurrentColours = colours;
    }
}
