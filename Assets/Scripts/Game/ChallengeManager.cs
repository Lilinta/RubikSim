using UnityEngine;

/// <summary>
/// Orchestrates the Challenge Mode experience:
/// - Handles initialization (scrambling the cube).
/// - Tracks player duration.
/// - Detects the solved state to end the challenge.
/// </summary>
public class ChallengeManager : MonoBehaviour
{
    public GameManager game_manager;
    public ChallengeModeEvents challenge_mode_events;
    public AudioSource celebration_sound;
    private float time = 0;
    bool finished = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game_manager.Scramble(false);
        challenge_mode_events.UpdateTimer(time);
    }

    // Update is called once per frame
    void Update()
    {
        if (!finished && game_manager.IsSolved())
        {
            finished = true;
            celebration_sound.Play();
        }
        if (!finished)
        {
            time += Time.deltaTime;
            challenge_mode_events.UpdateTimer(time);
        }
    }
}
