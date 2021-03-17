using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBuilder : MonoBehaviour
{
    private static GameObject[] _Chunks;
    private MeshBuilder _Builder;

    public static readonly int CHUNK_SIZE = 33;
    private readonly int HALF_CHUNK_SIZE = 16;
    private readonly int MAX_LOD = 9;

    public static float MAX_SIZE;

    public Material chunkMaterial;

    void Awake()
    {
        _Builder = ScriptableObject.CreateInstance<MeshBuilder>();
        CreateChunks();

        MAX_SIZE = (CHUNK_SIZE - 1) * Mathf.Pow(2, MAX_LOD - 1);
    }

    public static GameObject GetChunkLOD(int lod)
    {
        if (lod > _Chunks.Length || lod < 0)
            return null;

        return _Chunks[lod];
    }

    private void CreateChunks()
    {
        _Chunks = new GameObject[MAX_LOD];

        for (int i = 0; i < _Chunks.Length; i++)
        {
            _Chunks[i] = CreateChunkGameObject(i);
        }
    }

    private GameObject CreateChunkGameObject(int lod)
    {
        GameObject chunk = new GameObject("Chunk" + lod);

        MeshFilter filter = chunk.AddComponent<MeshFilter>();
        MeshRenderer renderer = chunk.AddComponent<MeshRenderer>();
        MeshCollider collider = chunk.AddComponent<MeshCollider>();

        filter.mesh = CreateChunkMesh(lod);
        filter.mesh.RecalculateNormals();
        collider.sharedMesh = filter.mesh;
        collider.enabled = false;

        if (chunkMaterial != null)
            renderer.material = chunkMaterial;

        chunk.hideFlags = HideFlags.HideInHierarchy;
        chunk.SetActive(false);

        return chunk;
    }

    private Mesh CreateChunkMesh(int lod)
    {
        if (_Builder == null)
            return null;

        _Builder.Clear();

        float uvSize = 1f / CHUNK_SIZE;
        float spacing = Mathf.Pow(2, lod);

        int index = 0;

        for (int z = -HALF_CHUNK_SIZE; z < HALF_CHUNK_SIZE + 1; z++)
        {
            for (int x = -HALF_CHUNK_SIZE; x < HALF_CHUNK_SIZE + 1; x++)
            {
                Vector3 vPos = new Vector3(
                    (this.transform.position.x + x) * spacing,
                    this.transform.position.y,
                    (this.transform.position.z + z) * spacing);

                _Builder.AddVertex(vPos, new Vector2(uvSize * x, uvSize * z));

                if (x < HALF_CHUNK_SIZE && z < HALF_CHUNK_SIZE)
                {
                    _Builder.AddTriangle(index + 1, index, index + CHUNK_SIZE);
                    _Builder.AddTriangle(index + 1, index + CHUNK_SIZE, index + CHUNK_SIZE + 1);
                }

                index++;
            }
        }

        return _Builder.CreateMesh("Chunk(" + lod + ")");
    }
}
