using System;
using UnityEngine;
using static Mapping;

/// <summary>
/// Mathematical representation of the Rubik's Cube based on cubie permutation and orientation.
/// This model is used by the solver (Kociemba/Two-Phase algorithm) rather than for rendering.
/// </summary>
public class CubieModel
{
    /// <summary> Permutation of the 8 corner cubies. </summary>
    public int[] corner_perm = new int[8];
    /// <summary> Orientation of the 8 corner cubies (0, 1, or 2). </summary>
    public int[] corner_ori = new int[8];

    /// <summary> Permutation of the 12 edge cubies. </summary>
    public int[] edge_perm = new int[12];
    /// <summary> Orientation of the 12 edge cubies (0 or 1). </summary>
    public int[] edge_ori = new int[12];

    /// <summary> Indices of corners affected by each face rotation. </summary>
    static int[][] FACE_CORNERS =
    {
        new int[] { 0, 4, 5, 1 },
        new int[] { 0, 1, 2, 3 },
        new int[] { 0, 3, 7, 4 },
        new int[] { 3, 2, 6, 7 },
        new int[] { 5, 4, 7, 6 },
        new int[] { 2, 1, 5, 6 },
    };

    /// <summary> Indices of edges affected by each face rotation. </summary>
    static int[][] FACE_EDGES =
    {
        new int[] { 1, 8, 5, 9 },
        new int[] { 0, 1, 2, 3 },
        new int[] { 0, 11, 4, 8 },
        new int[] { 11, 3, 10, 7 },
        new int[] { 5, 4, 7, 6 },
        new int[] { 2, 9, 6, 10 }
    };

    /// <summary> Predefined orientation changes for corners after a 90-degree clockwise turn. </summary>
    static int[][] CORNER_ORI_DELTA =
    {
        new int[] { 2, 1, 0, 0, 1, 2, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 1, 0, 0, 2, 2, 0, 0, 1 },
        new int[] { 0, 0, 2, 1, 0, 0, 1, 2 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 2, 1, 0, 0, 1, 2, 0 },
    };

    /// <summary> Predefined orientation changes for edges after a 90-degree clockwise turn. </summary>
    static int[][] EDGE_ORI_DELTA =
    {
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0 },
    };

    /// <summary>
    /// Initializes a new CubieModel in the solved state.
    /// </summary>
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

    /// <summary>
    /// Converts a standard CubeState into a mathematical CubieModel.
    /// </summary>
    /// <param name="state">The logical cube state to convert.</param>
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

    /// <summary>
    /// Creates a deep copy of the CubieModel.
    /// </summary>
    /// <returns>A cloned CubieModel.</returns>
    public CubieModel Clone()
    {
        CubieModel c = new CubieModel();

        Array.Copy(corner_perm, c.corner_perm, 8);
        Array.Copy(corner_ori, c.corner_ori, 8);
        Array.Copy(edge_perm, c.edge_perm, 12);
        Array.Copy(edge_ori, c.edge_ori, 12);

        return c;
    }

    /// <summary>
    /// Applies a move to the model by repeating a quarter turn.
    /// </summary>
    /// <param name="move">The move to apply.</param>
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

    /// <summary>
    /// Internal method to apply a 90-degree clockwise rotation to a specific face.
    /// </summary>
    /// <param name="face">Face index.</param>
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

    /// <summary>
    /// Performs a 4-cycle permutation on an array based on given indices.
    /// </summary>
    /// <param name="arr">The array to modify.</param>
    /// <param name="p">The indices defining the cycle.</param>
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

    /// <summary>
    /// Checks if the current model state is mathematically solvable.
    /// Validates corner orientation sum, edge orientation sum, and permutation parity.
    /// </summary>
    /// <returns>True if solvable.</returns>
    public bool IsSolvable()
    {
        int sum = 0;
        for (int i = 0; i < corner_ori.Length; i++)
        {
            sum += corner_ori[i];
        }
        sum %= 3;
        if (sum != 0) return false;
        sum = 0;
        for (int i = 0; i < edge_ori.Length; i++)
        {
            sum += edge_ori[i];
        }
        sum %= 2;
        if (sum != 0) return false;
        if (PermParity(corner_perm) != PermParity(edge_perm)) return false;
        return true;
    }

    /// <summary>
    /// Calculates the parity of a permutation.
    /// </summary>
    /// <param name="arr">Permutation array.</param>
    /// <returns>True if odd parity, false if even.</returns>
    private bool PermParity(int[] arr) 
    {
        int n = arr.Length;
        bool[] mark = new bool[n];
        for (int i = 0; i < n; i++)
        {
            mark[i] = false;
        }
        int res = 0;
        for (int i = 0; i < n; i++)
        {
            if (mark[i]) continue;
            int u = i;
            int cnt = 0;
            while (!mark[u])
            {
                cnt++;
                mark[u] = true;
                u = arr[u];
            }
            res += cnt - 1;
        }
        return res % 2 == 1;
    }
}