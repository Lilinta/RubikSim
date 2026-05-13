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
static class Mapping
{
    /// <summary> Index for the Front face (usually Blue). </summary>
    public const int FRONT = 0;
    /// <summary> Index for the Up/Top face (usually Yellow). </summary>
    public const int UP = 1;
    /// <summary> Index for the Right face (usually Red). </summary>
    public const int RIGHT = 2;
    /// <summary> Index for the Back face (usually Green). </summary>
    public const int BACK = 3;
    /// <summary> Index for the Down/Bottom face (usually White). </summary>
    public const int DOWN = 4;
    /// <summary> Index for the Left face (usually Orange/Magenta). </summary>
    public const int LEFT = 5;
    /// <summary> List of indices representing the 8 corner cubies in the internal array. </summary>
    public static int[] CORNER_LIST = new int[] {0, 2, 20, 18, 6, 8, 26, 24};
    /// <summary> List of indices representing the 12 edge cubies in the internal array. </summary>
    public static int[] EDGE_LIST = new int[] { 9, 1, 11, 19, 15, 7, 17, 25, 3, 5, 23, 21};
    /// <summary> Mapping array to convert internal indices to Kociemba solver indexing format. </summary>
    public static int[] TO_KOCIEMBA_IDX;

    /// <summary>
    /// Static constructor to initialize precomputed mapping tables.
    /// </summary>
    static Mapping()
    {
        TO_KOCIEMBA_IDX = new int[27];
        for (int i = 0; i < TO_KOCIEMBA_IDX.Length; i++)
        {
            TO_KOCIEMBA_IDX[i] = 0;
        }
        for (int i = 0; i < CORNER_LIST.Length; i++)
        {
            TO_KOCIEMBA_IDX[CORNER_LIST[i]] = i;
        }
        for (int i = 0; i < EDGE_LIST.Length; i++)
        {
            TO_KOCIEMBA_IDX[EDGE_LIST[i]] = i;
        }
    }

    /// <summary>
    /// Converts a linear cubie index (0..26) into its 3D position in cube space.
    /// 
    /// Coordinate system:
    /// x, y, z ∈ {-1, 0, 1}
    /// </summary>
    /// <param name="idx">Linear cubie index (0..26).</param>
    /// <returns>3D position as Vector3Int.</returns>
    public static Vector3Int IdxToPos(int idx)
    {
        return new Vector3Int(1 - idx / 9, 1 - (idx % 9 / 3), 1 - idx % 3);
    }

    /// <summary>
    /// Converts a 3D cube position (x,y,z) into its linear array index (0..26).
    /// </summary>
    /// <param name="Pos">Position in cube space (x,y,z) where components are in {-1, 0, 1}.</param>
    /// <returns>Linear cubie index.</returns>
    public static int PosToIdx(Vector3Int Pos)
    {
        return 9 * (1 - Pos.x) + 3 * (1 - Pos.y) + (1 - Pos.z);
    }

    /// <summary>
    /// Returns the Unity Color corresponding to a given face index.
    /// </summary>
    /// <param name="face">The face index constant.</param>
    /// <returns>The Unity Color associated with that face.</returns>
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
    /// Maps a Unity Color back to its corresponding face index.
    /// </summary>
    /// <param name="color">The Unity Color to check.</param>
    /// <returns>Face index constant, or -1 if no match found.</returns>
    public static int GetColorIdx(Color color)
    {
        if (color == Color.blue) return FRONT;
        else if (color == Color.yellow) return UP;
        else if (color == Color.red) return RIGHT;
        else if (color == Color.green) return BACK;
        else if (color == Color.white) return DOWN;
        else if (color == Color.magenta) return LEFT;
        else return -1;
    }

    /// <summary>
    /// Returns the 4 adjacent face indices surrounding a given face,
    /// ordered clockwise when looking directly at that face.
    /// </summary>
    /// <param name="face">Target face index.</param>
    /// <returns>Array of 4 adjacent face indices.</returns>
    /// <exception cref="Exception">Thrown if face index is invalid.</exception>
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
    /// Returns the 4 corner cubie indices of a given face, ordered clockwise.
    /// </summary>
    /// <param name="face">Target face index.</param>
    /// <returns>Array of 4 cubie indices (corners).</returns>
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
    /// Returns the 4 edge cubie indices of a given face, ordered clockwise.
    /// </summary>
    /// <param name="face">Target face index.</param>
    /// <returns>Array of 4 cubie indices (edges).</returns>
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
    /// </summary>
    /// <param name="face">Target face index.</param>
    /// <returns>Center cubie index.</returns>
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
    /// Returns the 3D rotation axis for a given face used for Unity animations.
    /// </summary>
    /// <param name="face">Face index.</param>
    /// <returns>Rotation axis as a Vector3Int.</returns>
    public static Vector3Int GetAxis(int face)
    {
        if (face == FRONT) return new Vector3Int(1, 0, 0);
        else if (face == UP) return new Vector3Int(0, 1, 0);
        else if (face == RIGHT) return new Vector3Int(0, 0, 1);
        else if (face == BACK) return new Vector3Int(-1, 0, 0);
        else if (face == DOWN) return new Vector3Int(0, -1, 0);
        else if (face == LEFT) return new Vector3Int(0, 0, -1);
        else throw new Exception("GetAxis: Invalid value for face");
    }

    /// <summary>
    /// Converts a move index (0..17) to a Move object.
    /// Mapping: 6 faces * 3 types (normal, 180, reverse).
    /// </summary>
    /// <param name="idx">Move index.</param>
    /// <returns>A Move object representation.</returns>
    public static Move IdxToMove(int idx)
    {
        if (idx >= 18 || idx < 0) throw new Exception("IdxToMove: Invalid value for idx");;
        int face = idx / 3;
        int type = idx % 3;
        if (type == 0) return new Move(face, false, false);
        else if (type == 1) return new Move(face, false, true);
        else return new Move(face, true, false);
    }

    /// <summary>
    /// Converts a Move object to its corresponding integer index (0..17).
    /// </summary>
    /// <param name="move">Move object to convert.</param>
    /// <returns>Integer index of the move.</returns>
    public static int MoveToIdx(Move move)
    {
        return move.face * 3 + Convert.ToInt32(move.rev) * 2 + Convert.ToInt32(move.is180) * 1;
    }
}