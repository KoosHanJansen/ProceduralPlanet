using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetData : ScriptableObject
{
    public ComputeShader computeMesh;
    
    public float planetSize = 4096;
    public float scale = 500;
    public float lacunarity = 2;
    public float persistance = 0.5f;
    public float maxHeight = 150;

    public int octaves = 10;
    public int seed = 0;

    public readonly int MAX_LOD = 8;

    public Vector3 origin;
    public Transform target;
}
