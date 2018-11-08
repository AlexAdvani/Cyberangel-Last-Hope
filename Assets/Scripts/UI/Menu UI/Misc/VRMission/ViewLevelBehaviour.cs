using UnityEngine;
using UnityEngine.UI;

using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;


public class ViewLevelBehaviour : MonoBehaviour
{
    // Player Input
    Player playerInput;

    // Pan Arrow Left
    public Image panArrowLeft;
    // Pan Arrow Right
    public Image panArrowRight;
    // Pan Arrow Up
    public Image panArrowUp;
    // Pan Arrow Down
    public Image panArrowDown;

    // Previous Pan Delta
    Vector2 v2PrevPanDelta;

    // PC Input Panel
    public GameObject goPCInputPanel;
    // Gamepad Input Panel
    public GameObject goGamepadInputPanel;

    // Pan Input Legend
    public UIInputLegendImage panInputLegend;
    // Zoom Input Legend
    public UIInputLegendImage zoomInputLegend;
    // Return Input Legend
    public UIInputLegendImage returnInputLegend;

    // Pro Camera Pan and Zoom Extension
    ProCameraPanAndZoomPlus cameraPanAndZoom;
    // Pro Camera Rooms Extension
    ProCamera2DRooms cameraRooms;

    // Camera Triggers Parent Object
    GameObject goCameraTriggersParent;

    // Current Control Type
    ControllerType currentControlType;

    // Initialization
    void Awake ()
    {
        cameraPanAndZoom = Camera.main.GetComponent<ProCameraPanAndZoomPlus>();
        cameraRooms = Camera.main.GetComponent<ProCamera2DRooms>();
        goCameraTriggersParent = GameObject.FindGameObjectWithTag("CameraTrigger");
        playerInput = ReInput.players.GetPlayer(0);

        panArrowLeft.color = Colors.Gray;
        panArrowRight.color = Colors.Gray;
        panArrowUp.color = Colors.Gray;
        panArrowDown.color = Colors.Gray;

        CheckControlType();
	}

    // On Enable
    void OnEnable()
    {
        cameraPanAndZoom.enabled = true;
        cameraRooms.enabled = false;
        goCameraTriggersParent.SetActive(false);
    }

    // On Disable
    void OnDisable()
    {
        cameraPanAndZoom.enabled = false;
        cameraRooms.enabled = true;
        goCameraTriggersParent.SetActive(true);
    }

    // Update
    void Update ()
    {
        if (ControllerStatusManager.currentControlType != currentControlType)
        {
            CheckControlType();
        }

        HandleCameraInput();
        HandlePanArrows();
    }

    // Check the current control type and show the appropriate panel
    private void CheckControlType()
    {
        if (ControllerStatusManager.currentControlType == ControllerType.Keyboard ||
            ControllerStatusManager.currentControlType == ControllerType.Mouse)
        {
            goPCInputPanel.SetActive(true);
            goGamepadInputPanel.SetActive(false);
        }
        else
        {
            goPCInputPanel.SetActive(false);
            goGamepadInputPanel.SetActive(true);
        }

        currentControlType = ControllerStatusManager.currentControlType;
    }

    // Handle Camera Input
    private void HandleCameraInput()
    {
        cameraPanAndZoom.PanInput = playerInput.GetAxis2D("UIHorizontal", "UIVertical");
        cameraPanAndZoom.ZoomInput = playerInput.GetAxis("Zoom");
    }

    // Handle Pan Arrows
    private void HandlePanArrows()
    {
        if (cameraPanAndZoom.PanDelta == v2PrevPanDelta)
        {
            return;
        }

        Vector2 delta = cameraPanAndZoom.PanDelta;

        panArrowLeft.color = Colors.Gray;
        panArrowRight.color = Colors.Gray;
        panArrowUp.color = Colors.Gray;
        panArrowDown.color = Colors.Gray;

        if (delta.x > 0)
        {
            panArrowRight.color = Color.white;
        }
        else if (delta.x < 0)
        {
            panArrowLeft.color = Color.white;
        }

        if (delta.y > 0)
        {
            panArrowUp.color = Color.white;
        }
        else if (delta.y < 0)
        {
            panArrowDown.color = Color.white;
        }

        v2PrevPanDelta = delta;
    }
}
