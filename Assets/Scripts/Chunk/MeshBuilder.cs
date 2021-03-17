using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : ScriptableObject
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    public int AddVertex(Vector3 vert, Vector2 uv = new Vector2())
    {
        vertices.Add(vert);
        uvs.Add(uv);
        return vertices.Count - 1;
    }

    public void AddFace(Vector3[] faceVertices, Vector2[] faceUvs, int[] faceTriangles)
    {
        for (int i = 0; i < faceVertices.Length; i++)
            vertices.Add(faceVertices[i]);

        for (int j = 0; j < faceUvs.Length; j++)
            uvs.Add(faceUvs[j]);

        for (int t = 0; t < faceTriangles.Length; t++)
            triangles.Add(faceTriangles[t]);
    }

    public void AddTriangle(int va, int vb, int vc)
    {
        triangles.Add(va);
        triangles.Add(vb);
        triangles.Add(vc);
    }

    public void AddUV(Vector2 uv)
    {
        uvs.Add(uv);
    }

    private Vector3[] getVertices()
    {
        return vertices.ToArray();
    }

    private int[] getTriangles()
    {
        return triangles.ToArray();
    }

    public Mesh CreateMesh(string name)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = getVertices();
        mesh.uv = uvs.ToArray();
        mesh.triangles = getTriangles();
        mesh.name = name;

        return mesh;
    }
}
