using UnityEngine;

public class PlayerSpotlightBehaviour : MonoBehaviour
{
    // Player Transform
    Transform playerTransform;
    // Position Offset
    public Vector3 v3PositionOffset;

	// Initialization
	void Start ()
    {
        playerTransform = GameManager.Player.transform;

        if (playerTransform == null)
        {
            Destroy(gameObject);
        }
	}
	
	// Update
	void Update ()
    {
        transform.position = playerTransform.position + v3PositionOffset;
	}
}
