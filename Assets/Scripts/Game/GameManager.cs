using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Mapping;
using UnityEngine.UI;
/// <summary>
/// Central controller of the game logic.
/// Responsibilities:
/// - Maintain the current CubeState (logical model).
/// - Coordinate animation and rendering.
/// - Apply moves safely (ensuring animations finish before next move).
/// - Trigger solver and scramble sequences.
/// </summary>

public class GameManager : MonoBehaviour
{
    public CubeRenderer render;
    public CubeAnimator animator;
    private KociembaSolver solver = new KociembaSolver();
    public CubeState current_state;
    private bool is_static = true;
    void Start()
    {
        current_state = new CubeState();
        // Automatically scramble if in Challenge Mode
        if (SceneManager.GetActiveScene().name == "ChallengeModeScene")
        {
            List<Move> scramble = Scrambler.GenerateScramble();
            for (int i = 0; i < scramble.Count; i++)
            {
                current_state.ApplyMove(scramble[i]);
            }
        }
        render.Initialize(current_state);
    }

    /// <summary>
    /// Debug helper to inspect the mathematical state of the cubies.
    /// </summary>
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
    public bool ApplyMove(Move move, bool animation=true)
    {
        if (is_static)
        {
            is_static = false;
            StartCoroutine(ApplyMoveRoutine(move, animation));
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
    private IEnumerator ApplyMoveRoutine(Move move, bool animation)
    {
        if (animation) yield return animator.AnimateMove(move);

        is_static = true;
        current_state.ApplyMove(move);
        render.RenderState(current_state);
    }

    /// <summary>
    /// Imports physical colors from the UI scene setup to the logical CubeState.
    /// Used for importing user-defined cube configurations.
    /// </summary>
    public void Import()
    {
        for (int cubie = 0; cubie < 27; ++cubie)
        {
            for (int face = 0; face < 6; ++face)
            {
                if (current_state[cubie][face] == -1) continue;
                current_state[cubie][face] = GetColorIdx(GameObject.Find("Face" + face.ToString() + "-" + cubie.ToString()).GetComponent<Image>().color);
            }
        }
        render.RenderState(current_state);
    }

    /// <summary>
    /// Validates if the current CubeState configuration is solvable by the Kociemba algorithm.
    /// </summary>
    public bool IsSolvable()
    {
        if (!current_state.IsValid()) return false;
        CubieModel model = new CubieModel(current_state);
        if (!model.IsSolvable()) return false;
        return true;
    }
    /// <summary>
    /// Starts a scramble animation sequence.
    /// </summary>
    public void Scramble(bool animation=true)
    {
        List<Move> scramble = Scrambler.GenerateScramble();
        StartCoroutine(ApplyMoveSeqRoutine(scramble, animation));
    }

    public List<Move> GetSolverMoves()
    {
        return solver.Solve(current_state);
    }
    public void Solve(bool animation=true)
    {
        List<Move> moves = solver.Solve(current_state);
        StartCoroutine(ApplyMoveSeqRoutine(moves, animation));
    }
    /// <summary>
    /// Coroutine that applies a series of scramble moves sequentially,
    /// waiting for each move to finish before starting the next.
    /// </summary>
    private IEnumerator ApplyMoveSeqRoutine(List<Move> moves, bool animation=true)
    {
        if (moves.Count != 0)
        {
            for (int i = 0; ;)
            {
                if (ApplyMove(moves[i], animation)) i++;
                if (i >= moves.Count) break;
                if (animation) yield return null;
            }
        }
    }

    public bool IsSolved()
    {
        return current_state.IsSolved();
    }
}