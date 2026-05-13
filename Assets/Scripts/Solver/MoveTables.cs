using System;
using static Mapping;

/// <summary>
/// Precomputes and stores transition tables for all cube coordinates.
/// These tables allow the solver to find the next coordinate index given a move
/// without performing expensive CubieModel simulations.
/// </summary>
public static class MoveTables
{
    /// <summary> Transition table for Corner Orientation. </summary>
    public static int[,] co_move;      // [2187, 18]
    /// <summary> Transition table for Edge Orientation. </summary>
    public static int[,] eo_move;      // [2048, 18]
    /// <summary> Transition table for UD-Slice position (Phase 1). </summary>
    public static int[,] uds_move;     // [495, 18]
    /// <summary> Transition table for Corner Permutation (Phase 2). </summary>
    public static int[,] cp_move;      // [40320, 18]
    /// <summary> Transition table for Edge Permutation (Phase 2). </summary>
    public static int[,] ep_move;      // [40320, 18]
    /// <summary> Transition table for UD-Slice permutation (Phase 2). </summary>
    public static int[,] uds2_move;    // [24, 18]

    /// <summary> Checks if it has been initialized </summary>
    static bool initialized = false;

    /// <summary>
    /// Initializes all transition tables if they haven't been built yet.
    /// </summary>
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

    /// <summary> Precomputes co_move </summary>
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

    /// <summary> Precomputes eo_move </summary>
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

    /// <summary> Precomputes uds_move </summary>
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

    /// <summary> Precomputes cp_move </summary>
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

    /// <summary> Precomputes ep_move </summary>
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

    /// <summary> Precomputes uds2_move </summary>
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
