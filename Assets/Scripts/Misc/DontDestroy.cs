using UnityEngine;

public class DontDestroy : MonoBehaviour
{
	// Initialization
	void Start ()
	{
		DontDestroyOnLoad(gameObject);
	}
}
