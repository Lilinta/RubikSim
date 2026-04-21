using System;
using static Mapping;

/// <summary>
/// Represents a single cubie (small cube) of the Rubik's Cube.
/// 
/// Each cubie stores the color index of its 6 faces.
/// The cubie itself does not know its spatial position;
/// that is handled by CubeState.
/// 
/// Rotation affects only the ordering of its face colors.
/// </summary>
public class Cubie
{
    int[] color;

    public Cubie()
    {
        color = new int[6];
        for (int i = 0; i < color.Length; i++)
        {
            color[i] = i;
        }
    }

    /// <summary>
    /// Gets or sets the color index of a specific face of this cubie.
    /// </summary>
    /// <param name="index">Face index</param>
    public int this[int index]
    {
        get => color[index];
        set => color[index] = value;
    }

    /// <summary>
    /// Applies a face rotation to this cubie by rotating its face colors.
    /// 
    /// This does NOT change cubie position in the cube.
    /// It only updates internal orientation (color permutation).
    /// 
    /// The rotation direction and type (normal, reverse, 180°)
    /// are determined by the Move object.
    /// </summary>
    /// <param name="move">Move to apply</param>
    public void ApplyMove(Move move)
    {
        int face = move.face;
        bool rev = move.rev;
        bool is180 = move.is180;
        int[] adj = GetAdjacent(face);
        if (rev) Array.Reverse(adj);
        int delta = 1;
        if (is180) delta++;
        int[] new_color = new int[adj.Length];
        for (int i = 0; i < adj.Length; i++)
        {
            new_color[i] = color[adj[(i - delta + adj.Length) % adj.Length]];
        }
        for (int i = 0; i < adj.Length; i++)
        {
            color[adj[i]] = new_color[i]; 
        }
    }
}
