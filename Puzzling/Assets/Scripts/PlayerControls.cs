using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public GameObject player;
    public CharacterController controller;
    public GameObject cam;
    public UIScripts ui;
    public GameObject physicsObjects;

    [Header("Basic settings")]
    public float movementSpeed;
    public float mouseSpeed;

    public bool invertControls;


    [Header("Camera rotation limits")]
    public float verticalRotationMax;
    public float verticalRotationMin;

    float horizontal;
    float vertical;



    [Header("Physics settings")]
    [Tooltip("World gravity")]
    public float gravity;
    private float curGravity;

    [Space]

    public float interactionDistanceMax = 3.2f;
    public float interactionDistanceMin = 0.1f;

    [Space]

    [Tooltip("Material applied to interacted objects")]
    public PhysicMaterial physicMaterial;

    [Space]

    [Tooltip("Force applied to items hovered")]
    public float pickUpSpeed = 550f; //Hovering items stuff
    public float movingMultiplier = 1.2f;

    [Tooltip("Force applied to objects grabbed")]
    public float grabbingForce = 10f;

    [Space]

    [Tooltip("Force applied to items thrown")]
    public float throwForce = 300f;
    public float throwAngularForce;

    [Space]

    [Tooltip("Force applied to items that are rotated as the camera moves")]
    public float lookRotationSpeed = 12f;

    [Tooltip("Force applied to items that are rotated with 'e'")]
    public float manualRotationSpeed = 1f;

    [Space]

    [Tooltip("Force divider when colliding with something")]
    public float clippingFrictionMultiplier = 1.5f;

    [Space]

    [Tooltip("Force applied to objects the player is colliding with")]
    public float pushPower = 55f; //Playerbody collision stuff
    public float weight = 55f;



    [Header("GameObject layering")]
    public LayerMask combinedMask;

    public LayerMask pickableMask;      //esine jota voi raahata ympäriinsä
    public LayerMask grabbableMask;
    public LayerMask interactableMask;  //esine joka avaa vaik oven, joku koodialusta tai poimii sen esineen ylös tai ihan mitä vaan muuta

    int pickableMaskIndex;
    int interactableMaskIndex;
    int grabbableMaskIndex;

    public static bool disconnectedMouse = false;
    public static bool moving = false;

    bool requireMousePress = false;
    bool pressingMouse = false;
    bool holdingMouse = false;

    bool runPhysicsInteraction = false;

    public static bool rotating = false;
    float rotationHorizontal;
    float rotationVertical;

    Vector3 movementVector = new Vector3();

    public static bool hoveringStuff = false;
    public static bool interactingStuff = false;
    public static bool grabbingStuff = false;

    GameObject other;
    Rigidbody otherRb;

    float distance;
    Vector3 pointToTransform;
    Vector3 objectMovement;

    Quaternion qOriginalRotation;
    Quaternion qOriginalPlayerRotation;

    ObjectClass objectType = ObjectClass.None;

    private MeshCollider[] colliders;
    bool collisionLimit = false;
    float dist;
    float pushAngle;

    CollisionAdjuster ca;

    SocketableObject socketableObject;
    SocketScript socketScript;
    [HideInInspector]
    public bool socketedObject;
    [HideInInspector]
    public float interactionForce = 0f;

    SwitchScript switchScript;

    void Start()
    {
        //Hide and lock the cursor in the middle of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        interactableMaskIndex = LayerMask.NameToLayer("InteractableObject");
        pickableMaskIndex = LayerMask.NameToLayer("PickableObject");
        grabbableMaskIndex = LayerMask.NameToLayer("GrabbableObject");
    }

    void Update()
    {
        Movement();
        CameraRotation();
        ui.PointerTransitions(objectType);
        Interaction();
    }

    private void FixedUpdate() //Run this in FixedUpdate, since it's very physics-heavy
    {
        if (runPhysicsInteraction)
        {
            RunObjectInteraction();
        }
    }

    //Collision detection
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 force;

        if (body == null || body.isKinematic) { return; }

        if (hit.moveDirection.y < -0.3)
        {
            force = new Vector3(0, -0.5f, 0) * gravity * weight;
        }
        else
        {
            force = hit.controller.velocity * pushPower;
        }

        body.AddForceAtPosition(force, hit.point);
    }

    //Camera rotation
    private void CameraRotation()
    {
        if (Input.GetKey(KeyCode.E) && hoveringStuff) //We check physics rotation here!
        {
            rotating = true;
        }
        else
        {
            rotating = false;
        }

        if (!rotating)
        {
            horizontal = Input.GetAxis("Mouse X") * mouseSpeed;
            vertical = -1 * Input.GetAxis("Mouse Y") * mouseSpeed;

            //Camera rotation restrictions
            bool restrictVerticalRotation = false;
            if (cam.transform.rotation.eulerAngles.x >= verticalRotationMax && cam.transform.rotation.eulerAngles.x < 180)
            {
                if (vertical > 0)
                {
                    restrictVerticalRotation = true;
                }
            }
            if (cam.transform.rotation.eulerAngles.x <= 360 + verticalRotationMin && cam.transform.rotation.eulerAngles.x > 180)
            {
                if (vertical < 0)
                {
                    restrictVerticalRotation = true;
                }
            }

            //Camera rotation according to mouse movement
            if (!disconnectedMouse)
            {
                if (!restrictVerticalRotation)
                {
                    cam.transform.Rotate(vertical, 0, 0);
                }

                player.transform.Rotate(0, horizontal, 0);
            }

            //Keep cam Z-rotation 0
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, 0);
        }
    }

    //Movement functionality
    private void Movement()
    {
        if (controller.isGrounded)
        {
            curGravity = gravity;
        }
        else
        {
            curGravity += gravity;
        }

        movementVector = new Vector3(0, -curGravity * Time.deltaTime, 0);

        moving = false;

        if (Input.GetKey(KeyCode.W))
        {
            moving = true;
            if (invertControls)
            {
                movementVector += player.transform.forward * movementSpeed * Time.deltaTime * -1;
            }
            else
            {
                movementVector += player.transform.forward * movementSpeed * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            moving = true;
            if (invertControls)
            {
                movementVector += player.transform.right * movementSpeed * Time.deltaTime;
            }
            else
            {
                movementVector += player.transform.right * movementSpeed * Time.deltaTime * -1;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            moving = true;
            if (invertControls)
            {
                movementVector += player.transform.forward * movementSpeed * Time.deltaTime;
            }
            else
            {
                movementVector += player.transform.forward * movementSpeed * Time.deltaTime * -1;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            moving = true;
            if (invertControls)
            {
                movementVector += player.transform.right * movementSpeed * Time.deltaTime * -1;
            }
            else
            {
                movementVector += player.transform.right * movementSpeed * Time.deltaTime;
            }
        }

        controller.Move(movementVector);
    }

    //Interaction with gameobjects and the world
    private void Interaction()
    {
        RaycastHit rch;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out rch, interactionDistanceMax, combinedMask, QueryTriggerInteraction.UseGlobal) || hoveringStuff || grabbingStuff)
        {
            if (!hoveringStuff && !grabbingStuff) //Identify the type of object we are looking at
            {
                if (rch.collider.gameObject.layer == interactableMaskIndex)
                {
                    objectType = ObjectClass.Interactable;
                }
                else if (rch.collider.gameObject.layer == pickableMaskIndex)
                {
                    objectType = ObjectClass.Pickable;
                }
                else if (rch.collider.gameObject.layer == grabbableMaskIndex)
                {
                    objectType = ObjectClass.Grabbable;
                }
                else
                {
                    objectType = ObjectClass.None;
                }
            }

            //Check mouse states
            pressingMouse = Input.GetMouseButtonDown(0);
            if (requireMousePress && pressingMouse)
            {
                requireMousePress = false;
            }

            if (requireMousePress)
            {
                holdingMouse = false;
            }
            else
            {
                holdingMouse = Input.GetMouseButton(0);
            }


            if (holdingMouse || pressingMouse)
            {
                if (!hoveringStuff && objectType == ObjectClass.Interactable) //Interaction with buttons, levers, code pads, whatever
                {
                    if (!interactingStuff && pressingMouse)
                    {
                        interactingStuff = true;

                        other = rch.collider.gameObject;
                        switchScript = other.GetComponent<SwitchScript>();
                        if (switchScript == null)
                        {
                            switchScript = other.GetComponentInParent<SwitchScript>();
                        }

                        switchScript.HoldEvents();
                    }
                }
                else if (hoveringStuff || (objectType == ObjectClass.Pickable || objectType == ObjectClass.Grabbable)) //Interaction with physics objects
                {
                    //Picking stuff up -code
                    if (!hoveringStuff)
                    {
                        other = rch.collider.gameObject;

                        //Get the rigidbody
                        otherRb = other.GetComponent<Rigidbody>();
                        if (otherRb == null)
                        {
                            otherRb = other.GetComponentInParent<Rigidbody>();
                            other = otherRb.gameObject;
                        }

                        grabbingStuff = objectType == ObjectClass.Grabbable;

                        if (!grabbingStuff)
                        {
                            //Add the collision adjuster
                            ca = other.GetComponent<CollisionAdjuster>();
                            if (ca == null)
                            {
                                ca = other.AddComponent<CollisionAdjuster>();
                            }
                            else
                            {
                                ca.removeScript = false;
                            }

                            //Check if it's a socketed object
                            socketableObject = other.GetComponent<SocketableObject>();
                            socketedObject = false;
                            if (socketableObject != null)
                            {
                                if (socketableObject.socketConnectedTo != null) //It is a socketable object and it is attached to a socket
                                {
                                    socketScript = socketableObject.socketConnectedTo;
                                    socketScript.Interacting(this);
                                    socketedObject = true;
                                }
                                else //It is a socketable object and it is not attached to a socket
                                {
                                    socketableObject.GetSockets(socketableObject, this);
                                }
                            }
                        }

                        //Set rigidbody values
                        otherRb.useGravity = false;

                        if(!grabbingStuff)
                        {
                            otherRb.interpolation = RigidbodyInterpolation.Interpolate;
                            otherRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        }
                        

                        distance = Vector3.Distance(cam.transform.position, other.transform.position);

                        qOriginalRotation = other.transform.rotation;
                        qOriginalPlayerRotation = player.transform.rotation;

                        //Ignore collision with the player
                        colliders = other.GetComponents<MeshCollider>();
                        if (colliders == null)
                        {
                            colliders = other.GetComponentsInChildren<MeshCollider>();
                        }
                        foreach (MeshCollider mc in colliders)
                        {
                            Physics.IgnoreCollision(controller, mc, true);
                            mc.material = physicMaterial;
                        }

                        hoveringStuff = true;

                        //Start hovering the object (we run this in FixedUpdate)
                        runPhysicsInteraction = true;
                    }

                    if (!grabbingStuff)
                    {
                        //Distance variability with scroll wheel
                        float scroll = Input.GetAxis("Mouse ScrollWheel");
                        if ((scroll > 0 && distance < interactionDistanceMax) || (scroll < 0 && distance > interactionDistanceMin))
                        {
                            distance += scroll;
                        }
                    }
                }
            }
            else
            {
                //Releasing held object
                if (hoveringStuff)
                {
                    runPhysicsInteraction = false;

                    if (!grabbingStuff)
                    {
                        //We released a socketed object, that was not detached
                        if (socketedObject)
                        {
                            Destroy(ca);
                            socketScript.StoppedInteraction();
                            otherRb.interpolation = RigidbodyInterpolation.Interpolate;
                        }
                        else
                        {
                            ca.removeScript = true;

                            //Add some velocity to the object
                            Vector3 toBeTranslation = (pointToTransform - other.transform.position);
                            otherRb.velocity = toBeTranslation * (throwForce / otherRb.mass) * Time.deltaTime;

                            Vector3 cross = Vector3.Cross(toBeTranslation, other.transform.position - player.transform.position);
                            otherRb.angularVelocity = -cross * (throwAngularForce / otherRb.mass);

                            //Debug.DrawRay(other.transform.position, -cross, Color.magenta, 2f);
                        }
                    }

                    otherRb.useGravity = true;
                    rotationHorizontal = 0f;
                    rotationVertical = 0f;

                    //Enable collisions with player
                    foreach (MeshCollider mc in colliders)
                    {
                        mc.material = null;
                        Physics.IgnoreCollision(controller, mc, false);
                    }

                    hoveringStuff = false;
                    grabbingStuff = false;
                }
                else if (interactingStuff)
                {
                    switchScript.ReleaseEvents();
                    switchScript = null;
                    interactingStuff = false;
                }
            }
        }
        else
        {
            if (interactingStuff)
            {
                switchScript.ReleaseEvents();
                switchScript = null;
                interactingStuff = false;
            }

            objectType = ObjectClass.None;
        }
    }

    //Call this to cut interaction with anything
    public void CutInteraction()
    {
        holdingMouse = false;
        requireMousePress = true;
    }

    //Interaction with physics objects
    private void RunObjectInteraction()
    {
        pointToTransform = cam.transform.position + cam.transform.forward * distance;
        objectMovement = pointToTransform - other.transform.position;

        if (!grabbingStuff)
        {
            collisionLimit = CollisionLimiter(other.transform.position, pointToTransform, objectMovement);
        }
        else
        {
            collisionLimit = false;
        }

        if (collisionLimit) //The object is colliding with a wall, we should limit the force applied to prevent clipping
        {
            otherRb.AddForce((objectMovement) * (((socketedObject ? interactionForce : pickUpSpeed) * Time.deltaTime) / (1 / pushAngle * dist * clippingFrictionMultiplier)), ForceMode.Impulse);
        }
        else
        {
            otherRb.AddForce((objectMovement) * (socketedObject ? interactionForce : (grabbingStuff ? grabbingForce : pickUpSpeed)) * (moving ? movingMultiplier : 1f) * Time.deltaTime, ForceMode.Impulse);


            if (!grabbingStuff)
            {
                //OBJECT ROTATION
                Quaternion start = other.transform.rotation;
                Quaternion target;

                //Manual rotation
                if (rotating)
                {
                    rotationHorizontal = -1 * Input.GetAxis("Mouse X") * Time.deltaTime;
                    rotationVertical = Input.GetAxis("Mouse Y") * Time.deltaTime;

                    qOriginalRotation = other.transform.rotation;
                    qOriginalPlayerRotation = player.transform.rotation;

                    target = player.transform.rotation * Quaternion.AngleAxis(rotationHorizontal, Vector3.up) * Quaternion.AngleAxis(rotationVertical, Quaternion.AngleAxis(-rotationHorizontal, Vector3.up) * Vector3.right) * Quaternion.Inverse(qOriginalPlayerRotation) * qOriginalRotation;
                }
                else
                {
                    //Rotate the object as the player moves
                    target = player.transform.rotation * Quaternion.Inverse(qOriginalPlayerRotation) * qOriginalRotation;
                }
                Quaternion change = target * Quaternion.Inverse(start);

                otherRb.AddTorque(change.x * (rotating ? manualRotationSpeed * otherRb.mass : lookRotationSpeed) * Time.deltaTime, change.y * (rotating ? manualRotationSpeed * otherRb.mass : lookRotationSpeed) * Time.deltaTime, change.z * (rotating ? manualRotationSpeed * otherRb.mass : lookRotationSpeed) * Time.deltaTime, ForceMode.Impulse);
            }
        }

        otherRb.angularVelocity = Vector3.zero;
        otherRb.velocity = Vector3.zero;
    }

    //Describes if we should apply collision related force restrictions like friction
    private bool CollisionLimiter(Vector3 a, Vector3 b, Vector3 abMovement)
    {
        //Debug.DrawLine(a, a + (b - a).normalized * dist, Color.blue, 0.3f);
        RaycastHit rch;

        dist = Vector3.Distance(a, b);
        if (Physics.Raycast(new Ray(a, (abMovement).normalized), out rch, dist, 1 << 0, QueryTriggerInteraction.UseGlobal))
        {
            Ray r = new Ray(rch.point, rch.normal);

            //Debug.DrawLine(r.origin, r.origin - r.direction, Color.red, 0.3f); //push vector and inverted surface normal
            pushAngle = Vector3.Angle((abMovement).normalized * dist, -r.direction).Map(0f, 90f, 0.01f, 1f);
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum ObjectClass
{
    Pickable,
    Interactable,
    Grabbable,
    None,
}

public static class Extentions
{
    public static float Map(this float x, float fromMin, float fromMax, float toMin, float toMax)
    {
        var m = (toMax - toMin) / (fromMax - fromMin);
        var c = toMin - m * fromMin; // point of interest: c is also equal to toMax - m * fromMax, though float math might lead to slightly different results.

        return m * x + c;
    }
}
