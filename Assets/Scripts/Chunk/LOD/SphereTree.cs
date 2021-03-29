using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTree
{
    private Vector3 _Position;
    private Transform _Planet;
    private QuadTree[] _Trees;
    private Transform[] _Parents;
    private PlanetData _PlanetData;

    public SphereTree(Transform planet, PlanetData planetData)
    {
        this._Position = planetData.origin;
        this._Planet = planet;
        this._PlanetData = planetData;

        CreateParents();
        Initialize();
    }

    private void CreateParents()
    {
        this._Parents = new Transform[6];
        
        float radius = this._PlanetData.planetSize;

        //Top
        GameObject go = new GameObject("Top");
        ParentSettings(go, 0, new Vector3(_Position.x, _Position.y + radius, _Position.z), new Vector3(0, 0, 0));
        //Left
        go = new GameObject("Left");
        ParentSettings(go, 1, new Vector3(_Position.x - radius, _Position.y, _Position.z), new Vector3(0, 0, 90));
        //Right
        go = new GameObject("Right");
        ParentSettings(go, 2, new Vector3(_Position.x + radius, _Position.y, _Position.z), new Vector3(0, 0, 270));
        //Front
        go = new GameObject("Front");
        ParentSettings(go, 3, new Vector3(_Position.x, _Position.y, _Position.z - radius), new Vector3(270, 0, 0));
        //Back
        go = new GameObject("Back");
        ParentSettings(go, 4, new Vector3(_Position.x, _Position.y, _Position.z + radius), new Vector3(90, 0, 0));
        //Bottom
        go = new GameObject("Bottom");
        ParentSettings(go, 5, new Vector3(_Position.x, _Position.y - radius, _Position.z), new Vector3(0, 0, 180));

    }

    private void ParentSettings(GameObject go, int index, Vector3 position, Vector3 rotation)
    {
        go.transform.parent = this._Planet;
        go.transform.position = position;
        go.transform.eulerAngles = rotation;

        _Parents[index] = go.transform;
    }

    private void Initialize()
    {
        _Trees = new QuadTree[6];

        for (int i = 0; i < _Trees.Length; i++)
        {
            _Trees[i] = new QuadTree(Vector3.zero, this._Parents[i], this._PlanetData, this._PlanetData.planetSize);
        }
    }

    public void Update()
    {
        for (int i = 0; i < _Trees.Length; i++)
        {
            //_Trees[i].UpdateTarget(this._PlanetData.target);
            _Trees[i].UpdateTree();
        }
    }
}
