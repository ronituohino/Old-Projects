using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Character ch;
    Rigidbody rb;
    Collider c;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        c = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Ground" && other.gameObject.GetComponentInParent<Character>() != ch)
        {
            if(name == "Hips" || name == "Head")
            {
                ch.FallDown();
            }
            else
            {
                ch.MakeRigid(rb, c);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //ch.MakeNonRigid(rb, c);
    }
}
