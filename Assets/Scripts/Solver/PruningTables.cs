using System;
using System.Collections.Generic;
using static Coordinate;
public static class PruningTables
{
    public static byte[,] co_eo_prune;   // [2187,2048]
    public static byte[,] eo_uds_prune;  // [2048,495]
    public static byte[,] cp_uds2_prune;  // [40320,24]
    public static byte[,] ep_uds2_prune;  // [40320,24]
    static int _co;
    static int _eo;
    static int _uds;
    static int _cp;
    static int _ep;
    static int _uds2;
    static bool initialized = false;

    public static void Init()
    {
        if (initialized) return;
        initialized = true;

        MoveTables.Init();

        CubieModel cm = new CubieModel();
        _co = Coordinate.EncodeCO(cm);
        _eo = Coordinate.EncodeEO(cm);
        _uds = Coordinate.EncodeUDSlice(cm);
        _cp = Coordinate.EncodeCornerPerm(cm);
        _ep = Coordinate.EncodeEdgePerm(cm);
        _uds2 = Coordinate.EncodeUDSlice2(cm);

        co_eo_prune = new byte[2187, 2048];
        eo_uds_prune = new byte[2048, 495];
        cp_uds2_prune = new byte[40320, 24];
        ep_uds2_prune = new byte[40320, 24];

        Fill(co_eo_prune, 255);
        Fill(eo_uds_prune, 255);
        Fill(cp_uds2_prune, 255);
        Fill(ep_uds2_prune, 255);

        Build_CO_EO();
        Build_EO_UDS();
        Build_CP_UDS2();
        Build_EP_UDS2();
    }

    // =====================================================

    static void Fill(byte[,] table, byte val)
    {
        for (int i = 0; i < table.GetLength(0); i++)
            for (int j = 0; j < table.GetLength(1); j++)
                table[i, j] = val;
    }

    // =====================================================
    // PHASE 1
    // =====================================================

    static void Build_CO_EO()
    {
        Queue<(int co, int eo)> q = new Queue<(int, int)>();
        co_eo_prune[_co, _eo] = 0;
        q.Enqueue((_co, _eo));

        while (q.Count > 0)
        {
            var (co, eo) = q.Dequeue();
            byte depth = co_eo_prune[co, eo];

            for (int m = 0; m < 18; m++)
            {
                int nco = MoveTables.co_move[co, m];
                int neo = MoveTables.eo_move[eo, m];

                if (co_eo_prune[nco, neo] == 255)
                {
                    co_eo_prune[nco, neo] = (byte)(depth + 1);
                    q.Enqueue((nco, neo));
                }
            }
        }
    }

    static void Build_EO_UDS()
    {
        Queue<(int eo, int uds)> q = new Queue<(int, int)>();
        eo_uds_prune[_eo, _uds] = 0;
        q.Enqueue((_eo, _uds));

        while (q.Count > 0)
        {
            var (eo, uds) = q.Dequeue();
            byte depth = eo_uds_prune[eo, uds];

            for (int m = 0; m < 18; m++)
            {
                int neo = MoveTables.eo_move[eo, m];
                int nuds = MoveTables.uds_move[uds, m];

                if (eo_uds_prune[neo, nuds] == 255)
                {
                    eo_uds_prune[neo, nuds] = (byte)(depth + 1);
                    q.Enqueue((neo, nuds));
                }
            }
        }
    }

    // =====================================================
    // PHASE 2
    // =====================================================

    static void Build_CP_UDS2()
    {
        Queue<(int cp, int uds2)> q = new Queue<(int, int)>();
        cp_uds2_prune[_cp, _uds2] = 0;
        q.Enqueue((_cp, _uds2));

        while (q.Count > 0)
        {
            var (cp, uds2) = q.Dequeue();
            byte depth = cp_uds2_prune[cp, uds2];

            for (int m = 0; m < 18; m++)
            {
                int ncp = MoveTables.cp_move[cp, m];
                int nuds2 = MoveTables.uds2_move[uds2, m];

                if (cp_uds2_prune[ncp, nuds2] == 255)
                {
                    cp_uds2_prune[ncp, nuds2] = (byte)(depth + 1);
                    q.Enqueue((ncp, nuds2));
                }
            }
        }
    }

    static void Build_EP_UDS2()
    {
        Queue<(int ep, int uds2)> q = new Queue<(int, int)>();
        ep_uds2_prune[_ep, _uds2] = 0;
        q.Enqueue((_ep, _uds2));

        while (q.Count > 0)
        {
            var (ep, uds2) = q.Dequeue();
            byte depth = ep_uds2_prune[ep, uds2];

            for (int m = 0; m < 18; m++)
            {
                int nep = MoveTables.ep_move[ep, m];
                int nuds2 = MoveTables.uds2_move[uds2, m];

                if (ep_uds2_prune[nep, nuds2] == 255)
                {
                    ep_uds2_prune[nep, nuds2] = (byte)(depth + 1);
                    q.Enqueue((nep, nuds2));
                }
            }
        }
    }
}

//public class PruningTables
//{
//    public static byte[] prune_phase_1;
//    public static byte[] prune_phase_2;

//    static PruningTables()
//    {

//    }

//    static void BuildPhase1Prune()
//    {

//    }

//    static void BuildPhase2Prune()
//    {

//    }

//    public static int GetPhase1Heuristic(int co, int eo, int mec)
//    {
//        return 0;
//    }

//    public static int GetPhase2Heuristic(int cp, int ep)
//    {
//        return 0;
//    }
//}
