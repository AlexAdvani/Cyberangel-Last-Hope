using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class CameraBoundaryManager : MonoBehaviour
{
    // Pro Camera
    ProCamera2D proCamera;

    // Left Collider
    public BoxCollider2D leftCollider;
    // Right Collider
    public BoxCollider2D rightCollider;
    // Up Collider
    public BoxCollider2D upCollider;
    // Down Collider
    public BoxCollider2D downCollider;

    // Bounds Offset
    public float fBoundsOffset = 2.5f;

    // Initialization
    void Start ()
    {
        proCamera = Camera.main.GetComponent<ProCamera2D>();
        proCamera.OnCameraResize += ResizeBoundaries;

        ResizeBoundaries(proCamera.ScreenSizeInWorldCoordinates);
	}

    // Update
    void Update()
    {
        transform.position = proCamera.transform.position;
    }

    // Resize the Camera Boundaries
    private void ResizeBoundaries(Vector2 cameraSize)
    {
        leftCollider.transform.localPosition = new Vector2(-cameraSize.x / 2- fBoundsOffset, 0);
        leftCollider.transform.localScale = new Vector2(1, cameraSize.y + fBoundsOffset * 2);

        rightCollider.transform.localPosition = new Vector2(cameraSize.x / 2  + fBoundsOffset, 0);
        rightCollider.transform.localScale = new Vector2(1, cameraSize.y + fBoundsOffset * 2);

        upCollider.transform.localPosition = new Vector2(0, cameraSize.y / 2 + fBoundsOffset);
        upCollider.transform.localScale = new Vector2(cameraSize.x + fBoundsOffset * 2, 1);

        downCollider.transform.localPosition = new Vector2(0, -cameraSize.y / 2 - fBoundsOffset);
        downCollider.transform.localScale = new Vector2(cameraSize.x + fBoundsOffset * 2, 1);
    }
}
