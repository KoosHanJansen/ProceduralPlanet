using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private Vector3 _LocalPosition;
    private Transform _Parent;
    private QuadTree[] _Branches;
    
    private ComputeBuffer _VerticeBuffer;

    private PlanetData _PlanetData;

    private int _Lod;
    private float _Size;

    public GameObject _Chunk;

    private float[] LOD_DIST =
    {
        64,                 //0
        128,                //1
        256,                //2
        512,                //3
        768,                //4
        1024,               //5
        2048,               //6
        4196,               //7
        8192                //8
    };

    public QuadTree(Vector3 localPosition, Transform parent, PlanetData planetData, float startSize, int lod = 8)
    {
        this._LocalPosition = localPosition;
        this._Parent = parent;
        this._Lod = lod;
        this._Size = startSize;
        this._PlanetData = planetData;

        CreateChunkInstance();
        UpdateTree();
    }

    /*
    public void UpdateTarget(Transform target)
    {
        this._PlanetData.target = target;

        if (_Branches != null)
        {
            for (int i = 0; i < _Branches.Length; i++)
            {
                _Branches[i].UpdateTarget(target);
            }
        }
    }*/

    public void UpdateTree()
    {
        if (_Lod > 0)
        {
            if (DistanceToTarget() < LOD_DIST[this._Lod])
                Subdivide();
            else
                UnloadBranches();
        }
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(_Chunk.GetComponent<MeshRenderer>().bounds.center, _PlanetData.target.transform.position);//Check distance to target object
        //return Vector3.Distance(_Chunk.GetComponent<MeshRenderer>().bounds.center, UnityEditor.SceneView.lastActiveSceneView.camera.transform.position);//Check distance to scene camera (testing)
    }

    public void Subdivide()
    {
        if (_Chunk != null)
            _Chunk.SetActive(false);

        if (_Branches != null)
        {
            for (int i = 0; i < _Branches.Length; i++)
            {
                _Branches[i].UpdateTree();
            }

            return;
        }

        //Initialize new branches
        float halfSize = this._Size * 0.5f;
        _Branches = new QuadTree[4];
        
        //Top Left ^ <-
        _Branches[0] = new QuadTree(
            new Vector3(_LocalPosition.x - halfSize, _LocalPosition.y, _LocalPosition.z + halfSize),
            this._Parent,
            _PlanetData,
            halfSize,
            this._Lod - 1);

        //Top Right ^ ->
        _Branches[1] = new QuadTree(
            new Vector3(_LocalPosition.x + halfSize, _LocalPosition.y, _LocalPosition.z + halfSize),
            this._Parent,
            _PlanetData,
            halfSize,
            this._Lod - 1);

        //Bottom Right v ->
        _Branches[2] = new QuadTree(
            new Vector3(_LocalPosition.x + halfSize, _LocalPosition.y, _LocalPosition.z - halfSize),
            this._Parent,
            _PlanetData,
            halfSize,
            this._Lod - 1);

        //Bottom Left v <-
        _Branches[3] = new QuadTree(
            new Vector3(_LocalPosition.x - halfSize, _LocalPosition.y, _LocalPosition.z - halfSize),
            this._Parent,
            _PlanetData,
            halfSize,
            this._Lod - 1);
    }

    private void CreateChunkInstance()
    {
        _Chunk = GameObject.Instantiate(ChunkBuilder.GetChunkLOD(this._Lod));
        _Chunk.SetActive(true);
        _Chunk.transform.parent = this._Parent;
        _Chunk.isStatic = true;

        _Chunk.transform.localPosition = _LocalPosition;
        _Chunk.transform.localRotation = Quaternion.identity;

        MeshFilter filter = _Chunk.GetComponent<MeshFilter>();
        Mesh newMesh = UpdateMesh(filter.mesh);
        filter.mesh = newMesh;

        if (_Lod <= 3)
        {
            MeshCollider col = _Chunk.GetComponent<MeshCollider>();
            col.enabled = true;
            col.sharedMesh = newMesh;
        }
    }

    private Mesh UpdateMesh(Mesh mesh)
    {
        if (mesh == null)
            return null;

        Mesh newMesh = mesh;
        Vector3[] verts = mesh.vertices;

        int kernel = _PlanetData.computeMesh.FindKernel("ModifyChunk");

        _VerticeBuffer = new ComputeBuffer(verts.Length, sizeof(float) * 3);
        _VerticeBuffer.SetData(mesh.vertices);

        _PlanetData.computeMesh.SetBuffer(kernel, "Vertices", _VerticeBuffer);

        _PlanetData.computeMesh.SetMatrix("localToWorldMatrix", _Chunk.transform.localToWorldMatrix);
        _PlanetData.computeMesh.SetMatrix("worldToLocalMatrix", _Chunk.transform.worldToLocalMatrix);

        _PlanetData.computeMesh.SetVector("origin", _PlanetData.origin);

        _PlanetData.computeMesh.SetFloat("planetSize", _PlanetData.planetSize);
        _PlanetData.computeMesh.SetFloat("scale", _PlanetData.scale);
        _PlanetData.computeMesh.SetFloat("lacunarity", _PlanetData.lacunarity);
        _PlanetData.computeMesh.SetFloat("persistance", _PlanetData.persistance);
        _PlanetData.computeMesh.SetFloat("maxTerrainHeight", _PlanetData.maxHeight);

        _PlanetData.computeMesh.SetInt("octaves", _PlanetData.octaves);
        _PlanetData.computeMesh.SetInt("seed", _PlanetData.seed);

        _PlanetData.computeMesh.Dispatch(kernel, ChunkBuilder.CHUNK_SIZE, ChunkBuilder.CHUNK_SIZE, 1);

        _VerticeBuffer.GetData(verts);

        newMesh.vertices = verts;

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        _VerticeBuffer.Dispose();

        return newMesh;
    }

    public void UnloadBranches()
    {
        if (_Branches != null)
        {
            for (int i = 0; i < _Branches.Length; i++)
            {
                _Branches[i].UnloadBranches();
                GameObject.Destroy(_Branches[i]._Chunk);
            }

            _Branches = null;
        }

        if (_Chunk != null)
            _Chunk.SetActive(true);
    }
}
