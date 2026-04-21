using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls camera orbit and zoom around a target point using Unity's New Input System.
///
/// Features:
/// - Mouse scroll to zoom in/out by adjusting camera field of view (FOV)
/// - Right mouse drag to orbit the camera around a target position
/// - Camera always looks at the target after movement
///
/// This script is purely for scene navigation and does not interact
/// with the Rubik's Cube logic.
/// </summary>
public class CameraController : MonoBehaviour
{
    public Camera cam;
    public float rotate_sensitivity = 100f;
    public float zoom_sensitivity = 300f;
    public Vector3 target = Vector3.zero;
    public float minZoom = 5f;
    public float maxZoom = 70f;

    /// <summary>
    /// Handles per-frame camera controls:
    /// - Zooming with mouse scroll
    /// - Orbiting with right mouse drag
    /// - Maintaining camera focus on the target
    /// </summary>
    void Update()
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y != 0)
        {
            cam.fieldOfView -= scroll.y * zoom_sensitivity * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }
        if (Mouse.current.rightButton.IsPressed())
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            transform.RotateAround(target, Vector3.up, delta.x*rotate_sensitivity*Time.deltaTime);
            transform.RotateAround(target, transform.right, -delta.y*rotate_sensitivity*Time.deltaTime);
        }
        transform.LookAt(target);
    }
}
