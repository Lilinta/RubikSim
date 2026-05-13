using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the step-by-step playback of the solver's solution.
/// Allows the user to move forward and backward through the calculated moves.
/// </summary>
public class SolverManager: MonoBehaviour
{
    public GameManager game_manager;
    List<Move> current_moves;
    int idx;

    /// <summary>
    /// Fetches the solution from the solver and resets the playback pointer.
    /// </summary>
    public void Init()
    {
        current_moves = game_manager.GetSolverMoves();
        idx = -1;
    }

    /// <summary>
    /// Reverts the previous move in the sequence.
    /// </summary>
    public void Backward()
    {
        if (idx == -1 ) return;
        if (game_manager.ApplyMove(current_moves[idx].GetInvertedMove()))
        {
            idx--;
        }
    }

    /// <summary>
    /// Applies the next move in the sequence.
    /// </summary>
    public void Forward()
    {
        if (idx == current_moves.Count - 1) return;
        if (game_manager.ApplyMove(current_moves[idx + 1]))
        {
            idx++;
        }
    }
}