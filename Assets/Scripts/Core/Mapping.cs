using System;
using UnityEngine;

/// <summary>
/// Provides static mapping utilities for Rubik's Cube representation.
/// 
/// This class defines:
/// - Face indices constants (FRONT, UP, RIGHT, BACK, DOWN, LEFT)
/// - Conversion between linear cubie index (0..26) and 3D position
/// - Face color mapping
/// - Precomputed structural relationships (adjacent faces, corners, edges, centers)
/// - Rotation axis for each face
/// 
/// This class contains no state and acts as a global lookup table
/// describing the geometric structure of the cube.
/// </summary>
class Mapping
{
    public const int FRONT = 0;
    public const int UP = 1;
    public const int RIGHT = 2;
    public const int BACK = 3;
    public const int DOWN = 4;
    public const int LEFT = 5;

    /// <summary>
    /// Converts a linear cubie index (0..26) into its 3D position in cube space.
    /// 
    /// Coordinate system:
    /// x, y, z ∈ {-1, 0, 1}
    /// 
    /// The mapping assumes a fixed ordering of cubies and is used
    /// to convert between array-based storage and spatial positioning.
    /// </summary>
    /// <param name="idx">Linear cubie index (0..26)</param>
    /// <returns>3D position as Vector3Int</returns>
    public static Vector3Int IdxToPos(int idx)
    {
        return new Vector3Int(1 - idx / 9, 1 - (idx % 9 / 3), 1 - idx % 3);
    }

    /// <summary>
    /// Converts a 3D cube position (x,y,z) into its linear array index (0..26).
    /// 
    /// This is the inverse function of IdxToPos().
    /// </summary>
    /// <param name="Pos">Position in cube space (x,y,z)</param>
    /// <returns>Linear cubie index</returns>
    public static int PosToIdx(Vector3Int Pos)
    {
        return 9 * (1 - Pos.x) + 3 * (1 - Pos.y) + (1 - Pos.z);
    }

    /// <summary>
    /// Returns the Unity color corresponding to a given face index.
    /// 
    /// This is used for rendering purposes only and does not affect cube logic.
    /// </summary>
    /// <param name="face">Face index constant</param>
    /// <returns>Color associated with that face</returns>
    public static Color GetColor(int face)
    {
        if (face == FRONT) return Color.blue;
        else if (face == UP) return Color.yellow;
        else if (face == RIGHT) return Color.red;
        else if (face == BACK) return Color.green;
        else if (face == DOWN) return Color.white;
        else if (face == LEFT) return Color.magenta;
        else return Color.black;
    }

    /// <summary>
    /// Returns the 4 adjacent face indices surrounding a given face,
    /// ordered in clockwise direction relative to looking directly at that face.
    /// 
    /// Used for rotating cubie face colors when a move is applied.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Array of 4 adjacent face indices (clockwise)</returns>
    public static int[] GetAdjacent(int face)
    {
        if (face == FRONT)
        {
            return new int[] { 1, 2, 4, 5 };
        }
        else if (face == UP)
        {
            return new int[] { 2, 0, 5, 3 };
        }
        else if (face == RIGHT)
        {
            return new int[] { 0, 1, 3, 4 };
        }
        else if (face == BACK)
        {
            return new int[] { 2, 1, 5, 4 };
        }
        else if (face == DOWN)
        {
            return new int[] { 0, 2, 3, 5 };
        }
        else if (face == LEFT)
        {
            return new int[] { 1, 0, 4, 3 };
        }
        else
        {
            throw new Exception("GetAdjacent: invalid value for face");
        }
    }

    /// <summary>
    /// Returns the 4 corner cubie indices of a given face,
    /// ordered in clockwise direction.
    /// 
    /// Used to permute corner cubies during face rotation.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Array of 4 cubie indices (corners)</returns>
    public static int[] GetCorners(int face)
    {
        if (face == FRONT)
        {
            return new int[] { 0, 6, 8, 2 };
        }
        else if (face == UP)
        {
            return new int[] { 0, 2, 20, 18 };
        }
        else if (face == RIGHT)
        {
            return new int[] { 0, 18, 24, 6 };
        }
        else if (face == BACK)
        {
            return new int[] { 18, 20, 26, 24 };
        }
        else if (face == DOWN)
        {
            return new int[] { 6, 24, 26, 8 };
        }
        else if (face == LEFT)
        {
            return new int[] { 20, 2, 8, 26 };
        }
        else
        {
            throw new Exception("GetCorners: invalid value for face");
        }
    }

    /// <summary>
    /// Returns the 4 edge cubie indices of a given face,
    /// ordered in clockwise direction.
    /// 
    /// Used to permute edge cubies during face rotation.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Array of 4 cubie indices (edges)</returns>
    public static int[] GetEdges(int face)
    {
        if (face == FRONT)
        {
            return new int[] { 1, 3, 7, 5 };
        }
        else if (face == UP)
        {
            return new int[] { 1, 11, 19, 9 };
        }
        else if (face == RIGHT)
        {
            return new int[] { 3, 9, 21, 15 };
        }
        else if (face == BACK)
        {
            return new int[] { 19, 23, 25, 21 };
        }
        else if (face == DOWN)
        {
            return new int[] { 7, 15, 25, 17 };
        }
        else if (face == LEFT)
        {
            return new int[] { 11, 5, 17, 23 };
        }
        else
        {
            throw new Exception("GetEdges: Invalid value for face");
        }
    }

    /// <summary>
    /// Returns the center cubie index of a given face.
    /// 
    /// Center cubies never change position but their orientation
    /// still changes during face rotations.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Center cubie index</returns>
    public static int GetCenter(int face)
    {
        if (face == FRONT) return 4;
        else if (face == UP) return 10;
        else if (face == RIGHT) return 12;
        else if (face == BACK) return 22;
        else if (face == DOWN) return 16;
        else if (face == LEFT) return 14;
        else throw new Exception("GetCenter: Invalid value for face");
    }

    /// <summary>
    /// Returns the 3D rotation axis for a given face.
    /// 
    /// This is used for animation in Unity when rotating a layer.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Rotation axis vector</returns>
    public static Vector3 GetAxis(int face)
    {
        if (face == FRONT) return new Vector3(1, 0, 0);
        else if (face == UP) return new Vector3(0, 1, 0);
        else if (face == RIGHT) return new Vector3(0, 0, 1);
        else if (face == BACK) return new Vector3(-1, 0, 0);
        else if (face == DOWN) return new Vector3(0, -1, 0);
        else if (face == LEFT) return new Vector3(0, 0, -1);
        else throw new Exception("GetAxis: Invalid value for face");
    }
}