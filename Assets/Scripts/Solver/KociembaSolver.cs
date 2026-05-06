using System;
using System.Collections.Generic;
using UnityEngine;
using static Mapping;
public class KociembaSolver
{
    private Phase1Solver phase1 = new Phase1Solver();
    private Phase2Solver phase2 = new Phase2Solver();

    public KociembaSolver()
    {
        PruningTables.Init();
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
    public List<Move> Solve(CubeState state)
    {
        CubieModel start = new CubieModel(state);
        // Đảm bảo tables đã build
        Debug.Log("CubieModel");
        Debug.Log("CO: ");
        DebugArray(start.corner_ori);
        Debug.Log("EO: ");
        DebugArray(start.edge_ori);
        Debug.Log("CP: ");
        DebugArray(start.corner_perm);
        Debug.Log("EP: ");
        DebugArray(start.edge_perm);
        PruningTables.Init();
        Debug.Log("PruningTables");
        // =========================
        // Encode Phase 1 coords
        // =========================
        int co = Coordinate.EncodeCO(start);
        int eo = Coordinate.EncodeEO(start);
        int uds = Coordinate.EncodeUDSlice(start);

        // =========================
        // Phase 1
        // =========================
        List<int> sol1 = phase1.Solve(co, eo, uds);
        Debug.Log("sol1: " + sol1.Count.ToString());
        // Apply phase 1 moves lên bản copy
        CubieModel g1 = start.Clone();
        foreach (int m in sol1)
            g1.ApplyMove(IdxToMove(m));
        Debug.Log("CubieModel");
        Debug.Log("CO: ");
        DebugArray(g1.corner_ori);
        Debug.Log("EO: ");
        DebugArray(g1.edge_ori);
        Debug.Log("CP: ");
        DebugArray(g1.corner_perm);
        Debug.Log("EP: ");
        DebugArray(g1.edge_perm);
        // =========================
        // Encode Phase 2 coords
        // =========================
        int cp = Coordinate.EncodeCornerPerm(g1);
        int ep = Coordinate.EncodeEdgePerm(g1);
        int uds2 = Coordinate.EncodeUDSlice2(g1); // vẫn cần

        // =========================
        // Phase 2
        // =========================
        List<int> sol2 = phase2.Solve(cp, ep, uds2);
        Debug.Log("sol2: " + sol2.Count.ToString());
        // =========================
        // Ghép lời giải
        // =========================
        List<Move> res = new List<Move>();
        Debug.Log("----");
        foreach (int m in sol1)
        {
            res.Add(IdxToMove(m));
        }
        foreach (int m in sol2)
        {
            res.Add(IdxToMove(m));
        }
        Debug.Log("res: " +  res.Count.ToString());
        return res;
    }
}

//using System.Collections.Generic;
//using static Mapping;

//public class KociembaSolver
//{
//    public static List<Move> Solve(CubeState state)
//    {
//        CubieModel cm = new CubieModel(state);

//        int co = Coordinate.EncodeCO(cm);
//        int eo = Coordinate.EncodeEO(cm);
//        int uds = Coordinate.EncodeUDSlice(cm);

//        Phase1Solver p1 = new Phase1Solver();
//        List<int> phase1Moves = p1.Solve(co, eo, uds);

//        foreach (int m in phase1Moves)
//            cm.ApplyMove(MoveFromIndex(m));

//        int cp = Coordinate.EncodeCornerPerm(cm);
//        int ep = Coordinate.EncodeEdgePerm(cm);

//        Phase2Solver p2 = new Phase2Solver();
//        List<int> phase2Moves = p2.Solve(cp, ep);

//        List<Move> res = new List<Move>();
//        for (int i = 0; i < phase1Moves.Count; i++)
//        {
//            res.Add(IdxToMove(phase1Moves[i]));
//        }
//        for (int i = 0; i < phase2Moves.Count; i++)
//        {
//            res.Add(IdxToMove(phase2Moves[i]));
//        }
//        return res;
//    }
//}
