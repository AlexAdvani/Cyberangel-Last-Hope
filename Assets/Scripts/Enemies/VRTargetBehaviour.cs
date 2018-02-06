using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(HealthManager))]
public class VRTargetBehaviour : MonoBehaviour
{
	// Health Manager
	HealthManager healthManager;

	// Impact Audio
	public SimpleAudioEvent impactAudio;

	// Initialization
	void Start ()
	{
		healthManager = GetComponent<HealthManager>();
		healthManager.onKill += DestroyTarget;
	}

	// On Trigger Enter 2D
	//void OnTriggerEnter2D(Collider2D collision)
	//{
	//	if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
	//	{
	//		impactAudio.Play();
	//	}
	//}

	// Destroys the Target and Decrements the Target Counter
	private void DestroyTarget()
	{
		VRMissionModeManager.Instance.DecrementTargets();
		gameObject.SetActive(false);
	}
}
