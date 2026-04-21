using System.Collections.Generic;

/// <summary>
/// Generates random scramble sequences for the Rubik's Cube.
/// 
/// The generated sequence avoids:
/// - Two consecutive moves on the same face
/// - Redundant patterns such as ABA where A and B are opposite faces
///   belonging to the same axis group (e.g., U-D, F-B, L-R)
/// 
/// This produces more realistic and effective scrambles
/// similar to official Rubik scramble rules.
/// </summary>
class Scrambler
{
    /// <summary>
    /// Generates a scramble sequence of specified length.
    /// 
    /// The sequence satisfies constraints to prevent trivial or redundant moves.
    /// </summary>
    /// <param name="len">Number of moves in the scramble</param>
    /// <returns>List of random valid moves</returns>
    public static List<Move> GenerateScramble(int len)
    {
        List<Move> res = new List<Move>();
        for (int i = 0; i < len; ++i)
        {
            Move move = new Move();
            while (true)
            {
                move = Move.GetRandomMove();
                if (res.Count > 0 && res[res.Count - 1].face == move.face) continue;
                if (res.Count > 1 && res[res.Count - 2].face == move.face && res[res.Count - 1].face % 3 == move.face % 3) continue;
                break;
            }
            res.Add(move);
        }
        return res;
    }
}