using UnityEngine;
using UnityEngine.UI;

public class DebugTestGameCounter : MonoBehaviour
{
	public Text counterText;

	// Initialization
	void Start ()
	{
	}
	
	// Update
	void Update ()
	{
		counterText.text = "Counter: " + GlobalVariables.iTestCounter;

		if (Input.GetKeyDown(KeyCode.B))
		{
			GlobalVariables.iTestCounter++;
		}
	}
}
