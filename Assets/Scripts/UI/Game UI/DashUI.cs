using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    // Animator
    Animator animator;

    // Recharge Image
    public Image rechargeImage;
    // Time to complete a full recharge
    float fRechargeTime;
    // Current Recharge Time
    float fCurrentRechargeTime = 0f;
    // Delay before Recharge Begins
    float fRechargeDelay;

    // Colour at start of recharge
    public Color startColour;
    // Colour at end of recharge
    public Color endColour;

    // Recharge Coroutiine
    Coroutine rechargeCoroutine;

    // Initialization
    void Start ()
    {
        animator = GetComponent<Animator>();

        fRechargeTime = GameManager.Player.Motor.dashCooldown;
        fRechargeDelay = GameManager.Player.Motor.dashDuration;
        GameManager.Player.onDash += OnDash;
	}

    // On Enable
    void OnEnable()
    {
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
            animator.SetBool("DashActive", true);
            animator.SetTrigger("DashTrigger");
        }
    }

    // On Dash Action
    public void OnDash()
    {
        animator.SetBool("DashActive", false);
        animator.SetTrigger("DashTrigger");

        rechargeImage.fillAmount = 0f;
        rechargeImage.color = startColour;

        StartCoroutine(RechargeDash());
    }

    // Dash Recharge Coroutine
    public IEnumerator RechargeDash()
    {
        yield return new WaitForSeconds(fRechargeDelay);

        fCurrentRechargeTime = 0f;

        while (fCurrentRechargeTime < fRechargeTime)
        {
            fCurrentRechargeTime += Time.deltaTime * Time.timeScale;
            float percentage = fCurrentRechargeTime / fRechargeTime;

            rechargeImage.fillAmount = percentage;
            rechargeImage.color = Color.Lerp(startColour, endColour, percentage);

            yield return null;

            if (fCurrentRechargeTime >= fRechargeTime)
            {
                animator.SetBool("DashActive", true);
                animator.SetTrigger("DashTrigger");
            }
        }
    }
}
