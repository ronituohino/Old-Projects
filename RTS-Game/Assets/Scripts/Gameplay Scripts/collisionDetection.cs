using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Terrain>() == null)
        {
            buildHandler.isColliding = true;
        } else
        {
            buildHandler.isColliding = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Terrain>() == null)
        {
            buildHandler.isColliding = true;
        } else
        {
            buildHandler.isColliding = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        buildHandler.isColliding = false;
    }
}
