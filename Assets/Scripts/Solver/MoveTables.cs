using System;
using static Mapping;
public static class MoveTables
{
    public static int[,] co_move;      // [2187, 18]
    public static int[,] eo_move;      // [2048, 18]
    public static int[,] uds_move;     // [495, 18]
    public static int[,] cp_move;      // [40320, 18]
    public static int[,] ep_move;      // [40320, 18]
    public static int[,] uds2_move;    // [24, 18]

    static bool initialized = false;

    public static void Init()
    {
        if (initialized) return;
        initialized = true;

        co_move = new int[2187, 18];
        eo_move = new int[2048, 18];
        uds_move = new int[495, 18];
        cp_move = new int[40320, 18];
        ep_move = new int[40320, 18];
        uds2_move = new int[24, 18];

        BuildCOMove();
        BuildEOMove();
        BuildUDSliceMove();
        BuildCPMove();
        BuildEPMove();
        BuildUDSlice2Move();
    }

    // =========================================================

    static void BuildCOMove()
    {
        for (int i = 0; i < 2187; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeCO(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                co_move[i, m] = Coordinate.EncodeCO(copy);
            }
        }
    }

    static void BuildEOMove()
    {
        for (int i = 0; i < 2048; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeEO(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                eo_move[i, m] = Coordinate.EncodeEO(copy);
            }
        }
    }

    static void BuildUDSliceMove()
    {
        for (int i = 0; i < 495; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeUDSlice(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                uds_move[i, m] = Coordinate.EncodeUDSlice(copy);
            }
        }
    }

    static void BuildCPMove()
    {
        for (int i = 0; i < 40320; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeCornerPerm(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                cp_move[i, m] = Coordinate.EncodeCornerPerm(copy);
            }
        }
    }

    static void BuildEPMove()
    {
        for (int i = 0; i < 40320; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeEdgePerm(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                ep_move[i, m] = Coordinate.EncodeEdgePerm(copy);
            }
        }
    }

    static void BuildUDSlice2Move()
    {
        for (int i = 0; i < 24; i++)
        {
            CubieModel c = new CubieModel();
            Coordinate.DecodeUDSlice2(i, c);

            for (int m = 0; m < 18; m++)
            {
                CubieModel copy = c.Clone();
                copy.ApplyMove(IdxToMove(m));
                uds2_move[i, m] = Coordinate.EncodeUDSlice2(copy);
            }
        }
    }
}

//using System.Collections.Generic;
//using static Mapping;
//using static Coordinate;

//public static class MoveTables
//{
//    public static int[,] move_co;
//    public static int[,] move_eo;
//    public static int[,] move_mec;

//    public static int[,] move_cp;
//    public static int[,] move_ep;
//    static CubeState state;
//    static MoveTables()
//    {
//        state = new CubeState();
//        BuildPhase1Tables();
//        BuildPhase2Tables();
//    }

//    static void BuildPhase1Tables()
//    {
//        move_co = new int[2187, 18];
//        for (int co = 0; co < 2187; co++)
//        {
//            for (int m = 0; m < 18; m++)
//            {
//                ApplyCornerOriIdx(state, co);
//                state.ApplyMove(IdxToMove(m));
//                move_co[co, m] = GetCornerOriIdx(state);
//            }
//        }

//        move_eo = new int[2048, 18];
//        for (int eo = 0; eo < 2048; eo++)
//        {
//            for (int m = 0; m < 18; m++)
//            {
//                ApplyEdgeOriIdx(state, eo);
//                state.ApplyMove(IdxToMove(m));
//                move_eo[eo, m] = GetEdgeOriIdx(state);
//            }
//        }

//        move_mec = new int[495, 18];
//        for (int mec = 0; mec < 495; mec++)
//        {
//            for (int m = 0; m < 18; m++)
//            {
//                ApplyMiddleEdgeCombIdx(state, mec);
//                state.ApplyMove(IdxToMove(m));
//                move_mec[mec, m] = GetMiddleEdgeCombIdx(state);
//            }
//        }
//    }

//    static void BuildPhase2Tables()
//    {
//        move_cp = new int[40320, 10];

//        move_ep = new int[40320, 10];

//    }
//}
