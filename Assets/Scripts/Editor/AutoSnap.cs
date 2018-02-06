using UnityEngine;
using UnityEditor;

public class AutoSnap : EditorWindow
{
	private Vector3 prevPosition;
	private bool doSnap = true;
	private float snapValue = 1;

	[MenuItem("Edit/Auto Snap %_l")]

	static void Init()
	{
		var window = (AutoSnap)EditorWindow.GetWindow(typeof(AutoSnap));
		window.maxSize = new Vector2(200, 100);
	}

	public void OnGUI()
	{
		doSnap = EditorGUILayout.Toggle("Auto Snap", doSnap);
		snapValue = EditorGUILayout.FloatField("Snap Value", snapValue);
	}

	public void Update()
	{
		if (doSnap
		  && !EditorApplication.isPlaying
		  && Selection.transforms.Length > 0
		  && Selection.transforms[0].position != prevPosition)
		{
			Snap();
			prevPosition = Selection.transforms[0].position;
		}
	}

	private void Snap()
	{
		foreach (var transform in Selection.transforms)
		{
			RectTransform rectTrans = transform.transform as RectTransform;
			if (rectTrans)
			{
				var pos = rectTrans.anchoredPosition;
				pos.x = Round(pos.x);
				pos.y = Round(pos.y);
				rectTrans.anchoredPosition = pos;
				var size = rectTrans.sizeDelta;
				size.x = Round(size.x);
				size.y = Round(size.y);
				rectTrans.sizeDelta = size;
			}
			else
			{
				var t = transform.transform.position;
				t.x = Round(t.x);
				t.y = Round(t.y);
				t.z = Round(t.z);
				transform.transform.position = t;
			}
		}
	}

	private float Round(float input)
	{
		return snapValue * Mathf.Round((input / snapValue));
	}
}