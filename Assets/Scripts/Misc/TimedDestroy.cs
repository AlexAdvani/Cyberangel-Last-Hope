using System.Collections;

using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
	public float fLifeTime = 1;
	public bool bDisableOnDestroy = false;

	// On Enable
	void OnEnable ()
	{
		StartCoroutine(DestroyObject());
	}

    // On Disable
    private void OnDisable()
    {
        StopAllCoroutines();
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
