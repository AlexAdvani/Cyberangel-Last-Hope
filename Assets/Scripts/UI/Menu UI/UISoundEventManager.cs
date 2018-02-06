using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISoundEventManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	// UI Control
	Selectable uiSelectable;
	// Screen Manager
	public UIMenuScreenManager screenManager;

	// Disable Highlight Sound flag
	public bool bDisableHighlightSound = false;
	// Disable Confirm Sound flag
	public bool bDisableConfirmSound = false;

	// Initialization
	void Awake ()
	{
		uiSelectable = GetComponent<Selectable>();	
	}

	// Pointer Enter Event
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (screenManager == null)
		{
			return;
		}

		if (bDisableHighlightSound)
		{
			return;
		}

		if (!uiSelectable.interactable || EventSystem.current.currentSelectedGameObject == uiSelectable.gameObject)
		{
			return;
		}

		screenManager.MouseHighlightSound();
	}

	// On Click Event
	public void OnPointerClick(PointerEventData eventData)
	{
		if (screenManager == null)
		{
			return;
		}

		if (bDisableConfirmSound)
		{
			return;
		}

		if (!uiSelectable.interactable)
		{
			return;
		}

		screenManager.PlayConfirmSound();
	}
}
