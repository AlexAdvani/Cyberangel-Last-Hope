using System.Collections;

using UnityEngine;

public class LaserSightBehaviour : MonoBehaviour
{
	// Laser Sight Line Renderer
	LineRenderer laserSightRenderer;
	// Laser Sight Colour
	public Color laserSightColour;
	// Laser Sight Collision Layer Mask
	public LayerMask laserSightLayerMask;

	// Fade Coroutine
	Coroutine fadeCoroutine;
	// Laser Fade Clear Colour Alpha 
	public float fFadeAlpha;
	// Laser Fade In/Out Time
	public float fFadeTime;
	// Current Time for a Fade 
	float fCurrentFadeTime;
	// Delay between Fade In and Fade Out
	public float fFadeDelay;

	// Laser Sight Active flag
	bool bLaserSightActive = false;

	#region Public Properties

	// Laser Sight Active flag
	public bool LaserSightActive
	{
		get{ return bLaserSightActive; }
	}

	#endregion

	// Initialization
	void Start ()
    {
		laserSightRenderer = GetComponent<LineRenderer>();

		if (laserSightRenderer != null)
		{
			SetLaserColour(laserSightColour);
			laserSightRenderer.useWorldSpace = true;

			laserSightRenderer.gameObject.SetActive(bLaserSightActive);
		}
	}
	
	// Update 
	void Update ()
    {
		if (laserSightRenderer == null)
		{
			return;
		}

		// Raycast laser to nearest collidable object
		RaycastHit2D hit = Physics2D.Raycast(laserSightRenderer.transform.position, laserSightRenderer.transform.right, 100, laserSightLayerMask);

		// Set start point
		laserSightRenderer.SetPosition(0, laserSightRenderer.transform.position);

		// Set end point depending on if the laser hit something
		if (hit.collider != null)
		{
			laserSightRenderer.SetPosition(1, hit.point);
		}
		else
		{
			laserSightRenderer.SetPosition(1, laserSightRenderer.transform.position + laserSightRenderer.transform.right * 100);
		}
	}
		
	// Set Laser Sight Active
	public void SetLaserSightActive(bool active)
	{
        if (GlobalSettings.iLaserVisible == 1)
        {
            laserSightRenderer.gameObject.SetActive(false);
            bLaserSightActive = false;
            return;
        }

        if (bLaserSightActive == active)
        {
            return;
        }

		bLaserSightActive = active;

		if (laserSightRenderer != null)
		{
			laserSightRenderer.gameObject.SetActive(active);

			if (active)
			{
				SetLaserColour(laserSightColour);
				Update();
			}
		}
	}

	// Set Laser Fade Time
	public void SetFadeTime(float time)
	{
		if (time < 0)
		{
			time = 0;
		}

		fFadeTime = time;
	}

	// Sets the laser sight colour
	private void SetLaserColour(Color colour)
	{
		laserSightRenderer.startColor = colour;
		laserSightRenderer.endColor = colour;
	}

	// Starts the Laser Fade Transition
	public void StartLaserFade()
	{
		if (!laserSightRenderer.gameObject.activeInHierarchy)
		{
			return;
		}

		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}

		fadeCoroutine = StartCoroutine(FadeOut(true));
	}

	// Enables laser sight
	public void EnableLaser(bool enabled)
	{
		if (!laserSightRenderer.gameObject.activeInHierarchy)
		{
			return;
		}

		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}

		if (enabled)
		{
			fadeCoroutine = StartCoroutine(FadeIn());
		}
		else
		{
			fadeCoroutine = StartCoroutine(FadeOut());
		}
	}

	// Fade Laser Out
	private IEnumerator FadeOut(bool fadeOnFinish = false)
	{
		fCurrentFadeTime = 0f;
		Color clearColour = laserSightColour;
		clearColour.a = fFadeAlpha;

		while (fCurrentFadeTime < fFadeTime)
		{
			SetLaserColour(Color.Lerp(laserSightRenderer.startColor, clearColour, fCurrentFadeTime / fFadeTime));
			fCurrentFadeTime += Time.deltaTime;
		
			yield return null;
		}

		if (fadeOnFinish)
		{
			fadeCoroutine = StartCoroutine(FadeIn());
		}
	}

	// Fade Laser In
	private IEnumerator FadeIn()
	{
		fCurrentFadeTime = 0f;

		while (fCurrentFadeTime < fFadeTime)
		{
			SetLaserColour(Color.Lerp(laserSightRenderer.startColor, laserSightColour, fCurrentFadeTime / fFadeTime));
			fCurrentFadeTime += Time.deltaTime;

			yield return null;
		}
	}
}
