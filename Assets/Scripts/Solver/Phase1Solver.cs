using System;
using System.Collections.Generic;

public class Phase1Solver
{
    private List<int> solution = new List<int>();
    static int _co;
    static int _eo;
    static int _uds;

    public Phase1Solver()
    {
        CubieModel cm = new CubieModel();
        _co = Coordinate.EncodeCO(cm);
        _eo = Coordinate.EncodeEO(cm);
        _uds = Coordinate.EncodeUDSlice(cm);
    }
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

    int Heuristic(int co, int eo, int uds)
    {
        byte h1 = PruningTables.co_eo_prune[co, eo];
        byte h2 = PruningTables.eo_uds_prune[eo, uds];
        return Math.Max(h1, h2);
    }
}

//using System.Collections.Generic;

///// <summary>
///// IDA* solver for Phase 1 of Kociemba's Two-Phase algorithm.
///// 
///// Goal of Phase 1:
///// Bring the cube from any state into group G1 where:
///// - All corner orientations are correct (CO = 0)
///// - All edge orientations are correct (EO = 0)
///// - All UD-slice edges are in the middle layer (UDS = 0)
/////
///// This solver operates purely on coordinate representation:
///// (co, eo, uds) without using CubeState or Cubie.
///// </summary>
//public class Phase1Solver
//{
//    private List<int> solutionMoves = new List<int>();
//    private int maxDepth;

//    /// <summary>
//    /// Solve Phase 1 starting from given coordinates.
//    /// Returns a sequence of move indices (0..17).
//    /// </summary>
//    public List<int> Solve(int co, int eo, int uds)
//    {
//        solutionMoves.Clear();

//        maxDepth = PruningTables.GetPhase1Heuristic(co, eo, uds);

//        while (true)
//        {
//            if (Search(co, eo, uds, 0, -1))
//                return new List<int>(solutionMoves);

//            maxDepth++;
//        }
//    }

//    /// <summary>
//    /// Recursive IDA* search.
//    /// </summary>
//    /// <param name="co">corner orientation coordinate</param>
//    /// <param name="eo">edge orientation coordinate</param>
//    /// <param name="uds">UD-slice coordinate</param>
//    /// <param name="depth">current search depth</param>
//    /// <param name="lastMove">previous move index (for pruning)</param>
//    /// <returns>true if goal found within bound</returns>
//    private bool Search(int co, int eo, int uds, int depth, int lastMove)
//    {
//        int h = PruningTables.GetPhase1Heuristic(co, eo, uds);

//        if (depth + h > maxDepth)
//            return false;

//        // Goal: reached G1
//        if (h == 0)
//            return true;

//        for (int move = 0; move < 18; move++)
//        {
//            // --- Move pruning rules ---

//            // Do not apply same face twice in a row
//            if (lastMove != -1 && move / 3 == lastMove / 3)
//                continue;

//            // Optional stronger pruning:
//            // avoid sequences like R L R (opposite faces)
//            if (lastMove != -1 && IsOppositeFace(move, lastMove))
//                continue;

//            int nextCO = MoveTables.moveCO[co, move];
//            int nextEO = MoveTables.moveEO[eo, move];
//            int nextUDS = MoveTables.moveUDS[uds, move];

//            solutionMoves.Add(move);

//            if (Search(nextCO, nextEO, nextUDS, depth + 1, move))
//                return true;

//            solutionMoves.RemoveAt(solutionMoves.Count - 1);
//        }

//        return false;
//    }

//    /// <summary>
//    /// Checks if two moves are on opposite faces (e.g. F vs B, U vs D, R vs L).
//    /// Used for additional pruning to reduce symmetric branches.
//    /// </summary>
//    private bool IsOppositeFace(int m1, int m2)
//    {
//        int f1 = m1 / 3;
//        int f2 = m2 / 3;

//        // Faces are indexed 0..5 in Mapping:
//        // FRONT(0) opposite BACK(3)
//        // UP(1) opposite DOWN(4)
//        // RIGHT(2) opposite LEFT(5)
//        return (f1 == 0 && f2 == 3) ||
//               (f1 == 3 && f2 == 0) ||
//               (f1 == 1 && f2 == 4) ||
//               (f1 == 4 && f2 == 1) ||
//               (f1 == 2 && f2 == 5) ||
//               (f1 == 5 && f2 == 2);
//    }
//}