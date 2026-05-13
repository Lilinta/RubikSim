using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles user interaction with the cube using Raycasting.
/// Detects clicks on specific "TouchQuad" colliders and translates them 
/// into logical cube moves.
/// </summary>
public class TouchSurfaces : MonoBehaviour
{
    public GameManager manager;
    public bool enable = true;
    
    void Update()
    {
        if (!enable) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.name.StartsWith("TouchQuad"))
                {
                    int face = hit.collider.name[^1] - '0';
                    Debug.Log("face: " + face.ToString());
                    manager.ApplyMove(new Move(face, Keyboard.current.shiftKey.isPressed, false));
                }
            }
        }
    }
}
