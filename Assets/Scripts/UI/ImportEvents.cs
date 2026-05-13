using UnityEngine;
using UnityEngine.UIElements;

public class ImportEvents : MonoBehaviour
{
    public GameObject import_container;
    public TouchSurfaces touch_surfaces;
    public GameManager game_manager;
    public FreeModeEvents free_mode_events;
    public void OnCancelButtonClick()
    {
        Debug.Log("You pressed Cancel button");
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        import_container.SetActive(false);
        touch_surfaces.enable = true;
    }

    public void OnConfirmButtonClick()
    {
        Debug.Log("You pressed Confirm button");
        game_manager.Import();
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        import_container.SetActive(false);
        touch_surfaces.enable = true;
        free_mode_events.IsSolvable();
    }
}
