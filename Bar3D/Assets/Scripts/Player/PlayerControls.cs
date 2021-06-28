using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves between the character and other scripts, the abstract player object
public class PlayerControls : MirrorHierarchyNetworkBehaviour
{
    [Header("Player Settings")]
    public string playerName;

    [Header("Physics Settings")]
    public Transform tr;
    public Rigidbody rb;

    public float movementSpeed;
    public float rotationSpeed;
    public float movementAngleMultiplier;

    Vector3 vel;
    Quaternion quat;

    float previousAngle = 0f;
    int fullRotations = 0;
    Quaternion restRotation = Quaternion.Euler(0, 0, 0);

    [System.Serializable]
    public class HandRig
    {
        public ArmIK arm;

        internal Vector3 targetPos; //Where the hands should be at
        internal Vector3 vel;

        public Transform handRestTransform;
        public Transform handCarryTransform;

        [Header("Player interaction")]

        [HideInInspector]
        public PhysicsObject obj; //PhysicsObject that we are hovering/grabbing

        internal bool hovering;
        internal float hoverHeight;

        public GrabPoint grab;
        internal bool pouring;
        internal GlassPhysics previousClosestGlass = null;
    }

    [Header("Hand movement")]

    public HandRig leftHand;
    public HandRig rightHand;

    [SerializeField] float handDamp;

    [Header("Head movement")]

    [SerializeField] LookAtIK lookAtIK;
    [SerializeField] float headLookHeight;
    [SerializeField] float headLookDistance;

    [Header("Hover settings")]

    [SerializeField] float targetMaxDistance;
    List<PhysicsObject> currentObjectsInRange = new List<PhysicsObject>();
    List<PhysicsObject> previousObjectsInRange = new List<PhysicsObject>();

    [Space]

    [SerializeField] float hoverMovementSpeed;
    [SerializeField] float hoverRotationSpeed;

    [Space]

    [SerializeField] float hoverHeightAdd;
    [SerializeField] float hoverHeightChangeMultiplier;

    [Header("Pour settings")]

    [SerializeField] float maxPourDistance;


    private void Start()
    {
        InputManager.Instance.MouseScroll += ChangeHover;
    }

    private void Update()
    {
        // TARGETS
        RaycastHit[] objects = Physics.SphereCastAll(rb.position, targetMaxDistance, Vector3.up, 1048576); //Layer 15 == 32 768 == PhysicsObject
        currentObjectsInRange.Clear();

        // See which targets are now in range
        foreach (RaycastHit rh in objects)
        {
            PhysicsObject obj = rh.collider.gameObject.GetComponent<PhysicsObject>();
            if (obj != null)
            {
                if (obj is GlassPhysics)
                {
                    obj = (GlassPhysics)obj;
                }

                if (previousObjectsInRange.Contains(obj))
                {
                    currentObjectsInRange.Add(obj);
                    obj.DuringRange();
                }
                else
                {
                    currentObjectsInRange.Add(obj);
                    obj.EnteredRange();
                }
            }
        }

        // Call exit on those that are no longer in range
        foreach (PhysicsObject obj in previousObjectsInRange)
        {
            if (!currentObjectsInRange.Contains(obj))
            {
                obj.ExitedRange();
            }
        }

        previousObjectsInRange.Clear();
        previousObjectsInRange.AddRange(currentObjectsInRange);

        // BODY
        tr.position += vel * Time.deltaTime;
        tr.rotation = Quaternion.Slerp(tr.rotation, quat, rotationSpeed);

        // HANDS
        MoveHand(leftHand);
        MoveHand(rightHand);

        // HEAD
        SetHeadTarget();
    }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.MouseScroll -= ChangeHover;
        }
    }

    // Transforms
    void MoveHand(HandRig hand)
    {
        // Hands
        if (hand.grab.carryingObject)
        {
            hand.targetPos = hand.handCarryTransform.position;
        }
        else if (hand.obj != null)
        {
            hand.targetPos = hand.obj.transform.position;
        }
        else
        {
            hand.targetPos = hand.handRestTransform.position;
        }

        hand.arm.solver.arm.target.position = Vector3.SmoothDamp(hand.arm.solver.arm.target.position, hand.targetPos, ref hand.vel, handDamp);
    }

    void SetHeadTarget()
    {
        if (rb.velocity.magnitude > 0.3f)
        {
            lookAtIK.solver.target.position = rb.transform.position
                                      + new Vector3(rb.velocity.x * headLookDistance, rb.position.y + headLookHeight, rb.velocity.z * headLookDistance);
        }
    }

    private void FixedUpdate()
    {
        // Move hovered object(s)
        HoverObject(leftHand);
        HoverObject(rightHand);
    }

    void HoverObject(HandRig hand)
    {
        if (hand.hovering)
        {
            hand.obj.rb.velocity = Vector3.zero;
            hand.obj.rb.angularVelocity = Vector3.zero;

            Ray mouseRay = InputManager.Instance.sceneCamera.ScreenPointToRay(InputManager.Instance.mousePositionInScreen);
            float lengthsToReachHoverPlane = (mouseRay.origin.y - hand.hoverHeight) / -mouseRay.direction.y;
            Vector3 hoverPoint = mouseRay.direction * lengthsToReachHoverPlane + mouseRay.origin;

            Quaternion current = hand.obj.rb.rotation;
            Quaternion target = Quaternion.Euler(-90, 0, 0);

            GlassPhysics closestGlass = GetClosestGlass(hand.obj.rb.worldCenterOfMass);

            // Calls to glasses
            if (closestGlass != null)
            {
                // The closest glass has changed
                if (hand.previousClosestGlass != closestGlass)
                {
                    // We actually found a new glass (switched from old one) to pour to
                    if (hand.previousClosestGlass != null)
                    {
                        hand.previousClosestGlass.UnsetAsPourTarget();
                    }

                    closestGlass.SetAsPourTarget();
                }
            }

            // Add rotation towards glass if pouring
            if (hand.pouring)
            {
                if (closestGlass != null)
                {
                    BottlePhysics bp = (BottlePhysics)hand.obj;
                    bp.Pour(closestGlass);

                    // Bottle rotation
                    Vector3 toGlass = hoverPoint - closestGlass.rb.worldCenterOfMass;
                    Vector3 sideVec = Vector3.Cross(toGlass, Vector3.up);

                    float targetBottleAngle = GlobalReferencesAndSettings.Instance.pourCurve.Evaluate(1 - bp.fullness) * 180f;
                    target *= Quaternion.AngleAxis
                                            (
                                                targetBottleAngle,
                                                Quaternion.Euler(90, 0, 0) * sideVec
                                            );


                    // Move bottle to optimal position on top of glass, takes into account the bottle pivot and centerOfMass difference
                    Vector3 bottleToTopPoint = (target * bp.bottle.bottleFluidPoint) + bp.transform.position - bp.rb.worldCenterOfMass;

                    // This rescales the vector so we don't need to deal with angles
                    Vector3 flatDistanceVectorFromGlass = new Vector3(bottleToTopPoint.x, 0, bottleToTopPoint.z);

                    Vector3 posAboveGlass = new Vector3(closestGlass.rb.worldCenterOfMass.x, hoverPoint.y, closestGlass.rb.worldCenterOfMass.z);
                    Vector3 finalBottlePosition = posAboveGlass - flatDistanceVectorFromGlass;

                    hoverPoint = finalBottlePosition;
                }
            }

            hand.previousClosestGlass = closestGlass;

            // Home to hoverPoint
            hand.obj.rb.velocity = (hoverPoint - hand.obj.rb.worldCenterOfMass) * hoverMovementSpeed;

            Quaternion diff = target * Quaternion.Inverse(current);
            hand.obj.rb.angularVelocity = new Vector3(diff.x, diff.y, diff.z) * hoverRotationSpeed;
        }
    }

    // Has a distance filter, if the closest glass is further than maxPourDistance, return null
    GlassPhysics GetClosestGlass(Vector3 origin)
    {
        float distance = maxPourDistance;
        GlassPhysics fObj = null;

        int count = currentObjectsInRange.Count;
        for (int i = 0; i < count; i++)
        {
            PhysicsObject obj = currentObjectsInRange[i];

            if (!(obj is GlassPhysics))
            {
                continue;
            }

            float d = (obj.transform.position - origin).magnitude;
            if (d < distance)
            {
                distance = d;
                fObj = (GlassPhysics)obj;
            }
        }

        return fObj;
    }


    #region INPUT

    public void PlayerMovement(Vector2 movement)
    {
        vel = new Vector3(movement.x, 0f, movement.y) * movementSpeed;
        float finalAngle = 0f;

        if (movement == Vector2.zero)
        {
            finalAngle = previousAngle;
            quat = restRotation;
        }
        else
        {
            float movementAngle = Mathf.Atan(movement.y / movement.x) * Mathf.Rad2Deg;

            bool onRightSide = movement.x >= 0f;
            bool onTopSide = movement.y >= 0f;

            //Turn range [-180:180]
            finalAngle = onRightSide ? movementAngle * -1 :
                                                    onTopSide ? -180f - movementAngle : 180f - movementAngle;
            //Turn range [0:360]
            finalAngle = finalAngle < 0f ? 360f + finalAngle : finalAngle;
        }

        //Handle turning over 360 degrees
        if (finalAngle < 180f && previousAngle >= 270f)
        {
            float counterClockwiseTurn = previousAngle - finalAngle;
            if (counterClockwiseTurn > 180f)
            {
                //We need to rotate over the 0/360 rotation
                fullRotations += 1;
            }
        }
        else if (finalAngle > 180f && previousAngle < 90f)
        {
            float counterClockwise = finalAngle - previousAngle;
            if (counterClockwise > 180f)
            {
                //We need to rotate over the 0/360 rotation
                fullRotations -= 1;
            }
        }

        Quaternion walkQuaternion = Quaternion.FromToRotation //Forwards tilt
                                                (
                                                    Vector3.up,
                                                    new Vector3(vel.x * movementAngleMultiplier, 1f, vel.z * movementAngleMultiplier).normalized
                                                )
                                    * Quaternion.Euler //Rotation to velocity
                                                (
                                                    0f,
                                                    finalAngle + fullRotations * 360f,
                                                    0f
                                                );
        quat = walkQuaternion;
        previousAngle = finalAngle;
    }

    public void ObjectPress(HandRig hand, PhysicsObject obj) // 0 == left, 1 == right
    {
        // Hover towards hand
        if (
                hand.obj == null
                && !hand.grab.carryingObject
                && leftHand.obj != obj
           )
        {
            MakeObjectHover(hand, obj);
        }
    }

    public void ObjectRelease(HandRig hand, PhysicsObject obj)
    {
        if (hand.hovering)
        {
            InterruptHover(hand, obj);
        }
    }

    public void Drop(HandRig hand)
    {
        if (hand.grab.carryingObject)
        {
            hand.grab.DropObject();
            hand.obj = null;
        }
    }

    public void Pour(bool pouring)
    {
        if (pouring)
        {
            if (leftHand.hovering && leftHand.obj is BottlePhysics)
            {
                leftHand.pouring = true;
            }
            if (rightHand.hovering && rightHand.obj is BottlePhysics)
            {
                rightHand.pouring = true;
            }
        }
        else
        {
            leftHand.pouring = false;
            rightHand.pouring = false;
        }
    }

    void ChangeHover(float val)
    {
        if (leftHand.hovering)
        {
            leftHand.hoverHeight += val * hoverHeightChangeMultiplier;
        }
        if (rightHand.hovering)
        {
            rightHand.hoverHeight += val * hoverHeightChangeMultiplier;
        }
    }

    #endregion

    #region ACTIONS
    void MakeObjectHover(HandRig hand, PhysicsObject obj)
    {
        hand.obj = obj;
        obj.rb.useGravity = false;
        hand.grab.enableGrab = true;

        hand.hoverHeight = obj.transform.position.y + hoverHeightAdd;
        hand.hovering = true;
    }

    void InterruptHover(HandRig hand, PhysicsObject obj)
    {
        hand.obj.rb.useGravity = true;
        hand.obj = null;
        hand.grab.enableGrab = false;

        hand.hovering = false;
        hand.pouring = false;
    }

    #endregion
}
