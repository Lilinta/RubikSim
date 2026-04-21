using UnityEngine;
using System;
using static Mapping;

/// <summary>
/// Responsible for creating and updating the visual representation
/// of the Rubik's Cube in Unity.
/// 
/// This class reads CubeState and updates GameObjects accordingly.
/// It contains no cube logic.
/// </summary>
public class CubeRenderer : MonoBehaviour
{
    public GameObject cubie_prefab;
    private GameObject[] cubies;

    /// <summary>
    /// Initializes the renderer by creating cubie GameObjects
    /// and rendering the initial cube state.
    /// </summary>
    public void Initialize(CubeState state)
    {
        CreateCubies();
        RenderState(state);
    }

    /// <summary>
    /// Instantiates all 27 cubies in correct spatial positions
    /// and stores them in an index-based array.
    /// </summary>
    void CreateCubies()
    {
        cubies = new GameObject[27];
        for (int x = 1; x >= -1; x--)
        {
            for (int y = 1; y >= -1; y--)
            {
                for (int z = 1; z >= -1; z--)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    GameObject c = Instantiate(cubie_prefab, pos, Quaternion.identity);
                    c.transform.parent = transform;
                    cubies[PosToIdx(pos)] = c;
                    //Debug.Log(PosToIdx(pos));
                }
            }
        }
    }

    /// <summary>
    /// Updates all cubie positions, rotations, scales,
    /// and face colors based on the provided CubeState.
    /// 
    /// This fully synchronizes the visual cube with the logical state.
    /// </summary>
    /// <param name="state">CubeState to render</param>
    public void RenderState(CubeState state)
    {
        for (int i = 0; i < cubies.Length; i++)
        {
            Vector3Int pos = IdxToPos(i);
            cubies[i].transform.position = pos;
            cubies[i].transform.rotation = Quaternion.identity;
            cubies[i].transform.localScale = Vector3.one;
            for (int j = 0; j < 6; j++)
            {
                //Debug.Log(i.ToString() + " " + j.ToString());
                Transform quad = cubies[i].transform.Find("Quad" + j.ToString());
                quad.GetComponent<Renderer>().material.color = GetColor(state[i][j]);
            }
        }
    }

    /// <summary>
    /// Retrieves all 9 cubies belonging to a specific face layer
    /// (4 corners, 4 edges, 1 center).
    /// 
    /// Used by CubeAnimator to determine which cubies to rotate.
    /// </summary>
    /// <param name="face">Face index</param>
    /// <returns>Array of 9 GameObjects in that layer</returns>
    public GameObject[] GetLayer(int face)
    {
        GameObject[] layer = new GameObject[9];
        int[] tmp = GetCorners(face);
        int idx = 0;
        for (int i = 0; i < tmp.Length; i++)
        {
            layer[idx] = cubies[tmp[i]];
            Debug.Log("GetLayer corner: " + tmp[i].ToString());
            idx++;
        }
        tmp = GetEdges(face);
        for (int i = 0; i < tmp.Length; i++)
        {
            layer[idx] = cubies[tmp[i]];
            Debug.Log("GetLayer edges: " + tmp[i].ToString());
            idx++;
        }
        layer[idx] = cubies[GetCenter(face)];
        Debug.Log("GetLayer center: " + GetCenter(face).ToString());
        idx++;
        if (idx != 9) throw new Exception("GetLayer: " + idx.ToString());
        return layer;
    }
}