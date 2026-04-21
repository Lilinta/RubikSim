using System;

/// <summary>
/// Represents a single face rotation move.
/// 
/// A move consists of:
/// - face: which face to rotate
/// - rev: whether rotation is counter-clockwise
/// - is180: whether rotation is 180 degrees
/// 
/// Moves are immutable after creation.
public class Move
{
    public int face;
    public bool rev;
    public bool is180;
    private static Random rng = new Random();
    public Move()
    {
        face = -1;
        rev = false;
        is180 = false;
    }
    public Move(int _face)
    {
        face = _face;
        rev = false;
        is180 = false;
    }
    public Move(int _face, bool _rev)
    {
        face = _face;
        rev = _rev;
        is180 = false;
    }
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
    /// <returns>Random Move</returns>
    public static Move GetRandomMove()
    {
        int _face = rng.Next(6);
        int type = rng.Next(3);
        if (type == 0) return new Move(_face, false, false);
        else if (type == 1) return new Move(_face, false, true);
        else return new Move(_face, true, false);
    }
}
