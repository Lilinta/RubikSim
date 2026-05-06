using System;
using System.Collections.Generic;

public class Phase2Solver
{
    private List<int> solution = new List<int>();

    // 10 moves hợp lệ trong phase 2 (map vào 0..17)
    private static readonly int[] phase2_moves =
    {
        3,4,5,        // U
        12,13,14,     // D
        1,            // F2
        10,           // B2
        7,            // R2
        16            // L2
    };

    static int _cp;
    static int _ep;
    static int _uds2;

    public Phase2Solver()
    {
        CubieModel cm = new CubieModel();
        _cp = Coordinate.EncodeCornerPerm(cm);
        _ep = Coordinate.EncodeEdgePerm(cm);
        _uds2 = Coordinate.EncodeUDSlice2(cm);
    }
    public List<int> Solve(int cp, int ep, int uds2)
    {
        solution.Clear();

        int bound = Heuristic(cp, ep, uds2);

        while (true)
        {
            int t = Search(cp, ep, uds2, 0, bound, -1, -1);
            if (t == -1) return solution;
            bound = t;
        }
    }

    int Search(int cp, int ep, int uds2, int depth, int bound, int prev_face, int prev_prev_face)
    {
        int h = Heuristic(cp, ep, uds2);
        int f = depth + h;

        if (f > bound) return f;

        if (cp == _cp && ep == _ep && uds2 == _uds2)
            return -1;

        int min = int.MaxValue;

        foreach (int move in phase2_moves)
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

            int ncp = MoveTables.cp_move[cp, move];
            int nep = MoveTables.ep_move[ep, move];
            int nuds2 = MoveTables.uds2_move[uds2, move];

            solution.Add(move);

            int t = Search(ncp, nep, nuds2, depth + 1, bound, face, prev_face);
            if (t == -1) return -1;

            if (t < min) min = t;

            solution.RemoveAt(solution.Count - 1);
        }

        return min;
    }

    int Heuristic(int cp, int ep, int uds2)
    {
        byte h1 = PruningTables.cp_uds2_prune[cp, uds2];
        byte h2 = PruningTables.ep_uds2_prune[ep, uds2];
        return Math.Max(h1, h2);
    }
}