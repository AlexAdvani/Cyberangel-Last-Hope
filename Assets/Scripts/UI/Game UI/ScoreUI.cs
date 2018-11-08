using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Score Text UI
    public TextMeshProUGUI scoreText;
    // Multiplier Text UI
    public TextMeshProUGUI multiplierText;
    // Multiplier Image UI
    public Image multiplierImage;

    // Current Score
    int iScore;
    // Current Multiplier
    int iMultiplier;
    // Max Multiplier
    public int iMaxMultiplier = 10;

    // Multiplier Timer
    float fMultiplierTimer;
    // Time before Multipler Resets
    public float fMultiplierResetTime = 2f;
    // Time before Multiplier Timer Starts
    public float fMultiplierTimerDelay = 0.25f;

    // Multiplier Image Start Colour
    public Color multiplierImageStartColour;
    // Multiplier Image End Colour
    public Color multiplierImageEndColour;

    // Multiplier Timer Coroutine
    Coroutine multiplierTimerCoroutine;

    #region Public Properties

    // Current Score
    public int Score
    {
        get { return iScore; }
    }

    // Current Multiplier
    public int Multiplier
    {
        get { return iMultiplier; }
    }

    #endregion

    // Initialization
    void Start()
    {
        ResetScore(true);
    }

    // Add Score
    public void AddScore(int amount, bool multiplied = false, bool increaseMultiplier = false)
    {
        if (multiplied)
        {
            amount *= iMultiplier;
        }

        iScore += amount;

        scoreText.text = iScore.ToString();

        if (increaseMultiplier)
        {
            IncreaseMultiplier();
        }
    }

    // Take Score
    public void TakeScore(int amount, bool multiplied = false, bool decreaseMultiplier = false)
    {
        if (multiplied)
        {
            amount *= iMultiplier;
        }

        iScore -= amount;
        iScore = Mathf.Max(iScore, 0);
        scoreText.text = iScore.ToString();

        if (decreaseMultiplier)
        {
            DecreaseMultiplier();
        }
    }

    // Reset Score
    public void ResetScore(bool resetMultiplier = false)
    {
        iScore = 0;
        scoreText.text = iScore.ToString();

        if (resetMultiplier)
        {
            ResetMultiplier();
        }
    }

    // Increase Multiplier
    public void IncreaseMultiplier()
    {
        if (iMultiplier < iMaxMultiplier)
        {
            iMultiplier++;
            multiplierText.text = iMultiplier.ToString() + "x";
        }

        if (multiplierTimerCoroutine != null)
        {
            StopCoroutine(multiplierTimerCoroutine);
        }

        multiplierTimerCoroutine = StartCoroutine(UpdateMultiplierTimer());
    }

    // Decrease Multiplier
    public void DecreaseMultiplier()
    {
        if (iMultiplier > 1)
        {
            iMultiplier--;
            multiplierText.text = iMultiplier.ToString() + "x";
        }

        if (multiplierTimerCoroutine != null)
        {
            StopCoroutine(multiplierTimerCoroutine);
        }

        multiplierTimerCoroutine = StartCoroutine(UpdateMultiplierTimer());
    }

    // Reset Multiplier
    public void ResetMultiplier()
    {
        iMultiplier = 1;
        multiplierText.text = iMultiplier.ToString() + "x";

        if (multiplierTimerCoroutine != null)
        {
            StopCoroutine(multiplierTimerCoroutine);
        }

        multiplierImage.fillAmount = 0;
    }

    // Updates the Multiplier Timer 
    private IEnumerator UpdateMultiplierTimer()
    {
        multiplierImage.fillAmount = 1f;
        multiplierImage.color = multiplierImageStartColour;

        yield return new WaitForSeconds(fMultiplierTimerDelay);

        fMultiplierTimer = 0f;

        while (fMultiplierTimer < fMultiplierResetTime)
        {
            fMultiplierTimer += Time.deltaTime * Time.timeScale;
            float percentage = fMultiplierTimer / fMultiplierResetTime;

            multiplierImage.fillAmount = 1 - percentage;
            multiplierImage.color = Color.Lerp(multiplierImageStartColour, multiplierImageEndColour, percentage);

            yield return null;

            if (fMultiplierTimer >= fMultiplierResetTime)
            {
                ResetMultiplier();
            }
        }
    }
}
