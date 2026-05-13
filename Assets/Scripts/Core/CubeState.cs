using System;
using UnityEngine;
using static Mapping;

/// <summary>
/// Represents the full logical state of a 3x3 Rubik's Cube.
/// 
/// The cube consists of 27 cubies stored in a fixed-size array.
/// State updates occur in two stages:
/// 1. Permuting cubie positions (shuffling pointers in the array)
/// 2. Updating cubie internal orientation (calling ApplyMove on individual cubies)
/// </summary>
public class CubeState
{
    /// <summary> Array of 27 cubies making up the 3x3x3 cube. </summary>
    Cubie[] cubies;

    /// <summary>
    /// Initializes a solved 3x3 Rubik's Cube state.
    /// Cubies that are inside or on faces they don't belong to are assigned -1 for those colors.
    /// </summary>
    public CubeState()
    {
        cubies = new Cubie[27];
        for (int i = 0; i < cubies.Length; i++)
        {
            cubies[i] = new Cubie();
        }
        for (int i = 0; i < cubies.Length; i++)
        {
            Vector3Int pos = IdxToPos(i);
            int x = pos.x;
            int y = pos.y;
            int z = pos.z;
            if (x != 1) cubies[i][0] = -1;
            if (y != 1) cubies[i][1] = -1;
            if (z != 1) cubies[i][2] = -1;
            if (x != -1) cubies[i][3] = -1;
            if (y != -1) cubies[i][4] = -1;
            if (z != -1) cubies[i][5] = -1;
        }
    }

    /// <summary>
    /// Creates a deep copy of this CubeState.
    /// Useful for AI search algorithms like IDA* to explore branches without modifying the current state.
    /// </summary>
    /// <returns>A cloned CubeState instance.</returns>
    public CubeState Clone()
    {
        CubeState res = new CubeState();
        for (int i = 0; i < cubies.Length; i++)
        { 
            for (int j = 0; j < 6; j++)
            {
                res.cubies[i][j] = cubies[i][j];
            }
        }
        return res;
    }

    /// <summary>
    /// Indexer to access a cubie at a specific array index.
    /// </summary>
    /// <param name="index">Array index (0..26).</param>
    /// <returns>The Cubie object at that index.</returns>
    public Cubie this[int index]
    {
        get => cubies[index];
        set => cubies[index] = value;
    }

    /// <summary>
    /// Applies a move to the cube state.
    /// 
    /// Steps:
    /// 1. Permute corner cubies
    /// 2. Permute edge cubies
    /// 3. Update orientation of affected cubies
    /// 
    /// This function maintains cube validity.
    /// </summary>
    /// <param name="move">Move to apply</param>
    public void ApplyMove(Move move)
    {
        int face = move.face;
        bool rev = move.rev;
        bool is180 = move.is180;
        int[] corners = GetCorners(face);
        int[] edges = GetEdges(face);
        if (rev)
        {
            Array.Reverse(corners);
            Array.Reverse(edges);
        }
        int delta = 1;
        if (is180) delta++;
        Cubie[] new_cubies = new Cubie[corners.Length];
        for (int i = 0; i < new_cubies.Length; i++)
        {
            new_cubies[i] = cubies[corners[(i - delta + corners.Length) % corners.Length]];
        }
        for (int i = 0; i < new_cubies.Length; ++i)
        {
            cubies[corners[i]] = new_cubies[i];
        }
        for (int i = 0; i < new_cubies.Length; ++i)
        {
            new_cubies[i] = cubies[edges[(i - delta + edges.Length) % edges.Length]];
        }
        for (int i = 0; i < new_cubies.Length; ++i)
        {
            cubies[edges[i]] = new_cubies[i];
        }
        for (int i = 0; i < corners.Length; ++i)
        {
            cubies[corners[i]].ApplyMove(move);
        }
        for (int i = 0; i < edges.Length; ++i)
        {
            cubies[edges[i]].ApplyMove(move);
        }
        cubies[GetCenter(face)].ApplyMove(move);
    }

    /// <summary>
    /// Checks whether the cube is in a solved state.
    /// A face is solved if all its stickers match the center sticker's color.
    /// </summary>
    /// <returns>True if all faces are uniform in color.</returns>
    public bool IsSolved()
    {
        for (int face = 0; face < 6; ++face)
        {
            int center = GetCenter(face);
            int[] corners = GetCorners(face);
            int[] edges = GetEdges(face);
            foreach (int i in corners)
            {
                if (cubies[center][face] != cubies[i][face]) return false;
            }
            foreach (int i in edges)
            {
                if (cubies[center][face] != cubies[i][face]) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Validates the cube state to ensure it is physically possible.
    /// Checks for unique corner and edge combinations and proper color placement.
    /// </summary>
    /// <returns>True if the cube state is valid, otherwise false.</returns>
    public bool IsValid()
    {
        bool[] mark = new bool[cubies.Length];
        foreach (int pos_idx in CORNER_LIST)
        {
            Vector3Int pos = IdxToPos(pos_idx);
            int x_color = cubies[pos_idx][pos.x > 0 ? 0 : 3];
            int y_color = cubies[pos_idx][pos.y > 0 ? 1 : 4];
            int z_color = cubies[pos_idx][pos.z > 0 ? 2 : 5];
            if (x_color == -1 || y_color == -1 || z_color == -1) return false;
            if (x_color % 3 == y_color % 3 || x_color % 3 == z_color % 3 || y_color % 3 == z_color % 3) return false;
            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
            int original_pos_idx = PosToIdx(new Vector3Int(back ? -1 : 1, down ? -1 : 1, left ? -1 : 1));
            if (mark[original_pos_idx]) return false;
            mark[original_pos_idx] = true;
        }

        foreach (int pos_idx in EDGE_LIST)
        {
            Vector3Int pos = IdxToPos(pos_idx);
            int x_color = -1;
            int y_color = -1;
            int z_color = -1;
            if (pos.x != 0) x_color = cubies[pos_idx][pos.x > 0 ? 0 : 3];
            if (pos.y != 0) y_color = cubies[pos_idx][pos.y > 0 ? 1 : 4];
            if (pos.z != 0) z_color = cubies[pos_idx][pos.z > 0 ? 2 : 5];
            int cnt = (x_color != -1 ? 1 : 0) + (y_color != -1 ? 1 : 0) + (z_color != -1 ? 1 : 0);
            if (cnt != 2) return false;
            bool front = (x_color == 0) || (y_color == 0) || (z_color == 0);
            bool up = (x_color == 1) || (y_color == 1) || (z_color == 1);
            bool right = (x_color == 2) || (y_color == 2) || (z_color == 2);
            bool back = (x_color == 3) || (y_color == 3) || (z_color == 3);
            bool down = (x_color == 4) || (y_color == 4) || (z_color == 4);
            bool left = (x_color == 5) || (y_color == 5) || (z_color == 5);
            if ((front && back) || (up && down) || (right && left)) return false;
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
            if (mark[original_pos_idx]) return false;
            mark[original_pos_idx] = true;
        }
        return true;
    }
}