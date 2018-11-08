using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBehaviour : MonoBehaviour
{
	// Rigidbody 2D
	Rigidbody2D rigidbody2d;
    // Collider 2D
    Collider2D collider2d;

	// Speed
	public float fSpeed;
	// Damage
	public float fDamage;

	// Time until projectle destroys itself
	public float fLifeTime;

    // Collision Layer Mask
    public LayerMask collisionMask;
    // Previous Position
    Vector3 v3PrevPosition;

    // Max Length for the Bullet Trail
    public float fMaxBulletTrailLength;

	// Impact Audio
	public SimpleAudioEvent impactAudio;

    // Bullet Impact Prefab
    public GameObject goBulletImpact;

	// Initialization
	void Awake()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        v3PrevPosition = transform.position;
    }

	// On Enable
	void OnEnable ()
	{
		StartCoroutine(LifetimeDestroy());

        v3PrevPosition = transform.position;
    }

    // On Trigger Enter
    void OnTriggerEnter2D(Collider2D other)
	{
		CollisionResult(other.gameObject, other);
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.contacts[0];
        CollisionResult(other.gameObject, contact.point, contact.normal);
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
    private void CollisionResult(GameObject other, Vector3 point, Vector3 normal)
    { 
        HealthManager healthManager = other.GetComponent<HealthManager>();

        if (healthManager != null)
        {
            healthManager.TakeHealth(fDamage);
        }

        if (other.layer == LayerMask.NameToLayer("Environment") ||
            other.layer == LayerMask.NameToLayer("MovingPlatform"))
        {
            impactAudio.Play();

            Instantiate(goBulletImpact, point, Quaternion.Euler(normal));
        }

        DestroyProjectile();
	}

    // Collision Result Overload that takes a Collision2D
    private void CollisionResult(GameObject other, Collider2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.01f, collisionMask, 0);
        CollisionResult(other, hit.point, hit.normal);
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
