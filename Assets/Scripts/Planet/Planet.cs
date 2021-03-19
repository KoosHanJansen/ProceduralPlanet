using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private SphereTree _SphereTree;
    public Transform target;

    public PlanetData planetData;
    
    void Start()
    {
        if (planetData != null)
        {
            planetData.origin = this.transform.position;
            planetData.target = this.target;
        }
            
        _SphereTree = new SphereTree(this.transform, this.planetData);
    }
    
    void LateUpdate()
    {
        _SphereTree.Update();
        AttractRigidbodies();
    }

    void AttractRigidbodies()
    {
        Rigidbody[] Rigidbodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Vector3 gravityUp = (Rigidbodies[i].transform.position - transform.position).normalized;
            Vector3 bodyUp = Rigidbodies[i].transform.up;

            Rigidbodies[i].AddForce(gravityUp * planetData.gravity);

            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * Rigidbodies[i].transform.rotation;
            Rigidbodies[i].transform.rotation = Quaternion.Slerp(Rigidbodies[i].transform.rotation, targetRotation, 1);
        }
    }
}
