using UnityEngine;
using UnityEngine.UI;
using static Mapping;
using UnityEngine.InputSystem;

/// <summary>
/// Attached to individual color buttons in the import UI.
/// Cycles through available colors on click; holding shift cycles backwards.
/// </summary>
public class FaceButtonEvents : MonoBehaviour
{
    public void OnFaceButtonClick()
    {
        GameObject button = gameObject;
        Debug.Log("You press " +  button.name);
        Image img = button.GetComponent<Image>();
        int idx = GetColorIdx(img.color);
        if (idx == -1)
        {
            img.color = GetColor(FRONT);
            return;
        }
        if (Keyboard.current.shiftKey.isPressed)
        {
            idx--;
        }
        else idx++;
        idx = (idx + 6) % 6;
        img.color = GetColor(idx);
    }
}
