using System.Collections;

using UnityEngine;

public class WeaponEjectBehaviour : MonoBehaviour
{
	// Sprite Renderer
	SpriteRenderer spriteRenderer;
	// Rigidbody 2D
	Rigidbody2D rigidbody2d;
    // Trail Renderer
    TrailRenderer trailRenderer;
    // Activate Trail Coroutine 
    Coroutine trailCoroutine;

    // Eject Force
    public Vector2 v2EjectForce;

	// Eject Torque
	public float fEjectTorque;

	// Impact Audio Clips
	public AudioEvent impactAudio;

	// Initialization
	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidbody2d = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
	}

	// On Enable
	void OnEnable()
	{
        if (transform.localPosition.z == -0.1f)
        {
            spriteRenderer.sortingOrder = 22;
        }
        else if (transform.localPosition.z == 0.1f)
        {
            spriteRenderer.sortingOrder = -5;
        }

        if (trailRenderer != null)
        {
            if (trailCoroutine != null)
            {
                StopCoroutine(trailCoroutine);
            }

            trailCoroutine = StartCoroutine(ActivateTrail());
        }

        ApplyEjectForce();
    }

    // On Disable
    void OnDisable()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    // Collision Enter 2D
    void OnCollisionEnter2D(Collision2D other)
	{
		if (impactAudio != null)
		{
			impactAudio.Play();
		}
	}

    // Activate Trail Renderer
    private IEnumerator ActivateTrail()
    {
        yield return new WaitForSeconds(0.05f);

        trailRenderer.enabled = true;
    }

	// Applies the Eject Force to the rigidbody
	private void ApplyEjectForce()
	{
		Vector2 force = v2EjectForce;
		float torque = fEjectTorque;

		force *= Random.Range(0.8f, 1.2f);
		torque *= Random.Range(0.8f, 1.2f);

		Vector2 rotatedForce = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector2.up) * force;

		if (transform.localScale.x == -1)
		{
			rotatedForce.x *= -1;
			torque *= -1;
		}

		rigidbody2d.velocity = GameManager.Player.Motor.velocity;

		rigidbody2d.AddForce(rotatedForce, ForceMode2D.Impulse);
		rigidbody2d.AddTorque(torque, ForceMode2D.Impulse);
	}
}
