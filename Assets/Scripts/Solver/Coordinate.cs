using System;

public static class Coordinate
{
    static int[] fact = new int[13];
    static int[,] comb = new int[13, 13];

    // ---------- Static constructor ----------
    static Coordinate()
    {
        // factorial
        fact[0] = 1;
        for (int i = 1; i < fact.Length; i++)
            fact[i] = fact[i - 1] * i;

        // combinations nCk
        for (int n = 0; n < 13; n++)
        {
            comb[n, 0] = comb[n, n] = 1;
            for (int k = 1; k < n; k++)
                comb[n, k] = comb[n - 1, k - 1] + comb[n - 1, k];
        }
    }

    // =========================================================
    // -------------------- PHASE 1 -----------------------------
    // =========================================================

    public static int EncodeCO(CubieModel c)
    {
        int idx = 0;
        for (int i = 0; i < 7; i++)
            idx = idx * 3 + c.corner_ori[i];
        return idx;
    }

    public static void DecodeCO(int idx, CubieModel c)
    {
        int sum = 0;
        for (int i = 6; i >= 0; i--)
        {
            c.corner_ori[i] = idx % 3;
            sum += c.corner_ori[i];
            idx /= 3;
        }
        c.corner_ori[7] = (3 - sum % 3) % 3;
    }

    public static int EncodeEO(CubieModel c)
    {
        int idx = 0;
        for (int i = 0; i < 11; i++)
            idx = idx * 2 + c.edge_ori[i];
        return idx;
    }

    public static void DecodeEO(int idx, CubieModel c)
    {
        int sum = 0;
        for (int i = 10; i >= 0; i--)
        {
            c.edge_ori[i] = idx % 2;
            sum += c.edge_ori[i];
            idx /= 2;
        }
        c.edge_ori[11] = (2 - sum % 2) % 2;
    }

    // Encode which 4 edges are in UD-slice (FR,FL,BL,BR)
    public static int EncodeUDSlice(CubieModel c)
    {
        int idx = 0;
        int r = 4;

        for (int i = 11; i >= 0; i--)
        {
            if (IsSliceEdge(c.edge_perm[i]))
            {
                idx += comb[i, r];
                r--;
            }
        }
        return idx;
    }
   
    public static void DecodeUDSlice(int idx, CubieModel c)
    {
        bool[] mark = new bool[12];
        int r = 4;

        for (int i = 11; i >= 0; i--)
        {
            if (idx >= comb[i, r])
            {
                mark[i] = true;
                idx -= comb[i, r];
                r--;
            }
        }

        int sliceEdge = 8;
        int otherEdge = 0;

        for (int i = 0; i < 12; i++)
        {
            if (mark[i])
                c.edge_perm[i] = sliceEdge++;
            else
                c.edge_perm[i] = otherEdge++;
        }
    }

    static bool IsSliceEdge(int e)
    {
        return e >= 8; // FR, FL, BL, BR
    }

    // =========================================================
    // -------------------- PHASE 2 -----------------------------
    // =========================================================

    public static int EncodeCornerPerm(CubieModel c)
    {
        return PermToIdx(c.corner_perm, 8);
    }

    public static void DecodeCornerPerm(int idx, CubieModel c)
    {
        IdxToPerm(idx, c.corner_perm, 8);
    }

    // Only 8 edges (UR,UF,UL,UB,DR,DF,DL,DB)
    public static int EncodeEdgePerm(CubieModel c)
    {
        int[] arr = new int[8];
        int k = 0;
        for (int i = 0; i < 12; i++)
            if (c.edge_perm[i] < 8)
                arr[k++] = c.edge_perm[i];

        return PermToIdx(arr, 8);
    }

    public static void DecodeEdgePerm(int idx, CubieModel c)
    {
        int[] arr = new int[8];
        IdxToPerm(idx, arr, 8);

        int k = 0;
        for (int i = 0; i < 12; i++)
        {
            if (c.edge_perm[i] < 8)
                c.edge_perm[i] = arr[k++];
        }
    }

    public static int EncodeUDSlice2(CubieModel c)
    {
        int[] arr = new int[4];
        int k = 0;
        for (int i = 0; i < 12; i++)
            if (IsSliceEdge(c.edge_perm[i]))
                arr[k++] = c.edge_perm[i] - 8;

        return PermToIdx(arr, 4);
    }

    public static void DecodeUDSlice2(int idx, CubieModel c)
    {
        int[] arr = new int[4];
        IdxToPerm(idx, arr, 4);

        int k = 0;
        for (int i = 0; i < 12; i++)
        {
            if (c.edge_perm[i] >= 8)
                c.edge_perm[i] = arr[k++] + 8;
        }
    }

    // =========================================================
    // ----------- Permutation ↔ Index (factoradic) ------------
    // =========================================================

    static int PermToIdx(int[] perm, int n)
    {
        int idx = 0;
        bool[] used = new bool[n];

        for (int i = 0; i < n; i++)
        {
            int cnt = 0;
            for (int j = 0; j < perm[i]; j++)
                if (!used[j]) cnt++;

            idx += cnt * fact[n - i - 1];
            used[perm[i]] = true;
        }
        return idx;
    }

    static void IdxToPerm(int idx, int[] perm, int n)
    {
        bool[] used = new bool[n];

        for (int i = 0; i < n; i++)
        {
            int f = fact[n - i - 1];
            int cnt = idx / f;
            idx %= f;

            int j = 0, k = 0;
            while (true)
            {
                if (!used[j])
                {
                    if (k == cnt) break;
                    k++;
                }
                j++;
            }

            perm[i] = j;
            used[j] = true;
        }
    }
}

//using System.Collections.Generic;
//using static Mapping;
//using static LexicoDP;

//public static class Coordinate
//{
//    // Phase 1
//    public static int EncodeCO(CubieModel c)
//    {
//        int co = 0;
//        int pw = 1;
//        for (int i = 0; i < c.corner_ori.Length - 1; i++)
//        {
//            co += c.corner_ori[i] * pw;
//            pw *= 3;
//        }
//        return co;
//    }
//    public static int EncodeEO(CubieModel c)
//    {
//        int eo = 0;
//        int pw = 1;
//        for (int i = 0; i < c.edge_ori.Length - 1; i++)
//        {
//            eo += c.edge_ori[i] * pw;
//            pw *= 2;
//        }
//        return eo;
//    }
//    public static int EncodeUDSlice(CubieModel c);

//    public static void DecodeCO(int co, CubieModel c)
//    {
//        int[] corner_ori = new int[8];
//        int sum = 0;
//        for (int i = 0; i < corner_ori.Length - 1; i++)
//        {
//            corner_ori[i] = co % 3;
//            co /= 3;
//            sum = (sum + corner_ori[i]) % 3;
//        }
//        corner_ori[corner_ori.Length - 1] = (3 - sum) % 3;

//        for (int i = 0; i < corner_ori.Length; i++)
//        {
//            c.corner_ori[i] = corner_ori[i];
//        }
//    }
//    public static void DecodeEO(int eo, CubieModel c)
//    {
//        int[] edge_ori = new int[12];
//        int sum = 0;
//        for (int i = 0; i < edge_ori.Length - 1; i++)
//        {
//            edge_ori[i] = eo % 2;
//            eo /= 2;
//            sum = (sum + edge_ori[i]) % 2;
//        }
//        edge_ori[edge_ori.Length - 1] = (2 - sum) % 2;

//        for (int i = 0; i < edge_ori.Length; i++)
//        {
//            c.edge_ori[i] = edge_ori[i];
//        }
//    }
//    public static void DecodeUDSlice(int uds, CubieModel c)
//    {

//    }

//    // Phase 2
//    public static int EncodeCornerPerm(CubieModel c);
//    public static int EncodeEdgePerm(CubieModel c);
//    public static void DecodeCornerPerm(int cp, CubieModel c);
//    public static void DecodeEdgePerm(int ep, CubieModel c);
//}

//using System.Collections.Generic;
//using UnityEngine;
//using static Mapping;
//using static LexicoDP;
//public static class Coordinate
//{
//    public static List<int> GetCornerOri(CubeState state)
//    {
//        List<int> corner_ori = new List<int>(8); // corner_ori[i] = orientation of i-th corner
//        foreach (int i in CORNER_LIST)
//        {
//            Vector3Int pos = IdxToPos(i);
//            int x_color = state[i][pos.x > 0 ? 0 : 3];
//            int y_color = state[i][pos.y > 0 ? 1 : 4];
//            int z_color = state[i][pos.z > 0 ? 2 : 5];
//            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
//            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
//            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
//            int idx = 4 * (back ? 1 : 0) + 2 * (down ? 1 : 0) + (left ? 1 : 0);

//            if (y_color % 3 == 1)
//            {
//                corner_ori[idx] = 0;
//            }
//            else if (x_color % 3 == 1)
//            {
//                corner_ori[idx] = 1;
//            }
//            else
//            {
//                corner_ori[idx] = 2;
//            }
//        }
//        return corner_ori;
//    }
//    public static int GetCornerOriIdx(CubeState state)
//    {
//        List<int> corner_ori = GetCornerOri(state);
//        int co = 0;
//        int pw = 1;
//        for (int i = 0; i < corner_ori.Count-1; i++)
//        {
//            co += corner_ori[i] * pw;
//            pw *= 3;
//        }
//        return co;
//    }

//    public static void ApplyCornerOriIdx(CubeState state, int co)
//    {
//        List<int> corner_ori = new List<int>(8);
//        int sum = 0;
//        for (int i = 0; i < corner_ori.Count-1; i++)
//        {
//            corner_ori[i] = co % 3;
//            co /= 3;
//            sum += corner_ori[i];
//        }
//        corner_ori[corner_ori.Count - 1] = (3 - sum) % 3;

//        foreach (int i in CORNER_LIST)
//        {
//            Vector3Int pos = IdxToPos(i);
//            int x_color = state[i][pos.x > 0 ? 0 : 3];
//            int y_color = state[i][pos.y > 0 ? 1 : 4];
//            int z_color = state[i][pos.z > 0 ? 2 : 5];
//            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
//            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
//            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
//            int idx = 4 * (back ? 1 : 0) + 2 * (down ? 1 : 0) + (left ? 1 : 0);

//            if (y_color % 3 == 1)
//            {
//                corner_ori[idx] = 0;
//            }
//            else if (x_color % 3 == 1)
//            {
//                corner_ori[idx] = 1;
//            }
//            else
//            {
//                corner_ori[idx] = 2;
//            }
//        }
//    }
//    public static List<int> GetCornerPerm(CubeState state)
//    {
//        List<int> corner_perm = new List<int>(8); // corner_perm[i] = the index of the corner in the i-th position of the cube
//        foreach (int i in CORNER_LIST)
//        {
//            Vector3Int pos = IdxToPos(i);
//            int x_color = state[i][pos.x > 0 ? 0 : 3];
//            int y_color = state[i][pos.y > 0 ? 1 : 4];
//            int z_color = state[i][pos.z > 0 ? 2 : 5];
//            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
//            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
//            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
//            int idx = 4 * (back ? 1 : 0) + 2 * (down ? 1 : 0) + (left ? 1 : 0);

//            corner_perm[LOCAL_CUBIE_IDX[i]] = idx;
//        }
//        return corner_perm;
//    }
//    public static int GetCornerPermIdx(CubeState state) {
//        List<int> corner_perm = GetCornerPerm(state);
//        return PermToIdx(corner_perm);
//    }

//    public static void ApplyCornerPermIdx(CubeState state, int cp)
//    {

//    }

//    public static List<int> GetEdgeOri(CubeState state)
//    {
//        List<int> edge_ori = new List<int>(12); // edge_ori[i] = orientation of i-th edge
//        foreach (int i in EDGE_LIST)
//        {
//            Vector3Int pos = IdxToPos(i);
//            int x_color = -1;
//            int y_color = -1;
//            int z_color = -1;
//            if (pos.x != 0) x_color = state[i][pos.x > 0 ? 0 : 3];
//            if (pos.y != 0) y_color = state[i][pos.y > 0 ? 1 : 4];
//            if (pos.z != 0) z_color = state[i][pos.z > 0 ? 2 : 5];
//            bool front = (x_color == 0) || (y_color == 0) || (z_color == 0);
//            bool up = (x_color == 1) || (y_color == 1) || (z_color == 1);
//            bool right = (x_color == 2) || (y_color == 2) || (z_color == 2);
//            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
//            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
//            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);

//            int idx = -1;
//            if (front)
//            {
//                if (up) idx = 1;
//                else if (right) idx = 3;
//                else if (left) idx = 5;
//                else idx = 7;
//            }
//            else if (!front && !back)
//            {
//                if (up)
//                {
//                    if (right) idx = 9;
//                    else idx = 11;
//                }
//                else
//                {
//                    if (right) idx = 15;
//                    else idx = 17;
//                }
//            }
//            else
//            {
//                if (up) idx = 19;
//                else if (right) idx = 21;
//                else if (left) idx = 23;
//                else idx = 25;
//            }
//            idx = LOCAL_CUBIE_IDX[idx];

//            if (up || down)
//            {
//                if (y_color % 3 == 1) edge_ori[idx] = 0;
//                else edge_ori[idx] = 1;
//            }
//            else
//            {
//                if (x_color % 3 == 0) edge_ori[idx] = 0;
//                else edge_ori[idx] = 1;
//            }
//        }
//        return edge_ori;
//    }
//    public static int GetEdgeOriIdx(CubeState state)
//    {
//        List<int> edge_ori = GetEdgeOri(state);
//        int eo = 0;
//        int pw = 1;
//        for (int i = 0; i < edge_ori.Count-1; i++)
//        {
//            eo += edge_ori[i] * pw;
//            pw *= 2;
//        }
//        return eo;
//    }

//    public static void ApplyEdgeOriIdx(CubeState state, int eo)
//    {

//    }

//    public static List<int> GetEdgePerm(CubeState state)
//    {
//        List<int> edge_perm = new List<int>(12); // edge_ori[i] = orientation of i-th edge
//        foreach (int i in EDGE_LIST)
//        {
//            Vector3Int pos = IdxToPos(i);
//            int x_color = -1;
//            int y_color = -1;
//            int z_color = -1;
//            if (pos.x != 0) x_color = state[i][pos.x > 0 ? 0 : 3];
//            if (pos.y != 0) y_color = state[i][pos.y > 0 ? 1 : 4];
//            if (pos.z != 0) z_color = state[i][pos.z > 0 ? 2 : 5];
//            bool front = (x_color == 0) || (y_color == 0) || (z_color == 0);
//            bool up = (x_color == 1) || (y_color == 1) || (z_color == 1);
//            bool right = (x_color == 2) || (y_color == 2) || (z_color == 2);
//            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
//            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
//            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);

//            int idx = -1;
//            if (front)
//            {
//                if (up) idx = 1;
//                else if (right) idx = 3;
//                else if (left) idx = 5;
//                else idx = 7;
//            }
//            else if (!front && !back)
//            {
//                if (up)
//                {
//                    if (right) idx = 9;
//                    else idx = 11;
//                }
//                else
//                {
//                    if (right) idx = 15;
//                    else idx = 17;
//                }
//            }
//            else
//            {
//                if (up) idx = 19;
//                else if (right) idx = 21;
//                else if (left) idx = 23;
//                else idx = 25;
//            }
//            idx = LOCAL_CUBIE_IDX[idx];

//            edge_perm[LOCAL_CUBIE_IDX[i]] = idx;
//        }
//        return edge_perm;
//    }
//    public static int GetEdgePermIdx(CubeState state)
//    {
//        List<int> edge_perm = GetEdgePerm(state);
//        return PermToIdx(edge_perm);
//    }

//    public static void ApplyEdgePermIdx(CubeState state, int ep)
//    {

//    }

//    public static List<int> GetMiddleEdgeComb(CubeState state)
//    {
//        List<int> edge_perm = GetEdgePerm(state);
//        List<int> middle_edge_comb = new List<int>();
//        for (int i = 0; i < edge_perm.Count; i++)
//        {
//            int idx = edge_perm[i];
//            if (idx == 3 || idx == 5 || idx == 21 || idx == 23)
//            {
//                middle_edge_comb.Add(i);
//            }
//        }
//        return middle_edge_comb;
//    }
//    public static int GetMiddleEdgeCombIdx(CubeState state)
//    {
//        List<int> middle_edge_comb = GetMiddleEdgeComb(state);
//        return CombToIdx(middle_edge_comb, 12);
//    }

//    public static void ApplyMiddleEdgeCombIdx(CubeState state, int mec)
//    {

//    }
//}

