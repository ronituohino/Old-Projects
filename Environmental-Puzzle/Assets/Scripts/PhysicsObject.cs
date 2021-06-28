using UnityEngine;

public class PhysicsObject : MonoBehaviour, IDraggable, IHoverable
{
    Rigidbody rb;
    bool simulating = false;

    Vector3 localHitPoint;

    float distanceFromCam;
    float targetDistanceFromCam;

    bool resetCollisionDetectionOnSleep = false;

    Vector3 previousTargetPosition = Vector3.zero;
    bool targetMoved = false;
    bool reachedGoal = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        if(simulating)
        {
            HoverVisuals.Instance.lineStartPos = rb.position;
        }
    }

    private void FixedUpdate()
    {
        if(simulating)
        {
            //Scroll wheel distance adjustment
            targetDistanceFromCam = targetDistanceFromCam += 
                                        InputManager.Instance.mouseScroll * 
                                        PhysicsSettings.Instance.distanceScrollMultiplier *
                                        InputManager.Instance.controlledCamera.cam.orthographicSize;

            targetDistanceFromCam = Mathf.Clamp
                                        (
                                            targetDistanceFromCam, 
                                            PhysicsSettings.Instance.minHoverDistance, 
                                            PhysicsSettings.Instance.maxHoverDistance
                                        );

            distanceFromCam = Mathf.Lerp(distanceFromCam, targetDistanceFromCam, PhysicsSettings.Instance.distanceScrollLerp);

            //Position
            Vector3 targetPosition = InputManager.Instance.mouseInWorld + InputManager.Instance.controlledCamera.cam.transform.forward * distanceFromCam;

            targetMoved = targetPosition != previousTargetPosition;
            if(targetMoved)
            {
                reachedGoal = false;
            }

            Vector3 hitPointInWorld = transform.TransformPoint(localHitPoint);
            Vector3 force = Vector3.ClampMagnitude(targetPosition - hitPointInWorld, PhysicsSettings.Instance.maximumForce);

            //Debug.DrawLine(rb.position, hitPointInWorld, Color.green, Time.deltaTime);
            //Debug.DrawRay(hitPointInWorld, force, Color.red, Time.deltaTime);

            rb.AddForceAtPosition
                (
                    force * PhysicsSettings.Instance.hoverMovementMulitplier, 
                    hitPointInWorld, 
                    ForceMode.VelocityChange
                );

            //Free object from drag constraints once it has reached it's goal
            if (reachedGoal || (!targetMoved && rb.velocity.magnitude < PhysicsSettings.Instance.noSmoothingVelocityThreshold))
            {
                reachedGoal = true;
            } 
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            previousTargetPosition = targetPosition;
        } 
        else
        {
            if(resetCollisionDetectionOnSleep && rb.IsSleeping())
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                resetCollisionDetectionOnSleep = false;
            }
        }
    }

    void IDraggable.OnDrag(Vector2 totalMovement)
    {
        
    }

    void IDraggable.OnDragPress()
    {
        localHitPoint = transform.InverseTransformPoint(InputManager.Instance.closestMouseRayHit.point);

        distanceFromCam = (InputManager.Instance.controlledCamera.cam.transform.position - InputManager.Instance.closestMouseRayHit.point).magnitude;
        targetDistanceFromCam = distanceFromCam;

        rb.drag = PhysicsSettings.Instance.drag;
        rb.angularDrag = PhysicsSettings.Instance.angularDrag;
        rb.sleepThreshold = 0f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Transform[] children = transform.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in children)
        {
            t.gameObject.layer = 8;
        }

        InputManager.Instance.interactingWithPhysicsObject = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        HoverVisuals.Instance.Interacting(true);

        simulating = true;
    }

    void IDraggable.OnDragRelease()
    {
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.sleepThreshold = 0.005f;

        rb.interpolation = RigidbodyInterpolation.None;
        resetCollisionDetectionOnSleep = true;

        Transform[] children = transform.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in children)
        {
            t.gameObject.layer = 0;
        }

        InputManager.Instance.interactingWithPhysicsObject = false;

        HoverVisuals.Instance.Interacting(false);

        simulating = false;
    }

    void IHoverable.OnHoverEnter()
    {
        //throw new System.NotImplementedException();
    }

    void IHoverable.OnHoverExit()
    {
        //throw new System.NotImplementedException();
    }

    void IHoverable.OnHoverStay()
    {
        //throw new System.NotImplementedException();
    }
}