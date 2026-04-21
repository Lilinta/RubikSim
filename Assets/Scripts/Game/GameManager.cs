using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central controller of the game logic.
/// 
/// Responsibilities:
/// - Maintain the current CubeState (logical model)
/// - Coordinate animation and rendering
/// - Apply moves safely (prevent overlapping animations)
/// - Trigger scramble sequences
/// 
/// This class acts as the bridge between Core logic and Rendering.
/// </summary>

public class GameManager : MonoBehaviour
{
    public CubeRenderer render;
    public CubeAnimator animator;

    private CubeState currentState;
    private bool is_static = true;
    void Start()
    {
        currentState = new CubeState();
        render.Initialize(currentState);
    }

    /// <summary>
    /// Attempts to apply a move to the cube.
    /// 
    /// A move can only be applied if no animation is currently running.
    /// </summary>
    /// <param name="move">Move to apply</param>
    /// <returns>True if the move was accepted, otherwise false</returns>
    public bool ApplyMove(Move move)
    {
        if (is_static)
        {
            is_static = false;
            StartCoroutine(ApplyMoveRoutine(move));
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Coroutine that performs:
    /// 1. Animation of the move
    /// 2. Logical update of CubeState
    /// 3. Re-rendering the cube
    /// </summary>
    private IEnumerator ApplyMoveRoutine(Move move)
    {
        yield return animator.AnimateMove(move);

        is_static = true;
        currentState.ApplyMove(move);
        render.RenderState(currentState);
    }

    /// <summary>
    /// Starts a scramble animation sequence.
    /// </summary>

    public void Scramble()
    {
        StartCoroutine(ScrambleRoutine());
    }

    /// <summary>
    /// Coroutine that applies a series of scramble moves sequentially,
    /// waiting for each move to finish before starting the next.
    /// </summary>
    private IEnumerator ScrambleRoutine()
    {
        List<Move> scramble = Scrambler.GenerateScramble(30);
        for (int i = 0;;)
        {
            if (ApplyMove(scramble[i])) i++;
            if (i >= scramble.Count) break;
            yield return null;
        }
    }
}