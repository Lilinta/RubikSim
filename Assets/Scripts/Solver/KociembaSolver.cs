using System.Collections.Generic;
using UnityEngine;
using static Mapping;

/// <summary>
/// The main solver class coordinating the Two-Phase Kociemba algorithm.
/// </summary>
public class KociembaSolver
{
    private Phase1Solver phase1 = new Phase1Solver();
    private Phase2Solver phase2 = new Phase2Solver();

    /// <summary>
    /// Initializes solver.
    /// </summary>
    public KociembaSolver()
    {
        // Ensure pruning tables are loaded into memory before solving
        PruningTables.Init();
    }

    /// <summary>
    /// Calculates the optimal (or near-optimal) solution for a given cube state.
    /// </summary>
    /// <param name="state">The current physical state of the cube.</param>
    /// <returns>A list of Move objects to solve the cube.</returns>
    public List<Move> Solve(CubeState state)
    {
        // 1. Convert visual state to mathematical model
        CubieModel start = new CubieModel(state);

        // 2. Solve Phase 1 (Orientation & Slice)
        int co = Coordinate.EncodeCO(start);
        int eo = Coordinate.EncodeEO(start);
        int uds = Coordinate.EncodeUDSlice(start);
        List<int> sol1 = phase1.Solve(co, eo, uds);
        Debug.Log("Phase 1 Solution Length: " + sol1.Count);

        // 3. Apply Phase 1 moves to the model to find the intermediate state (G1)
        CubieModel g1 = start.Clone();
        foreach (int m in sol1)
            g1.ApplyMove(IdxToMove(m));

        // 4. Solve Phase 2 (Permutation) starting from G1 state
        int cp = Coordinate.EncodeCornerPerm(g1);
        int ep = Coordinate.EncodeEdgePerm(g1);
        int uds2 = Coordinate.EncodeUDSlice2(g1); // vẫn cần
        List<int> sol2 = phase2.Solve(cp, ep, uds2);
        Debug.Log("Phase 2 Solution Length: " + sol2.Count);

        // 5. Combine solutions and convert back to Move objects
        List<Move> res = new List<Move>();
        foreach (int m in sol1)
        {
            res.Add(IdxToMove(m));
        }
        foreach (int m in sol2)
        {
            res.Add(IdxToMove(m));
        }
        Debug.Log("Total Move Count: " + res.Count);
        return res;
    }
}
