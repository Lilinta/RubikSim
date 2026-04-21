using UnityEngine;
using System.Collections;
using static Mapping;

/// <summary>
/// Handles visual animation of cube layer rotations in Unity.
/// 
/// This class does not modify CubeState. It only performs
/// temporary GameObject transformations for visual effect.
/// </summary>
public class CubeAnimator : MonoBehaviour
{
    public float rotate_speed = 200f;
    public CubeRenderer render;

    /// <summary>
    /// Animates a single face rotation by:
    /// - Grouping affected cubies under a temporary pivot
    /// - Rotating the pivot smoothly
    /// - Restoring cubies to original hierarchy
    /// 
    /// Supports normal, reverse, and 180-degree rotations.
    /// </summary>
    /// <param name="move">Move describing which face and direction to rotate</param>
    /// <returns>Coroutine for animation</returns>
    public IEnumerator AnimateMove(Move move)
    {
        GameObject[] layer = render.GetLayer(move.face);

        GameObject pivot = new GameObject("Pivot");
        foreach (var c in layer)
            c.transform.parent = pivot.transform;

        float rotated = 0f;
        Vector3 axis = GetAxis(move.face);
        float target = 90f;
        if (move.rev) axis = -axis;
        if (move.is180) target = 180f;
        while (rotated < target)
        {
            float step = rotate_speed * Time.deltaTime;
            pivot.transform.Rotate(axis, step);
            rotated += step;
            yield return null;
        }

        foreach (var c in layer)
            c.transform.parent = render.transform;

        Destroy(pivot);
    }
}