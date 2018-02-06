using UnityEngine;
using UnityEngine.SceneManagement;

using BeautifulTransitions.Scripts.Transitions.Components;

public class SceneLoadManager : SingletonBehaviour<SceneLoadManager>
{
	// Screen Fade UI GameObject
	public GameObject goScreenFadeUI;

	// Scene to Load in LoadingScene
	static string sSceneToLoad = "";

	// Scene To Load
	public static string SceneToLoad
	{
		get { return sSceneToLoad; }
	}

	// Load a Scene with a Screen Transition to the LoadingScene
	public void LoadScene(string sceneName)
	{
		if (!Application.CanStreamedLevelBeLoaded(sceneName))
		{
			Debug.LogError("Cannot load Scene. '" + sceneName + "' does not exist.");
			return;
		}

		Time.timeScale = 1;
		sSceneToLoad = sceneName;
		TransitionManager.Instance.TransitionOutAndLoadScene("LoadingScene", goScreenFadeUI);
    }

	// Load a Scene without a Screen Transition to the LoadingScene
	public void SetSceneWithoutLoad(string sceneName)
	{
		if (!Application.CanStreamedLevelBeLoaded(sceneName))
		{
			Debug.LogError("Cannot load Scene. '" + sceneName + "' does not exist.");
			return;
		}

        SceneManager.LoadScene(sceneName);
    }

	// Load a scene additively to the current scene
	public void LoadSceneAdditive(string sceneName)
	{
		if (!Application.CanStreamedLevelBeLoaded(sceneName))
		{
			Debug.LogError("Cannot load Scene. '" + sceneName + "' does not exist.");
			return;
		}

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
