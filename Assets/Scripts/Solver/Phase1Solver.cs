using System;
using System.Collections.Generic;

/// <summary>
/// Implements Phase 1 of the Kociemba algorithm.
/// Goal: Orient all corners and edges, and place the 4 UD-slice edges into the middle slice.
/// Uses IDA* search with heuristics from pruning tables.
/// </summary>
public class Phase1Solver
{
    /// <summary> Phase 1's Result </summary>
    private List<int> solution = new List<int>();

    /// <summary> Target Corner Orientation index </summary>
    static int _co;
    /// <summary> Target Edge Orientation index </summary>
    static int _eo;
    /// <summary> Target UD-Slice position index </summary>
    static int _uds;

    /// <summary>
    /// Initializes the Phase 1 solver and sets the target solved indices.
    /// </summary>
    public Phase1Solver()
    {
        CubieModel cm = new CubieModel();
        _co = Coordinate.EncodeCO(cm);
        _eo = Coordinate.EncodeEO(cm);
        _uds = Coordinate.EncodeUDSlice(cm);
    }

    /// <summary>
    /// Solves Phase 1 using Iterative Deepening A* (IDA*).
    /// </summary>
    /// <param name="co">Current Corner Orientation index.</param>
    /// <param name="eo">Current Edge Orientation index.</param>
    /// <param name="uds">Current UD-Slice position index.</param>
    /// <returns>A list of move indices representing the Phase 1 solution.</returns>
    public List<int> Solve(int co, int eo, int uds)
    {
        PruningTables.Init();

        solution.Clear();

        int bound = Heuristic(co, eo, uds);

        while (true)
        {
            int t = Search(co, eo, uds, 0, bound, -1, -1);
            if (t == -1) return solution;
            bound = t;
        }
    }

    /// <summary>
    /// Recursive depth-first search for IDA*.
    /// </summary>
    /// <param name="depth">Current search depth.</param>
    /// <param name="bound">Maximum allowed depth (f-score limit).</param>
    /// <param name="prev_face">The face of the last move to avoid redundant turns (e.g., R followed by R).</param>
    /// <param name="prev_prev_face">The face of the move before last (to handle cases like R L R).</param>
    /// <returns>-1 if solved, otherwise returns the minimum f-score encountered that exceeded bound.</returns>
    int Search(int co, int eo, int uds, int depth, int bound, int prev_face, int prev_prev_face)
    {
        int h = Heuristic(co, eo, uds);
        int f = depth + h;

        if (f > bound) return f;

        if (co == _co && eo == _eo && uds == _uds)
            return -1;

        int min = int.MaxValue;

        for (int move = 0; move < 18; move++)
        {
            int face = move / 3;

            if (prev_face != -1)
            {
                if (face == prev_face) continue;
                //if (face % 3 == prev_face % 3) continue;
            }
            if (prev_prev_face != -1)
            {
                if (face == prev_prev_face && face % 3 == prev_face % 3) continue; 
            }
            int nco = MoveTables.co_move[co, move];
            int neo = MoveTables.eo_move[eo, move];
            int nuds = MoveTables.uds_move[uds, move];

            solution.Add(move);

            int t = Search(nco, neo, nuds, depth + 1, bound, face, prev_face);
            if (t == -1) return -1;

            if (t < min) min = t;

            solution.RemoveAt(solution.Count - 1);
        }

        return min;
    }

    /// <summary>
    /// Calculates the lower bound of moves remaining using pruning tables.
    /// Uses the maximum of available heuristics to remain admissible.
    /// </summary>
    int Heuristic(int co, int eo, int uds)
    {
        byte h1 = PruningTables.co_eo_prune[co, eo];
        byte h2 = PruningTables.eo_uds_prune[eo, uds];
        return Math.Max(h1, h2);
    }
}