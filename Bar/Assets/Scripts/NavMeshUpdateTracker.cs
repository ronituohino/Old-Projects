using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds objects to be tracked in NavMeshUpdater.cs if we collide with other large objects
public class NavMeshUpdateTracker : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name + " " + collision.gameObject.layer + " " + Controls.Instance.largeObjects);
        Rigidbody r = collision.rigidbody;
        if (r && collision.gameObject.layer == 11) //11 being the large objects layer
        {
            if (!NavMeshUpdater.Instance.bodiesAffected.Contains(r))
            {
                NavMeshUpdater.Instance.bodiesAffected.Add(r);
            }
        }
    }
}
