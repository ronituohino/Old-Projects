using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAdjuster : MonoBehaviour
{
    //Vector3 initialPoint;
    public bool removeScript = false;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (removeScript)
        {
            if(rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.interpolation = RigidbodyInterpolation.None;
                Destroy(this);
            }
        }
    }
}
