using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Manages UI interactions for the Challenge Mode (UI Toolkit).
/// </summary>
public class ChallengeModeEvents : MonoBehaviour
{
    private UIDocument document;
    private Button return_button;
    private Label timer_label;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        return_button = document.rootVisualElement.Q("ReturnButton") as Button;
        timer_label = document.rootVisualElement.Q("TimerLabel") as Label;

        return_button.RegisterCallback<ClickEvent>(OnReturnClick);
    }

    private void OnDisable()
    {
        return_button.UnregisterCallback<ClickEvent>(OnReturnClick);
    }

    private void OnReturnClick(ClickEvent evt)
    {
        Debug.Log("You pressed Return button");
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Updates the UI label with the formatted elapsed time.
    /// </summary>
    public void UpdateTimer(float time)
    {
        TimeSpan text = TimeSpan.FromSeconds((double)time);
        timer_label.text = text.ToString(@"mm\:ss\.ff");
    }
}
