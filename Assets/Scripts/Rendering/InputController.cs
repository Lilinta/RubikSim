using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Temporary input handler used for testing cube interactions via keyboard.
/// Only function when pkey is pressed.
/// 
/// This controller maps specific keyboard keys to Rubik’s Cube face moves
/// and forwards them to <see cref="GameManager"/> for execution.
/// It is designed for rapid debugging and development before the final
/// mouse-based gesture input system is implemented.
///
/// Key mappings:
///     F or 0 → Front face
///     U or 1 → Up face
///     R or 2 → Right face
///     B or 3 → Back face
///     D or 4 → Down face
///     L or 5 → Left face
///
/// Holding the Shift key while pressing a face key will apply the move
/// in reverse (counter–clockwise).
///
/// Pressing:
///     K → Triggers a scramble sequence.
///     S → Triggers a solving sequence
///     Q → Debug Output
///
/// This script uses Unity's new Input System (<see cref="UnityEngine.InputSystem"/>)
/// and checks for key presses every frame in <see cref="Update"/>.
/// 
/// Note:
/// This controller is intended only for development and testing purposes.
/// In the final version of the application, cube manipulation will be driven
/// by mouse drag gestures and raycasting instead of direct keyboard input.
/// </summary>
public class InputController : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        if (!Keyboard.current.pKey.isPressed) return;
        if (Keyboard.current.fKey.wasPressedThisFrame || Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(0, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.uKey.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(1, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.rKey.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(2, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.bKey.wasPressedThisFrame || Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(3, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(4, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.lKey.wasPressedThisFrame || Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            gameManager.ApplyMove(new Move(5, Keyboard.current.shiftKey.isPressed));
        }
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            gameManager.Scramble();
        }
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            gameManager.Solve();
        }
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            gameManager.DebugOutput();
        }
    }
}