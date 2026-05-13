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
    /// <summary> Internal array storing the face color indices. </summary>
    int[] color;

    /// <summary>
    /// Initializes a new Cubie with default face colors corresponding to face indices.
    /// </summary>
    public Cubie()
    {
        color = new int[6];
        for (int i = 0; i < color.Length; i++)
        {
            color[i] = i;
        }
    }

    /// <summary>
    /// Indexer to get or set the color index of a specific face of this cubie.
    /// </summary>
    /// <param name="index">The face index constant (0..5).</param>
    /// <returns>The color index present on that face.</returns>
    public int this[int index]
    {
        get => color[index];
        set => color[index] = value;
    }

    /// <summary>
    /// Applies a face rotation to this cubie by permuting its internal face colors.
    /// 
    /// This does NOT change the cubie's position in the 3D grid.
    /// It updates which color faces which direction after a rotation.
    /// </summary>
    /// <param name="move">The Move object describing the rotation.</param>
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
