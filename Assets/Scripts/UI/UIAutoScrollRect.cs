using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

public class UIAutoScrollRect : MonoBehaviour
{
	// Scroll Rect
	ScrollRect scrollRect;
	// Currently Selected UI Object
	GameObject goCurrentSelectedObject = null;
	// Static Scroll Amount Relative to Scroll Objects
	static float fScrollAmount = 0.3f;

	// Objects Under Parent flag - If the selectable UI objects are children to the object to scroll by, check this
	public bool bObjectsUnderParent;

	// Initialization
	void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
	}

	// Update 
	void Update()
	{
		// Current UI Object Selected
		GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

		// If object has not changed or not under the scroll rect content, return
		if (selectedObj != null)
		{
			if (selectedObj == goCurrentSelectedObject ||
				!(selectedObj.transform.IsChildOf(scrollRect.content)))
			{
				return;
			}
		}
		else
		{
			return;
		}

		// Set Current Object and Find RectTransform
		goCurrentSelectedObject = selectedObj;
		RectTransform selectedRect = selectedObj.GetComponent<RectTransform>();

		// Content Height
		float contentHeight = scrollRect.content.rect.height;
		// Viewport Height
		float viewportHeight = scrollRect.viewport.rect.height;

		// Object Center Line
		float objCenter;

		if (bObjectsUnderParent)
		{
			objCenter = selectedObj.transform.parent.localPosition.y;
		}
		else
		{
			objCenter = selectedObj.transform.localPosition.y;
		}

		// Object Lower and Upper Bounds
		float objLowerBound = objCenter - selectedRect.rect.height / 2f; ;
		float objUpperBound = objCenter + selectedRect.rect.height / 2f;
		
		// Viewport Upper and Lower Bounds
		float viewLowerBound = (contentHeight - viewportHeight) * scrollRect.normalizedPosition.y - contentHeight;
		float viewUpperBound = viewLowerBound + viewportHeight;

		// Desired Lower Bounds for Scrolling
		float desiredLowerBound;

		// If object above viewport
		if (objUpperBound > viewUpperBound)
		{
			desiredLowerBound = objUpperBound - viewportHeight + selectedRect.rect.height * fScrollAmount;
		}
		else if (objLowerBound < viewLowerBound) // If objects below view port
		{
			desiredLowerBound = objLowerBound - selectedRect.rect.height * fScrollAmount;
		}
		else // In viewport, return
		{
			return;
		}

		// Normalized Desired Scroll Position
		float normalizedDesired = (desiredLowerBound + contentHeight) / (contentHeight - viewportHeight);
		scrollRect.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
	}
}
