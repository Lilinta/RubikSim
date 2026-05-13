using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI navigation for the Main Menu.
/// Switches between scenes based on user selection.
/// </summary>
public class MainMenuEvents : MonoBehaviour
{
    private UIDocument document;
    private Button challenge_mode_button;
    private Button free_mode_button;
    private Button multiplayer_mode_button;
    private Button exit_button;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        challenge_mode_button = document.rootVisualElement.Q("ChallengeModeButton") as Button;
        free_mode_button = document.rootVisualElement.Q("FreeModeButton") as Button;
        multiplayer_mode_button = document.rootVisualElement.Q("MultiplayerModeButton") as Button;
        exit_button = document.rootVisualElement.Q("ExitButton") as Button;

        challenge_mode_button.RegisterCallback<ClickEvent>(OnChallengeModeClick);
        free_mode_button.RegisterCallback<ClickEvent>(OnFreeModeClick);
        multiplayer_mode_button.RegisterCallback<ClickEvent>(OnMultiplayerModeClick);
        exit_button.RegisterCallback<ClickEvent>(OnExitClick);
    }

    private void OnDisable()
    {
        challenge_mode_button.UnregisterCallback<ClickEvent>(OnChallengeModeClick);
        free_mode_button.UnregisterCallback<ClickEvent>(OnFreeModeClick);
        multiplayer_mode_button.UnregisterCallback<ClickEvent>(OnMultiplayerModeClick);
        exit_button.UnregisterCallback<ClickEvent>(OnExitClick);
    }

    private void OnChallengeModeClick(ClickEvent evt)
    {
        Debug.Log("You pressed Challenge Mode button");
        SceneManager.LoadScene("ChallengeModeScene");
    }

    private void OnFreeModeClick(ClickEvent evt)
    {
        Debug.Log("You pressed Free Mode button");
        SceneManager.LoadScene("FreeModeScene");
    }

    private void OnMultiplayerModeClick(ClickEvent evt)
    {
        Debug.Log("You pressed Multiplayer Mode button");
    }

    private void OnExitClick(ClickEvent evt)
    {
        Debug.Log("You pressed Exit button");
        Application.Quit();
    }
}
