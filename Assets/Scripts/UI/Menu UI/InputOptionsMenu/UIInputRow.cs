using UnityEngine;
using UnityEngine.UI;

using Rewired;

using TMPro;

public class UIInputRow : MonoBehaviour
{
    // Input Name Text
    public TextMeshProUGUI inputNameText;
    // Primary Button
    public Button primaryButton;
    // Input Primary Button Text
    public TextMeshProUGUI primaryButtonText;
    // Input Primary Button Icon
    public Image primaryButtonIcon;
    // Secondary Button
    public Button secondaryButton;
    // Input Secondary Button Text
    public TextMeshProUGUI secondaryButtonText;
    // Input Seconadry Button Icon
    public Image secondaryButtonIcon;

    // Primary Map Input Action ID
    int iPrimaryActionID;
    // Secondary Map Input Action ID
    int iSecondaryActionID;
    // Control Type
    ControllerType controlType;
    // Button Image Data
    InputElementToImageData buttonImageData;

    #region Public Attributes

    public int PrimaryActionID
    {
        get { return iPrimaryActionID; }
    }

    public int SecondaryActionID
    {
        get { return iSecondaryActionID; }
    }

    #endregion

    // Initialize Input Row
    public void InitializeRow(string actionName, ActionElementMap primaryMap, 
        ActionElementMap secondaryMap, int primaryActionID, int secondaryActionID,
        ControllerType mapControlType, InputElementToImageData buttonData, bool unassigned)
    {
        // Input Name 
        inputNameText.text = actionName;
        // Control Type
        controlType = mapControlType;
        // Button Image Data
        buttonImageData = buttonData;
        // Primary Action ID
        iPrimaryActionID = primaryActionID;
        // Secondary Action ID
        iSecondaryActionID = secondaryActionID;

        UpdateRow(primaryMap, secondaryMap, unassigned);
    }

    // Update Input Row
    public void UpdateRow(ActionElementMap primaryMap, ActionElementMap secondaryMap, bool unassigned)
    {
        // Gamepad Controls
        if (controlType == ControllerType.Joystick)
        {
            // Gamepad unassigned
            if (unassigned)
            {
                primaryButtonText.gameObject.SetActive(true);
                primaryButtonIcon.gameObject.SetActive(false);
                primaryButtonText.text = "None";
                primaryButton.interactable = false;

                secondaryButtonText.gameObject.SetActive(true);
                secondaryButtonIcon.gameObject.SetActive(false);
                secondaryButtonText.text = "None";
                secondaryButton.interactable = false;

                return;
            }

            // Non-Generic Gamepad
            if (ControllerStatusManager.currentGamepadType != eGamepadButtonType.Generic &&
                ControllerStatusManager.nativeGamepadType != eGamepadButtonType.Generic)
            {
                // Primary Map 
                if (primaryMap == null || primaryMap.elementIdentifierName == string.Empty)
                {
                    primaryButtonText.gameObject.SetActive(true);
                    primaryButtonIcon.gameObject.SetActive(false);
                    primaryButtonText.text = "None";
                }
                else
                {
                    primaryButtonText.gameObject.SetActive(false);
                    primaryButtonIcon.gameObject.SetActive(true);
                    primaryButtonIcon.sprite = buttonImageData.GetImage(controlType, primaryMap.elementType, primaryMap.elementIdentifierId,
                        primaryMap.axisRange, primaryMap.axisContribution);
                }

                if (primaryButtonIcon.sprite == null)
                {
                    primaryButtonText.gameObject.SetActive(true);
                    primaryButtonIcon.gameObject.SetActive(false);
                    primaryButtonText.text = "None";
                }

                // Secondary Map 
                if (secondaryMap == null || secondaryMap.elementIdentifierName == string.Empty ||
                    secondaryMap.elementIdentifierId == primaryMap.elementIdentifierId)
                {
                    secondaryButtonText.gameObject.SetActive(true);
                    secondaryButtonIcon.gameObject.SetActive(false);
                    secondaryButtonText.text = "None";
                }
                else
                {
                    secondaryButtonText.gameObject.SetActive(false);
                    secondaryButtonIcon.gameObject.SetActive(true);
                    secondaryButtonIcon.sprite = buttonImageData.GetImage(controlType, secondaryMap.elementType, secondaryMap.elementIdentifierId,
                        secondaryMap.axisRange, secondaryMap.axisContribution);
                }

                if (secondaryButtonIcon.sprite == null)
                {
                    secondaryButtonText.gameObject.SetActive(true);
                    secondaryButtonIcon.gameObject.SetActive(false);
                    secondaryButtonText.text = "None";
                }
            }
            else // Generic Controllers
            {
                primaryButtonText.gameObject.SetActive(true);
                primaryButtonIcon.gameObject.SetActive(false);

                secondaryButtonText.gameObject.SetActive(true);
                secondaryButtonIcon.gameObject.SetActive(false);

                // Primary Map 
                if (primaryMap == null || primaryMap.elementIdentifierName == string.Empty)
                {
                    primaryButtonText.text = "None";
                }
                else
                {
                    primaryButtonText.text = primaryMap.elementIdentifierName;
                }

                // Secondary Map 
                if (secondaryMap == null || secondaryMap.elementIdentifierName == string.Empty ||
                    secondaryMap.elementIdentifierId == primaryMap.elementIdentifierId)
                {
                    secondaryButtonText.text = "None";
                }
                else
                {
                    secondaryButtonText.text = secondaryMap.elementIdentifierName;
                }
            }
        }
        else // Key/Mouse Controls
        {
            string elementName = primaryMap.elementIdentifierName;

            if (controlType == ControllerType.Mouse)
            {
                if (elementName == "Left Mouse Button")
                {
                    elementName = "Left Click";
                }
                else if (elementName == "Right Mouse Button")
                {
                    elementName = "Right Click";
                }
            }

            primaryButtonText.gameObject.SetActive(true);
            secondaryButtonText.gameObject.SetActive(true);

            if (primaryButtonIcon != null)
            {
                primaryButtonIcon.gameObject.SetActive(false);
            }

            if (secondaryButtonIcon != null)
            {
                secondaryButtonIcon.gameObject.SetActive(false);
            }

            // Primary Map
            if (primaryMap == null || primaryMap.elementIdentifierName == string.Empty)
            {
                primaryButtonText.text = "None";
            }
            else
            {
                primaryButtonText.text = elementName;
            }

            elementName = secondaryMap.elementIdentifierName;

            if (controlType == ControllerType.Mouse)
            {
                if (elementName == "Left Mouse Button")
                {
                    elementName = "Left Click";
                }
                else if (elementName == "Right Mouse Button")
                {
                    elementName = "Right Click";
                }
            }

            // Secondary Map
            if (secondaryMap == null || secondaryMap.elementIdentifierName == string.Empty)
            {
                secondaryButtonText.text = "None";
            }
            else
            {
                if (controlType == ControllerType.Keyboard)
                {
                    if (secondaryMap.keyCode == primaryMap.keyCode)
                    {
                        secondaryButtonText.text = "None";
                    }
                    else
                    {
                        secondaryButtonText.text = elementName;
                    }
                }
                else
                {
                    if (secondaryMap.elementIdentifierId == primaryMap.elementIdentifierId)
                    {
                        secondaryButtonText.text = "None";
                    }
                    else
                    {
                        secondaryButtonText.text = elementName;
                    }
                }
            }
        }

        primaryButton.interactable = true;
        secondaryButton.interactable = true;
    }
}
