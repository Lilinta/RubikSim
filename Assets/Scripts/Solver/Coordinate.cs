using System;

/// <summary>
/// Static utility class to encode/decode CubieModel states into coordinate indices.
/// These coordinates are used for Move and Pruning tables in the Two-Phase algorithm.
/// </summary>
public static class Coordinate
{
    /// <summary>
    /// fact[i] = i-th factorial
    /// </summary>
    static int[] fact = new int[13];
    /// <summary>
    /// comb[n, k] = nCK
    /// </summary>
    static int[,] comb = new int[13, 13];

    /// <summary>
    /// Precomputes factorials and combinations (nCk).
    /// </summary>
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

    // --- PHASE 1 ENCODING ---

    /// <summary> Encodes corner orientation into an index (0..2186). </summary>
    public static int EncodeCO(CubieModel c)
    {
        int idx = 0;
        for (int i = 0; i < 7; i++)
            idx = idx * 3 + c.corner_ori[i];
        return idx;
    }

    /// <summary> Decodes index back into corner orientation. </summary>
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

    /// <summary> Encodes edge orientation into an index (0..2047). </summary>
    public static int EncodeEO(CubieModel c)
    {
        int idx = 0;
        for (int i = 0; i < 11; i++)
            idx = idx * 2 + c.edge_ori[i];
        return idx;
    }

    /// <summary> Decodes index back into edge orientation. </summary>
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

    /// <summary> Encodes the position of the 4 edges in the UD-slice (0..494). </summary>
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

    /// <summary> Decodes UD-slice position into edge permutation. </summary>
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

    /// <summary> Check if a edge is in UD-Slice </summary>
    static bool IsSliceEdge(int e)
    {
        return e >= 8; // FR, FL, BL, BR
    }

    // --- PHASE 2 ENCODING ---

    /// <summary> Encodes corner permutation (8!) into index (0..40319). </summary>
    public static int EncodeCornerPerm(CubieModel c)
    {
        return PermToIdx(c.corner_perm, 8);
    }

    /// <summary> Decodes index into corner permutation. </summary>
    public static void DecodeCornerPerm(int idx, CubieModel c)
    {
        IdxToPerm(idx, c.corner_perm, 8);
    }

    /// <summary> Encodes permutation of 8 edges not in UD-slice. </summary>
    public static int EncodeEdgePerm(CubieModel c)
    {
        int[] arr = new int[8];
        int k = 0;
        for (int i = 0; i < 12; i++)
            if (c.edge_perm[i] < 8)
                arr[k++] = c.edge_perm[i];

        return PermToIdx(arr, 8);
    }

    /// <summary> Decodes index into the 8 non-slice edges. </summary>
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

    /// <summary> Encodes permutation of the 4 UD-slice edges. </summary>

    public static int EncodeUDSlice2(CubieModel c)
    {
        int[] arr = new int[4];
        int k = 0;
        for (int i = 0; i < 12; i++)
            if (IsSliceEdge(c.edge_perm[i]))
                arr[k++] = c.edge_perm[i] - 8;

        return PermToIdx(arr, 4);
    }

    /// <summary> Decodes index into UD-slice edge permutation. </summary>
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

    /// <summary> Converts a permutation to its factoradic index. </summary>
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

    /// <summary> Converts a factoradic index back to a permutation. </summary>
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