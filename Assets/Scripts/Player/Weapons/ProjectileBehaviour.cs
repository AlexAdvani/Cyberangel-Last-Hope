using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBehaviour : MonoBehaviour
{
	// Rigidbody 2D
	Rigidbody2D rigidbody2d;

	// Speed
	public float fSpeed;
	// Damage
	public float fDamage;

	// Time until projectle destroys itself
	public float fLifeTime;

    // Collision Layer Mask
    public LayerMask collisionMask;

	// Impact Audio
	public SimpleAudioEvent impactAudio;

	// Initialization
	void Awake()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
	}

	// On Enable
	void OnEnable ()
	{
		StartCoroutine(LifetimeDestroy());
	}

    // Fixed Update
    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, rigidbody2d.velocity.magnitude, collisionMask, 0);

        if (hit.collider != null)
        {
            CollisionResult(hit.collider.gameObject);
        }
    }

    // On Collision Enter
    void OnCollisionEnter2D(Collision2D other)
	{
		CollisionResult(other.gameObject);
	}

	// Trigger Enter 
	void OnTriggerEnter2D(Collider2D other)
	{
		CollisionResult(other.gameObject);
	}

	// Launches Projectile
	public virtual void LaunchProjectile(float speedMultiplier = 1)
	{
		rigidbody2d.velocity = transform.up * fSpeed * speedMultiplier;
	}

    // Sets the damage for the projectile
    public virtual void SetDamage(float damage)
    {
        fDamage = damage;
    }

	// Looks for Health Manager on collided object to take health then destroys this gameobject
	private void CollisionResult(GameObject other)
	{
		HealthManager healthManager = other.GetComponent<HealthManager>();

		if (healthManager != null)
		{
			healthManager.TakeHealth(fDamage);
		}

		if (other.layer == LayerMask.NameToLayer("Environment"))
		{
			impactAudio.Play();
		}

		DestroyProjectile();
	}

	// Destroy Bullet After Set Time
	private IEnumerator LifetimeDestroy()
	{
		yield return new WaitForSeconds(fLifeTime);

		DestroyProjectile();
	}

	// Destroys the projectile
	private void DestroyProjectile()
	{
		StopAllCoroutines();
        gameObject.SetActive(false);
	}
}
