using System;
using UnityEngine;
using static Mapping;

public class CubieModel
{
    public int[] corner_perm = new int[8];
    public int[] corner_ori = new int[8];

    public int[] edge_perm = new int[12];
    public int[] edge_ori = new int[12];

    static int[][] FACE_CORNERS =
    {
        new int[] { 0, 4, 5, 1 },
        new int[] { 0, 1, 2, 3 },
        new int[] { 0, 3, 7, 4 },
        new int[] { 3, 2, 6, 7 },
        new int[] { 5, 4, 7, 6 },
        new int[] { 2, 1, 5, 6 },
    };
    static int[][] FACE_EDGES =
    {
        new int[] { 1, 8, 5, 9 },
        new int[] { 0, 1, 2, 3 },
        new int[] { 0, 11, 4, 8 },
        new int[] { 11, 3, 10, 7 },
        new int[] { 5, 4, 7, 6 },
        new int[] { 2, 9, 6, 10 }
    };
    static int[][] CORNER_ORI_DELTA =
    {
        new int[] { 2, 1, 0, 0, 1, 2, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 1, 0, 0, 2, 2, 0, 0, 1 },
        new int[] { 0, 0, 2, 1, 0, 0, 1, 2 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 2, 1, 0, 0, 1, 2, 0 },
    };
    static int[][] EDGE_ORI_DELTA =
    {
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0 },
    };
    public CubieModel()
    {
        for (int i = 0; i < 8; i++)
        {
            corner_perm[i] = i;
            corner_ori[i] = 0;
        }
        for (int i = 0; i < 12; i++)
        {
            edge_perm[i] = i;
            edge_ori[i] = 0;
        }
    }

    public CubieModel(CubeState state)
    {
        foreach (int pos_idx in CORNER_LIST)
        {
            Vector3Int pos = IdxToPos(pos_idx);
            int x_color = state[pos_idx][pos.x > 0 ? 0 : 3];
            int y_color = state[pos_idx][pos.y > 0 ? 1 : 4];
            int z_color = state[pos_idx][pos.z > 0 ? 2 : 5];
            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
            int original_pos_idx = PosToIdx(new Vector3Int(back ? -1 : 1, down ? -1 : 1, left ? -1 : 1));
            corner_perm[TO_KOCIEMBA_IDX[pos_idx]] = TO_KOCIEMBA_IDX[original_pos_idx];

            int ori = 0;
            if (y_color % 3 == 1) ori = 0;
            else
            {
                int rev = (pos.x < 0 ? 1 : 0) ^ (pos.y < 0 ? 1 : 0) ^ (pos.z < 0 ? 1 : 0);

                if (rev == 0)
                {
                    if (x_color % 3 == 1) ori = 1;
                    else ori = 2;
                }
                else
                {
                    if (x_color % 3 == 1) ori = 2;
                    else ori = 1;
                }
            }
            corner_ori[TO_KOCIEMBA_IDX[pos_idx]] = ori;
        }

        foreach (int pos_idx in EDGE_LIST)
        {
            Vector3Int pos = IdxToPos(pos_idx);
            int x_color = -1;
            int y_color = -1;
            int z_color = -1;
            if (pos.x != 0) x_color = state[pos_idx][pos.x > 0 ? 0 : 3];
            if (pos.y != 0) y_color = state[pos_idx][pos.y > 0 ? 1 : 4];
            if (pos.z != 0) z_color = state[pos_idx][pos.z > 0 ? 2 : 5];
            bool front = (x_color == 0) || (y_color == 0) || (z_color == 0);
            bool up = (x_color == 1) || (y_color == 1) || (z_color == 1);
            bool right = (x_color == 2) || (y_color == 2) || (z_color == 2);
            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
            int original_x = 0;
            int original_y = 0;
            int original_z = 0;
            if (front) original_x = 1;
            if (back) original_x = -1;
            if (up) original_y = 1;
            if (down) original_y = -1;
            if (right) original_z = 1;
            if (left) original_z = -1;
            int original_pos_idx = PosToIdx(new Vector3Int(original_x, original_y, original_z));
            edge_perm[TO_KOCIEMBA_IDX[pos_idx]] = TO_KOCIEMBA_IDX[original_pos_idx];

            int ori = 0;
            if (pos.y != 0)
            {
                if (up || down)
                {
                    if (y_color % 3 == 1) ori = 0;
                    else ori = 1;
                } else
                {
                    if (y_color % 3 == 2) ori = 0;
                    else ori = 1;
                }
            } else
            {
                if (up || down)
                {
                    if (z_color % 3 == 1) ori = 0;
                    else ori = 1;
                } else
                {
                    if (z_color % 3 == 2) ori = 0;
                    else ori = 1;
                }
            }
            edge_ori[TO_KOCIEMBA_IDX[pos_idx]] = ori;
        }
    }
    
    public CubieModel Clone()
    {
        CubieModel c = new CubieModel();

        Array.Copy(corner_perm, c.corner_perm, 8);
        Array.Copy(corner_ori, c.corner_ori, 8);
        Array.Copy(edge_perm, c.edge_perm, 12);
        Array.Copy(edge_ori, c.edge_ori, 12);

        return c;
    }
    public void ApplyMove(Move move)
    {
        int face = move.face;
        bool rev = move.rev;
        bool is180 = move.is180;
        int times = 1;
        if (rev) times = 3;
        if (is180) times = 2;
        for (int t = 0; t < times; t++)
        {
            ApplyQuarterClockwiseMove(face);
        }
    }

    private void ApplyQuarterClockwiseMove(int face)
    {
        Cycle(corner_perm, FACE_CORNERS[face]);
        Cycle(corner_ori, FACE_CORNERS[face]);
        for (int i = 0; i < corner_ori.Length; i++)
        {
            corner_ori[i] = (corner_ori[i] + CORNER_ORI_DELTA[face][i]) % 3;
        }

        Cycle(edge_perm, FACE_EDGES[face]);
        Cycle(edge_ori, FACE_EDGES[face]);
        for (int i = 0; i < edge_ori.Length; i++)
        {
            edge_ori[i] = edge_ori[i] ^ EDGE_ORI_DELTA[face][i];
        }
    }

    private void Cycle(int[] arr, int[] p)
    {
        int k = p.Length;
        int tmp = arr[p[k - 1]];
        for (int i = k-1; i > 0; i--)
        {
            arr[p[i]] = arr[p[i - 1]];
        }
        arr[p[0]] = tmp;
    }
}

//using System;
//using UnityEngine;
//using static Mapping;
//public class CubieModel
//{
//    // =============================
//    // Kociemba indexing
//    // =============================

//    public int[] corner_perm = new int[8];
//    public int[] corner_ori = new int[8];

//    public int[] edge_perm = new int[12];
//    public int[] edge_ori = new int[12];

//    // =============================
//    // Constructor
//    // =============================

//    public CubieModel()
//    {
//        for (int i = 0; i < 8; i++)
//        {
//            corner_perm[i] = i;
//            corner_ori[i] = 0;
//        }

//        for (int i = 0; i < 12; i++)
//        {
//            edge_perm[i] = i;
//            edge_ori[i] = 0;
//        }
//    }

//    public CubieModel(CubeState state) : this()
//    {
//        BuildFromState(state);
//    }

//    public CubieModel Clone()
//    {
//        CubieModel c = new CubieModel();

//        Array.Copy(corner_perm, c.corner_perm, 8);
//        Array.Copy(corner_ori, c.corner_ori, 8);
//        Array.Copy(edge_perm, c.edge_perm, 12);
//        Array.Copy(edge_ori, c.edge_ori, 12);

//        return c;
//    }

//    // ============================================================
//    // Move application (18 moves: face*3 + type)
//    // type: 0=90, 1=180, 2=rev
//    // ============================================================

//    public void ApplyMove(Move _move)
//    {
//        int move = MoveToIdx(_move);
//        int face = move / 3;
//        int type = move % 3;

//        int turns = 1;
//        if (type == 1) turns = 2;
//        if (type == 2) turns = 3;

//        for (int i = 0; i < turns; i++)
//            ApplyFace(face);
//    }

//    // ============================================================
//    // Core face rotation
//    // ============================================================

//    void ApplyFace(int face)
//    {
//        switch (face)
//        {
//            case 0: RotateF(); break;
//            case 1: RotateU(); break;
//            case 2: RotateR(); break;
//            case 3: RotateB(); break;
//            case 4: RotateD(); break;
//            case 5: RotateL(); break;
//        }
//    }

//    // ============================================================
//    // FACE ROTATIONS (Kociemba-consistent)
//    // ============================================================

//    void Cycle4(int[] arr, int a, int b, int c, int d)
//    {
//        int temp = arr[a];
//        arr[a] = arr[d];
//        arr[d] = arr[c];
//        arr[c] = arr[b];
//        arr[b] = temp;
//    }

//    void RotateU()
//    {
//        Cycle4(corner_perm, 0, 1, 2, 3);
//        Cycle4(edge_perm, 0, 1, 2, 3);
//    }

//    void RotateD()
//    {
//        Cycle4(corner_perm, 4, 7, 6, 5);
//        Cycle4(edge_perm, 4, 7, 6, 5);
//    }

//    void RotateF()
//    {
//        Cycle4(corner_perm, 0, 4, 5, 1);
//        Cycle4(edge_perm, 1, 8, 5, 9);

//        TwistCorner(0, 1);
//        TwistCorner(4, 2);
//        TwistCorner(5, 1);
//        TwistCorner(1, 2);

//        FlipEdge(1);
//        FlipEdge(8);
//        FlipEdge(5);
//        FlipEdge(9);
//    }

//    void RotateB()
//    {
//        Cycle4(corner_perm, 2, 3, 7, 6);
//        Cycle4(edge_perm, 3, 11, 7, 10);

//        TwistCorner(2, 1);
//        TwistCorner(3, 2);
//        TwistCorner(7, 1);
//        TwistCorner(6, 2);

//        FlipEdge(3);
//        FlipEdge(11);
//        FlipEdge(7);
//        FlipEdge(10);
//    }

//    void RotateR()
//    {
//        Cycle4(corner_perm, 0, 3, 7, 4);
//        Cycle4(edge_perm, 0, 11, 4, 8);

//        TwistCorner(0, 2);
//        TwistCorner(3, 1);
//        TwistCorner(7, 2);
//        TwistCorner(4, 1);
//    }

//    void RotateL()
//    {
//        Cycle4(corner_perm, 1, 5, 6, 2);
//        Cycle4(edge_perm, 2, 9, 6, 10);

//        TwistCorner(1, 2);
//        TwistCorner(5, 1);
//        TwistCorner(6, 2);
//        TwistCorner(2, 1);
//    }

//    // ============================================================
//    // Orientation helpers
//    // ============================================================

//    void TwistCorner(int idx, int amount)
//    {
//        corner_ori[idx] = (corner_ori[idx] + amount) % 3;
//    }

//    void FlipEdge(int idx)
//    {
//        edge_ori[idx] ^= 1;
//    }

//    // ============================
//    // Mapping chuẩn Kociemba
//    // ============================

//    static readonly int[][] corner_colors =
//    {
//    new[]{ Mapping.UP, Mapping.RIGHT, Mapping.FRONT }, // URF
//    new[]{ Mapping.UP, Mapping.FRONT, Mapping.LEFT  }, // UFL
//    new[]{ Mapping.UP, Mapping.LEFT,  Mapping.BACK  }, // ULB
//    new[]{ Mapping.UP, Mapping.BACK,  Mapping.RIGHT }, // UBR
//    new[]{ Mapping.DOWN,Mapping.FRONT,Mapping.RIGHT }, // DFR
//    new[]{ Mapping.DOWN,Mapping.LEFT, Mapping.FRONT }, // DLF
//    new[]{ Mapping.DOWN,Mapping.BACK, Mapping.LEFT  }, // DBL
//    new[]{ Mapping.DOWN,Mapping.RIGHT,Mapping.BACK  }  // DRB
//};

//    static readonly int[][] edge_colors =
//    {
//    new[]{ Mapping.UP,   Mapping.RIGHT }, // UR
//    new[]{ Mapping.UP,   Mapping.FRONT }, // UF
//    new[]{ Mapping.UP,   Mapping.LEFT  }, // UL
//    new[]{ Mapping.UP,   Mapping.BACK  }, // UB
//    new[]{ Mapping.DOWN, Mapping.RIGHT }, // DR
//    new[]{ Mapping.DOWN, Mapping.FRONT }, // DF
//    new[]{ Mapping.DOWN, Mapping.LEFT  }, // DL
//    new[]{ Mapping.DOWN, Mapping.BACK  }, // DB
//    new[]{ Mapping.FRONT,Mapping.RIGHT }, // FR
//    new[]{ Mapping.FRONT,Mapping.LEFT  }, // FL
//    new[]{ Mapping.BACK, Mapping.LEFT  }, // BL
//    new[]{ Mapping.BACK, Mapping.RIGHT }  // BR
//};

//    // ===============================================

//    void BuildFromState(CubeState state)
//    {
//        for (int idx = 0; idx < 27; idx++)
//        {
//            Vector3Int pos = Mapping.IdxToPos(idx);
//            Cubie c = state[idx];

//            int non_black = CountVisibleFaces(c);

//            if (non_black == 3)
//                ProcessCorner(c, pos);
//            else if (non_black == 2)
//                ProcessEdge(c, pos);
//        }
//    }

//    int CountVisibleFaces(Cubie c)
//    {
//        int cnt = 0;
//        for (int i = 0; i < 6; i++)
//            if (c[i] != i) cnt++;
//        return cnt;
//    }

//    void ProcessCorner(Cubie c, Vector3Int pos)
//    {
//        int[] colors = GetColors(c, 3);

//        for (int i = 0; i < 8; i++)
//        {
//            if (MatchSet(colors, corner_colors[i]))
//            {
//                corner_perm[i] = i;

//                // orientation
//                if (c[Mapping.UP] == Mapping.UP || c[Mapping.DOWN] == Mapping.DOWN)
//                    corner_ori[i] = 0;
//                else if (c[Mapping.FRONT] == Mapping.UP || c[Mapping.FRONT] == Mapping.DOWN)
//                    corner_ori[i] = 1;
//                else
//                    corner_ori[i] = 2;

//                return;
//            }
//        }
//    }

//    void ProcessEdge(Cubie c, Vector3Int pos)
//    {
//        int[] colors = GetColors(c, 2);

//        for (int i = 0; i < 12; i++)
//        {
//            if (MatchSet(colors, edge_colors[i]))
//            {
//                edge_perm[i] = i;

//                if (c[Mapping.UP] == Mapping.UP ||
//                    c[Mapping.DOWN] == Mapping.DOWN ||
//                    c[Mapping.FRONT] == Mapping.FRONT ||
//                    c[Mapping.BACK] == Mapping.BACK)
//                    edge_ori[i] = 0;
//                else
//                    edge_ori[i] = 1;

//                return;
//            }
//        }
//    }

//    int[] GetColors(Cubie c, int n)
//    {
//        int[] res = new int[n];
//        int k = 0;
//        for (int i = 0; i < 6; i++)
//            if (c[i] != i)
//                res[k++] = c[i];
//        return res;
//    }

//    bool MatchSet(int[] a, int[] b)
//    {
//        foreach (int x in a)
//        {
//            bool found = false;
//            foreach (int y in b)
//                if (x == y) found = true;
//            if (!found) return false;
//        }
//        return true;
//    }
//}

//using System;
//using System.Collections.Generic;

///// <summary>
///// Cubie-level representation of a Rubik's Cube used by the Kociemba solver.
/////
///// Stores only:
///// - Corner permutation + orientation
///// - Edge permutation + orientation
/////
///// This model is independent from Unity and colors.
///// </summary>
//public class CubieModel
//{
//    // ----------- Corner & Edge storage -----------

//    public int[] corner_perm = new int[8];
//    public int[] corner_ori = new int[8];

//    public int[] edge_perm = new int[12];
//    public int[] edge_ori = new int[12];

//    // ----------- Corner indices (standard) -----------

//    // 0 URF, 1 UFL, 2 ULB, 3 UBR, 4 DFR, 5 DLF, 6 DBL, 7 DRB

//    // ----------- Edge indices (standard) -----------

//    // 0 UR,1 UF,2 UL,3 UB,4 DR,5 DF,6 DL,7 DB,8 FR,9 FL,10 BL,11 BR

//    // ----------- Orientation delta tables -----------

//    static int[,] corner_ori_delta = new int[6, 8]
//    {
//        // U
//        {0,0,0,0,0,0,0,0},
//        // D
//        {0,0,0,0,0,0,0,0},
//        // F
//        {1,2,0,0,2,1,0,0},
//        // B
//        {0,0,2,1,0,0,1,2},
//        // R
//        {2,0,0,1,1,0,0,2},
//        // L
//        {0,1,2,0,0,2,1,0},
//    };

//    static int[,] edge_ori_delta = new int[6, 12]
//    {
//        // U
//        {0,0,0,0,0,0,0,0,0,0,0,0},
//        // D
//        {0,0,0,0,0,0,0,0,0,0,0,0},
//        // F
//        {0,1,0,0,0,1,0,0,1,1,0,0},
//        // B
//        {0,0,0,1,0,0,0,1,0,0,1,1},
//        // R
//        {0,0,0,0,0,0,0,0,0,0,0,0},
//        // L
//        {0,0,0,0,0,0,0,0,0,0,0,0},
//    };

//    // ----------- Constructors -----------

//    /// <summary>
//    /// Create solved cubie model.
//    /// </summary>
//    public CubieModel()
//    {
//        for (int i = 0; i < 8; i++)
//        {
//            corner_perm[i] = i;
//            corner_ori[i] = 0;
//        }
//        for (int i = 0; i < 12; i++)
//        {
//            edge_perm[i] = i;
//            edge_ori[i] = 0;
//        }
//    }

//    /// <summary>
//    /// Extract cubie model from your CubeState (color-based).
//    /// </summary>
//    public CubieModel(CubeState state) : this()
//    {
//        ExtractFromState(state);
//    }

//    public CubieModel Clone()
//    {
//        CubieModel c = new CubieModel();
//        Array.Copy(corner_perm, c.corner_perm, 8);
//        Array.Copy(corner_ori, c.corner_ori, 8);
//        Array.Copy(edge_perm, c.edge_perm, 12);
//        Array.Copy(edge_ori, c.edge_ori, 12);
//        return c;
//    }

//    // ----------- Move application -----------

//    public void ApplyMove(Move m)
//    {
//        int face = m.face;
//        int times = m.is180 ? 2 : 1;
//        if (m.rev) times = 4 - times;

//        for (int t = 0; t < times; t++)
//            ApplyQuarterTurn(face);
//    }

//    void ApplyQuarterTurn(int face)
//    {
//        // permute corners
//        CycleCorners(face);

//        // permute edges
//        CycleEdges(face);

//        // update orientations
//        for (int i = 0; i < 8; i++)
//            corner_ori[i] = (corner_ori[i] + corner_ori_delta[face, i]) % 3;

//        for (int i = 0; i < 12; i++)
//            edge_ori[i] ^= edge_ori_delta[face, i];
//    }

//    // ----------- Permutation cycles -----------

//    void CycleCorners(int face)
//    {
//        int[][] cycles = {
//            new int[]{0,3,2,1}, // U
//            new int[]{4,5,6,7}, // D
//            new int[]{0,1,5,4}, // F
//            new int[]{3,7,6,2}, // B
//            new int[]{0,4,7,3}, // R
//            new int[]{1,2,6,5}, // L
//        };

//        Cycle(corner_perm, cycles[face]);
//        Cycle(corner_ori, cycles[face]);
//    }

//    void CycleEdges(int face)
//    {
//        int[][] cycles = {
//            new int[]{0,3,2,1}, // U
//            new int[]{4,5,6,7}, // D
//            new int[]{1,9,5,8}, // F
//            new int[]{3,11,7,10}, // B
//            new int[]{0,8,4,11}, // R
//            new int[]{2,10,6,9}, // L
//        };

//        Cycle(edge_perm, cycles[face]);
//        Cycle(edge_ori, cycles[face]);
//    }

//    void Cycle(int[] arr, int[] idx)
//    {
//        int temp = arr[idx[0]];
//        for (int i = 0; i < 3; i++)
//            arr[idx[i]] = arr[idx[i + 1]];
//        arr[idx[3]] = temp;
//    }

//    // ----------- Extraction from CubeState -----------
//    // face index: FRONT=0, UP=1, RIGHT=2, BACK=3, DOWN=4, LEFT=5

//    static readonly int[][] corner_colors =
//    {
//    new[]{1,2,0}, // URF
//    new[]{1,0,5}, // UFL
//    new[]{1,5,3}, // ULB
//    new[]{1,3,2}, // UBR
//    new[]{4,0,2}, // DFR
//    new[]{4,5,0}, // DLF
//    new[]{4,3,5}, // DBL
//    new[]{4,2,3}, // DRB
//    };

//    static readonly int[][] edge_colors =
//    {
//    new[]{1,2}, // UR
//    new[]{1,0}, // UF
//    new[]{1,5}, // UL
//    new[]{1,3}, // UB
//    new[]{4,2}, // DR
//    new[]{4,0}, // DF
//    new[]{4,5}, // DL
//    new[]{4,3}, // DB
//    new[]{0,2}, // FR
//    new[]{0,5}, // FL
//    new[]{3,5}, // BL
//    new[]{3,2}, // BR
//    };

//    static readonly int[] corner_pos = { 0, 2, 20, 18, 6, 8, 26, 24 };
//    //static readonly int[] edge_pos = { 1, 11, 19, 9, 15, 7, 17, 25, 3, 5, 23, 21 };
//    static readonly int[] edge_pos = { 9, 1, 11, 19, 15, 7, 17, 25, 3, 5, 23, 21 };

//    bool MatchSet(int[] a, int[] b)
//    {
//        foreach (int x in a)
//        {
//            bool found = false;
//            foreach (int y in b)
//                if (x == y) found = true;
//            if (!found) return false;
//        }
//        return true;
//    }

//    int GetCornerOri(int[] faces)
//    {
//        // U/D nằm ở vị trí nào trong 3 mặt
//        for (int i = 0; i < 3; i++)
//        {
//            if (faces[i] == Mapping.UP || faces[i] == Mapping.DOWN)
//                return i % 3;
//        }
//        return 0;
//    }

//    int GetEdgeOri(int f1, int f2)
//    {
//        if (f1 == Mapping.UP || f1 == Mapping.DOWN)
//            return 0;
//        if (f2 == Mapping.UP || f2 == Mapping.DOWN)
//            return 1;

//        // Với FR,FL,BL,BR: xét F/B
//        if (f1 == Mapping.FRONT || f1 == Mapping.BACK)
//            return 0;

//        return 1;
//    }
//    void ExtractFromState(CubeState state)
//    {
//        // ----- Corners -----
//        for (int i = 0; i < 8; i++)
//        {
//            Cubie c = state[corner_pos[i]];

//            int[] faces = new int[3];
//            int idx = 0;
//            //for (int f = 0; f < 6; f++)
//            //    if (c[f] == f)
//            //        faces[idx++] = f;
//            for (int _f = 0; _f < 3; _f++)
//            {
//                int f = (4 - _f) % 3;
//                if (c[f] != -1)
//                {
//                    faces[idx++] = c[f];
//                }
//                if (c[f + 3] != -1)
//                {
//                    faces[idx++] = c[f];
//                }
//            }
//            for (int j = 0; j < 8; j++)
//            {
//                if (MatchSet(faces, corner_colors[j]))
//                {
//                    corner_perm[i] = j;
//                    corner_ori[i] = GetCornerOri(faces);
//                    break;
//                }
//            }
//        }

//        // ----- Edges -----
//        for (int i = 0; i < 12; i++)
//        {
//            Cubie c = state[edge_pos[i]];

//            int[] faces = new int[2];
//            int idx = 0;
//            //for (int f = 0; f < 6; f++)
//            //    if (c[f] == f)
//            //        faces[idx++] = f;
//            for (int _f = 0; _f < 3; _f++)
//            {
//                int f = (4 - _f) % 3;
//                if (c[f] != -1)
//                {
//                    faces[idx++] = c[f];
//                }
//                if (c[f + 3] != -1)
//                {
//                    faces[idx++] = c[f];
//                }
//            }
//            for (int j = 0; j < 12; j++)
//            {
//                if (MatchSet(faces, edge_colors[j]))
//                {
//                    edge_perm[i] = j;
//                    edge_ori[i] = GetEdgeOri(faces[0], faces[1]);
//                    break;
//                }
//            }
//        }
//    }
//    // ----------- CubieModel to CubeState -----------
//    int[] RotateCorner(int[] colors, int ori)
//    {
//        int[] res = new int[3];
//        for (int i = 0; i < 3; i++)
//            res[i] = colors[(i + 3 - ori) % 3];
//        return res;
//    }

//    int[] RotateEdge(int[] colors, int ori)
//    {
//        if (ori == 0) return colors;
//        return new int[] { colors[1], colors[0] };
//    }

//    public CubeState ToCubeState()
//    {
//        CubeState state = new CubeState();

//        // Reset tất cả cubie về màu "đen"
//        for (int i = 0; i < 27; i++)
//            for (int f = 0; f < 6; f++)
//                state[i][f] = -1;

//        // ----- Corners -----
//        for (int i = 0; i < 8; i++)
//        {
//            int cubie_index = corner_perm[i];
//            int ori = corner_ori[i];

//            int[] colors = RotateCorner(corner_colors[cubie_index], ori);
//            Cubie c = state[corner_pos[i]];

//            foreach (int face in colors)
//                c[face] = face;
//        }

//        // ----- Edges -----
//        for (int i = 0; i < 12; i++)
//        {
//            int cubie_index = edge_perm[i];
//            int ori = edge_ori[i];

//            int[] colors = RotateEdge(edge_colors[cubie_index], ori);
//            Cubie c = state[edge_pos[i]];

//            foreach (int face in colors)
//                c[face] = face;
//        }

//        // ----- Centers -----
//        for (int face = 0; face < 6; face++)
//        {
//            int center = Mapping.GetCenter(face);
//            state[center][face] = face;
//        }

//        return state;
//    }
//    // ----------- Phase 1 condition -----------

//    public bool IsG1()
//    {
//        for (int i = 0; i < 8; i++)
//            if (corner_ori[i] != 0) return false;

//        for (int i = 0; i < 12; i++)
//            if (edge_ori[i] != 0) return false;

//        // TODO: udslice
//        return true;
//    }
//}