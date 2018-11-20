using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using BeautifulTransitions.Scripts.Transitions;

public class LoadingScreenManager : MonoBehaviour
{
	// Loading Async Operation
	AsyncOperation async;

	// Fade UI Game Object
	public GameObject goScreenFadeUI;
	// Min delay before changing to the next scene
	public float fLoadDelay = 1;

	// Transitioning flag
	bool bTransitioning = false;

	// Initialization
	void Start ()
	{
		StartCoroutine(LoadScene());
       
		AudioManager.Instance.DestroyAllAudioSources();

        if (ObjectPoolManager.Instance.PoolCount > 0)
        {
            ObjectPoolManager.Instance.ClearPools();
        }

        System.GC.Collect();
    }

	// Update
	void Update()
	{
		if (async != null && !bTransitioning)
		{
			if (async.progress == 0.9f)
			{
				TransitionHelper.TransitionOut(goScreenFadeUI, EnterScene);
				bTransitioning = true;
			}
		}
	}

	// Loads the next scene after a short delay
	private IEnumerator LoadScene()
	{
		yield return new WaitForSecondsRealtime(fLoadDelay);

		async = SceneManager.LoadSceneAsync(SceneLoadManager.SceneToLoad);
		async.allowSceneActivation = false;

		yield return async;
	}

	// Enters the next scene
	private void EnterScene()
	{
		async.allowSceneActivation = true;
	}
}
