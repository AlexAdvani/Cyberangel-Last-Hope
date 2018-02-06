﻿using UnityEngine;
using UnityEngine.UI;

using Rewired;

public class CrosshairUIBehaviour : MonoBehaviour
{
    // Rect Transform
    RectTransform rectTransform;
    // Crosshair Image
    Image image;
    // Player Input
    Player playerInput;

    // Current Target Colour
    Color currentColour = Color.white;
    // Highlight colour (When hovering over enemies/targets)
    public Color cHighlightColour;
    // Crosshair highlight layer mask
    public LayerMask highlightMask;

    // Current Target Alpha
    float fCurrentAlpha = 1;
    // Alpha Transparency when crosshair is set to semi visible
    public float fSemiVisibleAlpha = 0.5f;

    // Visible flag
    bool bVisible = true;
    // Active flag
    bool bActive = true;

    #region Public Properties

    // Active flag
    public bool Active
    {
        get { return bActive; }
    }

    #endregion

    // Initialization
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        playerInput = ReInput.players.GetPlayer(0);

        GameManager.onPause += DisableCrosshair;
        GameManager.onUnpause += EnableCrosshair;

        if (GameManager.GamePaused || !GameManager.Instance.InGame)
        {
            DisableCrosshair();
        }
        else
        {
            EnableCrosshair();
        }
    }

    // On Destroy
    void OnDestroy()
    {
        GameManager.onPause -= DisableCrosshair;
        GameManager.onUnpause -= EnableCrosshair;
    }

    // Update
    void Update ()
    {
        if (!bActive || GameManager.Player.PlayerControlDisabled)
        {
            return;
        }

        rectTransform.position = Input.mousePosition;

        HandleVisibility();

        if (GlobalSettings.iCrosshairVisibility == 3 ||
            ControllerStatusManager.currentControlType == ControllerType.Joystick)
        {
            return;
        }

        HandleHighlighting();
	}

    // Handles highlighting the crosshair if it is over a target
    private void HandleHighlighting()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, highlightMask);

        if (hit.collider != null)
        {
            SetColour(cHighlightColour);
        }
        else
        {
            SetColour(Color.white);
        }
    }

    // Sets the colour of the crosshair
    private void SetColour(Color newColour)
    {
        if (currentColour == newColour)
        {
            return;
        }

        currentColour = newColour;

        newColour.a = bVisible ? 1 : fCurrentAlpha;
        image.color = newColour;
    }

    // Handles the visibility of the crosshair based on player input
    private void HandleVisibility()
    {
        if (GlobalSettings.iCrosshairVisibility == 3 ||
            ControllerStatusManager.currentControlType == ControllerType.Joystick)
        {
            if (fCurrentAlpha != 0)
            {
                fCurrentAlpha = 0;
                image.color = Color.clear;
            }

            return;
        }
        else if (GlobalSettings.iCrosshairVisibility < 2 && fCurrentAlpha == 0)
        {
            bVisible = !bVisible;
        }

        if (playerInput.GetButton("Aim Mode"))
        {
            SetVisibility(true);
        }
        else
        {
            SetVisibility(false);
        }
    }

    // Sets the visibility of the crosshair
    public void SetVisibility(bool visible)
    {
        if (bVisible == visible)
        {
            return;
        }

        if (visible)
        {
            currentColour.a = 1;
        }
        else
        {
            switch (GlobalSettings.iCrosshairVisibility)
            {
                case 0: // Always Visible
                    fCurrentAlpha = 1;
                break;

                case 1: // Transparent when not aiming
                    fCurrentAlpha = fSemiVisibleAlpha;
                break;

                case 2: // Invisible when not aiming
                    fCurrentAlpha = 0;
                break;
            }

            currentColour.a = fCurrentAlpha;
        }

        image.color = currentColour;
        bVisible = visible;
    }

    // Enables the crosshair
    private void EnableCrosshair()
    {
        Cursor.visible = false;
        image.enabled = true;
        bActive = true;
    }

    // Disables the crosshair
    private void DisableCrosshair()
    {
        Cursor.visible = true;
        image.enabled = false;
        bActive = false;
    }
}