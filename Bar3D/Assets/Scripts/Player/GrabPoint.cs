using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Looks for collisions on the ends of the players hands, grabs onto PhysicsObjects
public class GrabPoint : MonoBehaviour
{
    [SerializeField] Rigidbody thisRb;
    [SerializeField] Collider thisCollider;
    public bool carryingObject = false;

    [Space]

    //If hand can attach to physics objects, handled from elsewhere
    public bool enableGrab = false;

    public PhysicsObject obj = null;
    FixedJoint fj = null;

    private void OnCollisionEnter(Collision collision)
    {
        if(enableGrab)
        {
            if (!carryingObject)
            {
                obj = collision.collider.GetComponent<PhysicsObject>();
                if (obj != null)
                {
                    fj = obj.gameObject.AddComponent<FixedJoint>();
                    fj.connectedBody = thisRb;
                    fj.enableCollision = false;
                    fj.autoConfigureConnectedAnchor = true;

                    obj.GrabThis(this);

                    enableGrab = false;
                    carryingObject = true;
                }
            }
        }
    }

    public void DropObject()
    {
        if(carryingObject)
        {
            Destroy(fj);

            obj.DropThis(this);

            enableGrab = false;
            carryingObject = false;
        }
    }
}
