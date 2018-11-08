using System.Collections.Generic;

using UnityEngine;

using Rewired;

[CreateAssetMenu(menuName = "Generic Game Data/Input Element To Image Data")]
public class InputElementToImageData : ScriptableObject
{
	public InputElementGroup keyInputs;
	public InputElementGroup mouseInputs;
	public List<InputElementGroup> lPadInputs;

	[Inspectionary("PS Input", "Element ID")]
	public InputPS3ElementConvertDictionary dPS4InputConverts;

	public Sprite GetImage(ControllerType controlType, ControllerElementType elementType, int elementID, AxisRange range = AxisRange.Full, Pole axis = Pole.Positive)
	{
		Sprite image = null;
		int controllerPlatform = 0;

		if (controlType == ControllerType.Joystick)
		{
			if (lPadInputs.Count == 0)
			{
				return null;
			}

            if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic)
            {
                if (ControllerStatusManager.nativeGamepadType == eGamepadButtonType.PlayStation3 ||
                    ControllerStatusManager.nativeGamepadType == eGamepadButtonType.PlayStation4)
                {
                    elementID = dPS4InputConverts[elementID];
                }
            }

            // Xbox Controls
            if ((int)ControllerStatusManager.currentGamepadType <= 2)
			{
				controllerPlatform = 0;
			}
			else if ((int)ControllerStatusManager.currentGamepadType < 5) // PS Controls
			{
				controllerPlatform = 1;
			}
		}
				
		switch (controlType)
		{
			case ControllerType.Keyboard:
				image = keyInputs.GetElement(ControllerElementType.Button, elementID);
			break;

			case ControllerType.Mouse:
				image = mouseInputs.GetElement(elementType, elementID, range);
			break;

			case ControllerType.Joystick:
				image = lPadInputs[controllerPlatform].GetElement(elementType, elementID, range, axis);
			break;
		}

		return image;
	}

    public Sprite GetImageAxis2D(ControllerType controlType, int elementID)
    {
        if (controlType != ControllerType.Joystick)
        {
            return null;
        }

        int controllerPlatform = -1;
        
        // Xbox Controls
        if ((int)ControllerStatusManager.currentGamepadType <= 2)
        {
            controllerPlatform = 0;
        }
        else if ((int)ControllerStatusManager.currentGamepadType < 5) // PS Controls
        {
            controllerPlatform = 1;
        }

        if (controllerPlatform == -1)
        {
            return null;
        }

        return lPadInputs[controllerPlatform].GetAxis2D(elementID);
    }
}

[System.Serializable]
public class InputElementGroup
{
	public string sName;
	public ControllerType controlType;

	[Inspectionary("Element Identifier", "Button")]
	public InputButtonSpriteDictionary dButtons;
	[Inspectionary("Element Identifier", "Axis")]
	public InputAxisDictionary dAxes;
    [Inspectionary("Element Identifier", "Axis 2D")]
    public InputButtonSpriteDictionary dAxes2D;

	public Sprite GetElement(ControllerElementType elementType, int elementID, AxisRange range = AxisRange.Full, Pole axis = Pole.Positive)
	{
		if (elementType == ControllerElementType.Axis)
		{
			return GetAxis(elementID, range, axis);
		}
		else if (elementType == ControllerElementType.Button)
		{
			return GetButton(elementID);
		}

		return null;
	}

	private Sprite GetButton(int elementID)
	{
		if (!dButtons.ContainsKey(elementID))
		{
			return null;
		}

		return dButtons[elementID];
	}

	private Sprite GetAxis(int elementID, AxisRange range, Pole axis)
	{
		if (!dAxes.ContainsKey(elementID))
		{
			return null;
		}

		return dAxes[elementID].GetImage(range, axis);
	}

    public Sprite GetAxis2D(int elementID)
    {
        if (!dAxes2D.ContainsKey(elementID))
        {
            return null;
        }

        return dAxes2D[elementID];
    }
}

[System.Serializable]
public struct InputElementAxis
{
	public Sprite fullAxis;
	public Sprite posAxis;
	public Sprite negAxis;

	public Sprite GetImage(AxisRange range, Pole axis)
	{
		Sprite image = null;

		if (range == AxisRange.Full)
		{
			image = fullAxis;
		}
		else
		{
			if (axis == Pole.Positive)
			{
				image = posAxis;
			}
			else
			{
				image = negAxis;
			}
		}

		return image;
	}
}

[System.Serializable]
public class InputButtonSpriteDictionary : SerializableDictionary<int, Sprite> { }

[System.Serializable]
public class InputAxisDictionary : SerializableDictionary<int, InputElementAxis> { }

[System.Serializable]
public class InputPS3ElementConvertDictionary : SerializableDictionary<int, int> { }

[System.Serializable]
public class InputPS4ElementConvertDictionary : SerializableDictionary<int, int> { }