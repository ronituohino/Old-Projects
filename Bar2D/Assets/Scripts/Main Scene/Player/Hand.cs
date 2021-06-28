using Mirror;
using System.Collections;
using UnityEngine;
using System.Linq;

public class Hand : MonoBehaviour
{
    [SerializeField] Character character;

    bool playerHands = false;

    [SerializeField] Transform restTransform;

    [SerializeField] float handDamp;

    public bool searchingForGrabbableObject = true;
    Vector2 vel = Vector2.zero;

    RaycastHit2D closest = new RaycastHit2D();
    Transform previousClosestTransform = null;
    PhysicsObject previousPhysicsObject;

    [HideInInspector]
    public float distanceToHoveredObject = float.MaxValue;
    [HideInInspector]
    public bool grabbingHoveredObject = false;

    public PhysicsObject carriedObject = null;

    // Update is called once per frame
    void Update()
    {
        distanceToHoveredObject = float.MaxValue;

        if (searchingForGrabbableObject && carriedObject == null)
        {
            RaycastHit2D[] hoveredObjects = Physics2D.CircleCastAll
                                                    (
                                                        restTransform.position, 
                                                        GlobalReferencesAndSettings.Instance.handGrabDistance, 
                                                        Vector2.zero, 
                                                        0f, 
                                                        1024
                                                    );

            foreach(RaycastHit2D hit in hoveredObjects)
            {
                print("!");
                float dist = Vector2.Distance(hit.collider.transform.position, restTransform.position);
                if(dist < distanceToHoveredObject)
                {
                    distanceToHoveredObject = dist;
                    closest = hit;
                }
            }

            grabbingHoveredObject = distanceToHoveredObject < GlobalReferencesAndSettings.Instance.handGrabDistance &&
                                    (character.left == this
                                    ? !character.right.grabbingHoveredObject || character.right.distanceToHoveredObject > distanceToHoveredObject
                                    : !character.left.grabbingHoveredObject || character.left.distanceToHoveredObject > distanceToHoveredObject);
        }
        else
        {
            grabbingHoveredObject = false;
        }

        if (grabbingHoveredObject)
        {
            if (closest.transform != previousClosestTransform)
            {
                previousPhysicsObject.handsInRange.Remove(this);

                previousPhysicsObject = closest.transform.GetComponent<PhysicsObject>();
                if(!previousPhysicsObject.handsInRange.Contains(this))
                {
                    previousPhysicsObject.handsInRange.Add(this);
                }
            }

            transform.position = Vector2.SmoothDamp(transform.position, closest.collider.transform.position, ref vel, handDamp);
        }
        else
        {
            transform.position = Vector2.SmoothDamp(transform.position, restTransform.position, ref vel, handDamp);
        }
    }

    public void StartedHover()
    {
        carriedObject = null;
    }

    public void StoppedHover(PhysicsObject physicsObject)
    {
        if (grabbingHoveredObject)
        {
            carriedObject = physicsObject;
        }
    }
}