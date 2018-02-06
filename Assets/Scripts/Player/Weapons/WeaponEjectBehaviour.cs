using UnityEngine;

public class WeaponEjectBehaviour : MonoBehaviour
{
	// Sprite Renderer
	SpriteRenderer spriteRenderer;
	// Rigidbody 2D
	Rigidbody2D rigidbody2d;

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

        ApplyEjectForce();
    }

	// Collision Enter 2D
	void OnCollisionEnter2D(Collision2D other)
	{
		if (impactAudio != null)
		{
			impactAudio.Play();
		}
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
