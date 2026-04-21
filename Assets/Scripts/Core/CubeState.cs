using System;
using static Mapping;

/// <summary>
/// Represents the full logical state of a 3x3 Rubik's Cube.
/// 
/// The cube consists of 27 cubies stored in a fixed-size array.
/// 
/// State updates occur in two stages:
/// 1. Permuting cubie positions (corners & edges)
/// 2. Updating cubie internal orientation
/// 
/// This class contains no rendering logic.
/// </summary>
public class CubeState
{
    Cubie[] cubies;

    public CubeState()
    {
        cubies = new Cubie[27];
        for (int i = 0; i < cubies.Length; i++)
        {
            cubies[i] = new Cubie();
        }
    }

    /// <summary>
    /// Creates a deep copy of this CubeState.
    /// 
    /// Used for search algorithms (e.g., IDA*) where immutable branching
    /// of states is required.
    /// </summary>
    /// <returns>Cloned CubeState</returns>
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
    /// Checks whether the cube is in solved state.
    /// 
    /// A face is solved if all its corner and edge stickers
    /// match the color of its center.
    /// </summary>
    /// <returns>True if solved, otherwise false</returns>
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
}