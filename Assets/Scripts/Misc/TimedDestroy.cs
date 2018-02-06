using System.Collections;

using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
	public float fLifeTime = 1;
	public bool bDisableOnDestroy = false;

	// Initialization
	void OnEnable ()
	{
		StartCoroutine(DestroyObject());
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(fLifeTime);

		if (bDisableOnDestroy)
		{
			gameObject.SetActive(false);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
