using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public GameObject skeletonParent;
    [HideInInspector]
    public Rigidbody[] rigidbodies;
    [HideInInspector]
    public Collider[] colliders;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        skeletonParent = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        anim.enabled = true;

        rigidbodies = skeletonParent.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
            r.useGravity = false;
        }

        colliders = skeletonParent.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            CollisionDetection cd = c.gameObject.AddComponent<CollisionDetection>();
            cd.ch = this;
            c.isTrigger = true;
        }
    }

    public void FallDown()
    {
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            r.useGravity = true;
        }
        foreach (Collider c in colliders)
        {
            c.isTrigger = false;
        }

        anim.enabled = false;

        PlayerControls pc = GetComponent<PlayerControls>();
        if(pc != null)
        {
            pc.movementLock = true;
            pc.FocusCameraOnBody();
        }
    }

    public void MakeRigid(Rigidbody rb, Collider c)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        c.isTrigger = false;
    }

    public void MakeNonRigid(Rigidbody rb, Collider c)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        c.isTrigger = true;
    }
}
