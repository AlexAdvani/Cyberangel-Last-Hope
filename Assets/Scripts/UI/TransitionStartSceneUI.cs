using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;

public class TransitionStartSceneUI : MonoBehaviour
{
	// Initialization
	void Start ()
	{
		TransitionHelper.TransitionIn(gameObject);
	}
}
