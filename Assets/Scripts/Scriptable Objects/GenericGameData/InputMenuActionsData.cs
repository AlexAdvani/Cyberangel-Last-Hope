using UnityEngine;

using Rewired;

[CreateAssetMenu(menuName = "Generic Game Data/Input Menu Actions Data")]
public class InputMenuActionsData : ScriptableObject
{
    // Input Action Array
    public InputAction[] aInputActions;
}

// Input Action Data
[System.Serializable]
public class InputAction
{
    // Name
    public string sName;
    // Action ID
    public int iActionID;
    // Axis Range
    public AxisRange axisRange;
    // Axis Direction
    public Pole axisDirection;
}