using System;

/// <summary>
/// Represents a single face rotation move on the Rubik's Cube.
/// 
/// A move consists of:
/// - face: which face to rotate (0..5)
/// - rev: whether rotation is counter-clockwise (90 degrees CCW)
/// - is180: whether rotation is a double turn (180 degrees)
/// 
/// Moves are intended to be immutable after creation.
/// </summary>
public class Move
{
    /// <summary> The index of the face being rotated (FRONT, UP, RIGHT, etc.). </summary>
    public int face;
    /// <summary> True if the rotation is counter-clockwise (Reverse). </summary>
    public bool rev;
    /// <summary> True if the rotation is 180 degrees. </summary>
    public bool is180;

    /// <summary> Shared random number generator for generating random moves. </summary>
    private static Random rng = new Random();

    /// <summary>
    /// Initializes a default move (Face -1, no rotation).
    /// </summary>
    public Move()
    {
        face = -1;
        rev = false;
        is180 = false;
    }

    /// <summary>
    /// Initializes a move with a specific face and default 90-degree clockwise rotation.
    /// </summary>
    /// <param name="_face">Face index.</param>
    public Move(int _face)
    {
        face = _face;
        rev = false;
        is180 = false;
    }

    /// <summary>
    /// Initializes a move with a specific face and direction (90-degree rotation).
    /// </summary>
    /// <param name="_face">Face index.</param>
    /// <param name="_rev">True for counter-clockwise, false for clockwise.</param>
    public Move(int _face, bool _rev)
    {
        face = _face;
        rev = _rev;
        is180 = false;
    }

    /// <summary>
    /// Initializes a move with specific face, direction, and rotation type.
    /// </summary>
    /// <param name="_face">Face index.</param>
    /// <param name="_rev">True for counter-clockwise.</param>
    /// <param name="_is180">True for 180-degree rotation.</param>
    public Move(int _face, bool _rev, bool _is180)
    {
        face = _face;
        rev = _rev;
        is180 = _is180;
    }

    /// <summary>
    /// Generates a random valid move.
    /// 
    /// Each move has:
    /// - Random face (0..5)
    /// - Random type (normal, reverse, 180°)
    /// 
    /// Useful for scrambling.
    /// </summary>
    /// <returns>A new random Move object.</returns>
    public static Move GetRandomMove()
    {
        int _face = rng.Next(6);
        int type = rng.Next(3);
        if (type == 0) return new Move(_face, false, false);
        else if (type == 1) return new Move(_face, false, true);
        else return new Move(_face, true, false);
    }

    /// <summary>
    /// Returns a new move that is the logical inverse of the current move.
    /// </summary>
    /// <returns>The inverted Move object.</returns>
    public Move GetInvertedMove()
    {
        return new Move(face, !rev, is180);
    }
}
