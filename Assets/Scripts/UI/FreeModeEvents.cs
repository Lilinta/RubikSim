using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI events for the Free Mode scene.
/// Manages transitions between the main cube view, the import overlay, and the solver playback view.
/// </summary>
public class FreeModeEvents : MonoBehaviour
{
    public GameManager game_manager;
    public SolverManager solver_manager;
    public GameObject import_container;
    public TouchSurfaces touch_surfaces;
    private UIDocument document;
    
    private VisualElement container;
    private Button return_button;
    private Button import_button;
    private Button scramble_button;
    private Button solve_button;

    private VisualElement solver_container;
    private Button solver_return_button;
    private Button solver_backward_button;
    private Button solver_forward_button;

    private bool is_solvable = true;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        container = document.rootVisualElement.Q("Container") as VisualElement;
        return_button = document.rootVisualElement.Q("ReturnButton") as Button;
        import_button = document.rootVisualElement.Q("ImportButton") as Button;
        scramble_button = document.rootVisualElement.Q("ScrambleButton") as Button;
        solve_button = document.rootVisualElement.Q("SolveButton") as Button;
        
        solver_container = document.rootVisualElement.Q("SolverContainer") as VisualElement;
        solver_return_button = document.rootVisualElement.Q("SolverReturnButton") as Button;
        solver_backward_button = document.rootVisualElement.Q("SolverBackwardButton") as Button;
        solver_forward_button = document.rootVisualElement.Q("SolverForwardButton") as Button;

        return_button.RegisterCallback<ClickEvent>(OnReturnClick);
        import_button.RegisterCallback<ClickEvent>(OnImportClick);
        scramble_button.RegisterCallback<ClickEvent>(OnScrambleClick);
        solve_button.RegisterCallback<ClickEvent>(OnSolveClick);

        solver_return_button.RegisterCallback<ClickEvent>(OnSolverReturnClick);
        solver_backward_button.RegisterCallback<ClickEvent>(OnSolverBackwardClick);
        solver_forward_button.RegisterCallback<ClickEvent>(OnSolverForwardClick);

        import_container.SetActive(false);
    }

    private void OnDisable()
    {
        return_button.UnregisterCallback<ClickEvent>(OnReturnClick);
        import_button.UnregisterCallback<ClickEvent>(OnImportClick);
        scramble_button.UnregisterCallback<ClickEvent>(OnScrambleClick);
        solve_button.UnregisterCallback<ClickEvent>(OnSolveClick);

        solver_return_button.UnregisterCallback<ClickEvent>(OnSolverReturnClick);
        solver_backward_button.UnregisterCallback<ClickEvent>(OnSolverBackwardClick);
        solver_forward_button.UnregisterCallback<ClickEvent>(OnSolverForwardClick);
    }

    /// <summary>
    /// Checks the current state solvability and updates the Solve button label accordingly.
    /// </summary>
    public void IsSolvable()
    {
        if (!game_manager.IsSolvable())
        {
            is_solvable = false;
            solve_button.text = "Unsolvable!";
        } else
        {
            is_solvable = true;
            solve_button.text = "Solve";
        }
    }

    private void OnReturnClick(ClickEvent evt)
    {
        Debug.Log("You pressed Return button");
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnImportClick(ClickEvent evt)
    {
        Debug.Log("You pressed Import button");
        touch_surfaces.enable = false;
        import_container.SetActive(true);
        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnScrambleClick(ClickEvent evt)
    {
        Debug.Log("You pressed Scramble button");
        game_manager.Scramble();
    }

    private void OnSolveClick(ClickEvent evt)
    {
        Debug.Log("You pressed Solve button");
        if (!is_solvable) return;
        touch_surfaces.enable = false;
        container.style.display = DisplayStyle.None;
        solver_container.style.display = DisplayStyle.Flex;
        solver_manager.Init();
    }

    private void OnSolverReturnClick(ClickEvent evt)
    {
        Debug.Log("You pressed Solver Return button");
        solver_container.style.display = DisplayStyle.None;
        container.style.display = DisplayStyle.Flex;
        touch_surfaces.enable = true;
    }

    private void OnSolverBackwardClick(ClickEvent evt)
    {
        Debug.Log("You pressed Solver Backward button");
        solver_manager.Backward();
    }

    private void OnSolverForwardClick(ClickEvent evt)
    {
        Debug.Log("You pressed Solver Forward button");
        solver_manager.Forward();
    }
}
