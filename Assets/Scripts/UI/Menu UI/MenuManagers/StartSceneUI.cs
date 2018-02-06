using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.Components;

public class StartSceneUI : MonoBehaviour
{
	public CanvasRenderer[] aCanvasRenderers;
	public GameObject goObjFadeUI;
	public GameObject goScreenFadeUI;
	int iCurrentRenderer = 0;

	// Initialization
	void Start()
	{
		for (int i = 0; i < aCanvasRenderers.Length; i++)
		{
			if (i == 0)
			{
				aCanvasRenderers[i].gameObject.SetActive(true);
				continue;
			}

			aCanvasRenderers[i].gameObject.SetActive(false);
		}

		FadeIn();
	}

	// Fade Screen In
	private void FadeIn()
	{
	    TransitionHelper.TransitionIn(goObjFadeUI, FadeOut);
	}

	// Fade Screen Out
	private void FadeOut()
	{
		TransitionHelper.TransitionOut(goObjFadeUI, IncrementRenderers);
	}

	// Move on to the next canva renderer
	private void IncrementRenderers()
	{
		aCanvasRenderers[iCurrentRenderer].gameObject.SetActive(false);
		iCurrentRenderer++;

		if (iCurrentRenderer >= aCanvasRenderers.Length)
		{
			TransitionManager.Instance.TransitionOutAndLoadScene("MainMenu", goScreenFadeUI);
			return;
		}

		aCanvasRenderers[iCurrentRenderer].gameObject.SetActive(true);

		FadeIn();
	}
}