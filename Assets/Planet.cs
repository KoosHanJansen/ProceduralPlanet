using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private SphereTree _SphereTree;
    public Transform target;

    public PlanetData planetData;

    // Start is called before the first frame update
    void Start()
    {
        if (planetData != null)
        {
            planetData.origin = this.transform.position;
            planetData.target = this.target;
        }
            

        _SphereTree = new SphereTree(this.transform, this.planetData);
    }

    // Update is called once per frame
    void Update()
    {
        _SphereTree.Update();
    }
}
