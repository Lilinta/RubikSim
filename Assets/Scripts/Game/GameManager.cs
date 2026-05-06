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
    private KociembaSolver solver = new KociembaSolver();
    private CubeState current_state;
    private bool is_static = true;
    void Start()
    {
        current_state = new CubeState();
        render.Initialize(current_state);
    }

    public void DebugOutput()
    {
        CubieModel start = new CubieModel(current_state);
        // Đảm bảo tables đã build
        Debug.Log("_________________________");
        Debug.Log("CubieModel");
        Debug.Log("CO: ");
        DebugArray(start.corner_ori);
        Debug.Log("EO: ");
        DebugArray(start.edge_ori);
        Debug.Log("CP: ");
        DebugArray(start.corner_perm);
        Debug.Log("EP: ");
        DebugArray(start.edge_perm);
        Debug.Log("-------------------------");
    }
    void DebugArray(int[] arr)
    {
        string txt = string.Empty;
        foreach (int i in arr)
        {
            txt += i.ToString() + " ";
        }
        Debug.Log(txt);
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
        current_state.ApplyMove(move);
        render.RenderState(current_state);
    }

    /// <summary>
    /// Starts a scramble animation sequence.
    /// </summary>

    public void Scramble()
    {
        List<Move> scramble = Scrambler.GenerateScramble(30);
        StartCoroutine(ApplyMoveSeqRoutine(scramble));
    }

    public void Solve()
    {
        List<Move> moves = solver.Solve(current_state);
        StartCoroutine(ApplyMoveSeqRoutine(moves));
    }
    /// <summary>
    /// Coroutine that applies a series of scramble moves sequentially,
    /// waiting for each move to finish before starting the next.
    /// </summary>
    private IEnumerator ApplyMoveSeqRoutine(List<Move> moves)
    {
        if (moves.Count != 0)
        {
            for (int i = 0; ;)
            {
                if (ApplyMove(moves[i])) i++;
                if (i >= moves.Count) break;
                yield return null;
            }
        }
    }
}