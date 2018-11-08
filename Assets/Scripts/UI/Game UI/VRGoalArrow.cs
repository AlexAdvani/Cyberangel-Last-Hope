using UnityEngine;

public class VRGoalArrow : MonoBehaviour
{
    // Goal Arrow Image
    public RectTransform rtGoalArrow;
    
    // Goal Transform
    Transform tGoalTransform;
    // Height Offset for the Goal Arrow when pointing at the goal
    public float fGoalArrowHeightOffset = 2f;
    // Safe Zone Border from the edge of the screen (Between 0 and 1)
    public float fSafeZoneBorder = 0.1f;

	// Initialization
	void Awake ()
    {
        GameObject goal = GameObject.FindGameObjectWithTag("VRGoal");

        if (goal == null)
        {
            Debug.LogError("No VR Goal found. Make sure there is one in the scene with the tag: VRGoal");
            gameObject.SetActive(false);
        }

        tGoalTransform = goal.transform;
	}
	
	// Update
	void Update ()
    {
        Vector3 goalScreenPos = Camera.main.WorldToScreenPoint(tGoalTransform.position);

        if (goalScreenPos.x > 0 && goalScreenPos.x < Screen.width &&
            goalScreenPos.y > 0 && goalScreenPos.y < Screen.height)
        {
            rtGoalArrow.position = goalScreenPos + Vector3.up * fGoalArrowHeightOffset;
            rtGoalArrow.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            Vector3 camPos = Camera.main.transform.position;
            Vector3 diff = (tGoalTransform.position - camPos);
            diff.Normalize();

            rtGoalArrow.position = new Vector3(diff.x * fSafeZoneBorder * Screen.width / 2 + Screen.width / 2,
                diff.y * fSafeZoneBorder * Screen.height / 2 + Screen.height / 2, 0);
            rtGoalArrow.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        }
	}
}
