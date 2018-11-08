using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(HealthManager))]
public class VRTargetBehaviour : MonoBehaviour
{
    // Animator
    public Animator animator;

	// Health Manager
	HealthManager healthManager;

	// Impact Audio
	public SimpleAudioEvent impactAudio;
    // Destroy Audio
    public SimpleAudioEvent destroyAudio;

    // Ray Colliding flag
    bool bRayColliding = false;
    // Previous Frame Ray Colliding flag
    bool bPrevRayColliding = false;

    // Score Test flag - TEST
    public bool bScoreTest = false;

    // Score - TEST
    public int iScore = 100;

	// Initialization
	void Start ()
	{
		healthManager = GetComponent<HealthManager>();
		healthManager.onKill += DestroyTarget;
        healthManager.onRayCollision += RayCollision;
	}

    // Late Update
    void LateUpdate()
    {
        if (bRayColliding)
        {
            bPrevRayColliding = true;
            bRayColliding = false;
        }
        else if (bPrevRayColliding)
        {
            animator.SetBool("ContinuousHit", false);
            bPrevRayColliding = false;
        }
    }

    // On Collision Enter 2D
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            impactAudio.Play();

            animator.SetTrigger("Hit");
        }
    }

    // Ray Collision
    public void RayCollision()
    {
        if (!bRayColliding)
        {
            animator.SetBool("ContinuousHit", true);
            bRayColliding = true;
        }
    }

    // Destroys the Target and Decrements the Target Counter
    private void DestroyTarget()
	{
        bRayColliding = false;
        bPrevRayColliding = false;
        animator.SetBool("ContinuousHit", false);

        if (bScoreTest)
        {
            VRMissionModeManager.Instance.AddScore(iScore, true, true);
        }
        
        VRMissionModeManager.Instance.DecrementTargets();

        destroyAudio.Play();
        gameObject.SetActive(false);
	}
}
