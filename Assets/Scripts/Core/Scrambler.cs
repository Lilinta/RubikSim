using System.Collections.Generic;
using System;
/// <summary>
/// Generates random scramble sequences for the Rubik's Cube.
/// 
/// The generated sequence avoids:
/// - Two consecutive moves on the same face (e.g., R R)
/// - Redundant patterns such as ABA where A and B are opposite faces 
///   belonging to the same axis group (e.g., U D U is invalid).
/// 
/// This produces more realistic and effective scrambles 
/// similar to official World Cube Association (WCA) rules.
/// </summary>
class Scrambler
{
    /// <summary> Internal random number generator for scramble generation. </summary>
    private static Random rng = new Random();

    /// <summary>
    /// Generates a scramble sequence of specified length.
    /// 
    /// The sequence satisfies constraints to prevent trivial or redundant moves,
    /// such as moving the same face twice or sandwiching opposite faces.
    /// </summary>
    /// <param name="len">Number of moves in the scramble. If -1, a random length between 20 and 30 is used.</param>
    /// <returns>A List of random Move objects representing the scramble.</returns>
    public static List<Move> GenerateScramble(int len=-1)
    {
        if (len == -1) len = rng.Next(20, 31); 
        List<Move> res = new List<Move>();
        for (int i = 0; i < len; ++i)
        {
            Move move = new Move();
            while (true)
            {
                move = Move.GetRandomMove();

                // Constraint 1: Don't move the same face twice in a row (e.g., R R')
                if (res.Count > 0 && res[res.Count - 1].face == move.face) continue;

                // Constraint 2: Don't move the same face if it's separated by an opposite face on the same axis
                // (e.g., if index % 3 is equal, they are on the same axis: U/D, F/B, R/L)
                // This prevents redundant sequences like U D U.
                if (res.Count > 1 && res[res.Count - 2].face == move.face && res[res.Count - 1].face % 3 == move.face % 3) continue;
                break;
            }
            res.Add(move);
        }
        return res;
    }
}