using NVIDIA.Flex;
using UnityEngine;

//Handles camera movement and object controlling
public class Controls : Singleton<Controls>
{
    [Header("References")]
    public Camera mainCamera;

    [Space]

    public Transform cameraPivot;
    public Transform glassParent;

    [Header("Options")]

    public float cameraSensitivityOrbit;

    [Space]

    public AnimationCurve cameraZoomDistance;
    public float cameraZoomDistanceMin;
    public float cameraZoomDistanceMax;
    public float cameraZoomSensitivity;

    float cameraZoom = 0.5f;

    [Space]

    public float movementSpeed;

    [Space]

    public float hoverDistance;
    public float grabIntensity;
    public float pourHeightMultiplier;

    [Space]

    public float fluidPourAngle;

    [Space]

    public float rotationSpeed;
    public float swayIntensity;

    [Space]

    public float pourAmount;
    public Vector3 targetCorrection;


    //Layers
    [HideInInspector]
    public int smallObjects;
    [HideInInspector]
    public int largeObjects;
    [HideInInspector]
    public int unmovable;

    bool hoveringOverObject = false;
    RaycastHit selectableRaycastHit;
    GameObject selectableGameObject;
    Rigidbody selectableGameObjectRb;

    [HideInInspector]
    public ObjectType selectableObjectType;

    bool isBottle = false;
    Transform bottleTop;

    [HideInInspector]
    public bool movingObject = false;

    Ray previousPhysicsMouseToWorld; //Used if mouse goes out-of-bounds somehow
    Ray physicsMouseToWorld;

    Vector3 originalMousePosition;
    Vector3 originalEuler;
    Vector3 mouseDelta;
    bool mouseCheck = false;

    const float sine45 = 0.707106f;

    bool pouring = false;
    Transform closest = null;
    float closestDistance = Mathf.Infinity;

    FlexSourceActor pourScript;
    GameObject pourEffectObject;
    bool useSameFlexContainer = false;

    Container fluidContainer;
    GameObject targetObject;

    //Quaternion originalQ;
    //Vector3 originalV;
    //Vector3 originalPointy;

    public enum ObjectType
    {
        Small,
        Large,
    }

    // Start is called before the first frame update
    void Start()
    {
        smallObjects = LayerMask.GetMask("Small Objects");
        largeObjects = LayerMask.GetMask("Large Objects");
        unmovable = LayerMask.GetMask("Unmovable");

        targetObject = new GameObject("Target");
    }

    // Update is called once per frame
    void Update()
    {
        //CAMERA
        CameraOrbit();
        CameraZoom();
        //CameraMovement();

        //OBJECTS
        ObjectHighlight();
        ObjectLifting();
        ObjectPouring();

        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }

    #region Camera

    void CameraOrbit()
    {
        //Orbiting
        if (Input.GetMouseButton(2))
        {
            if (!mouseCheck)
            {
                originalMousePosition = Input.mousePosition;
                originalEuler = cameraPivot.rotation.eulerAngles;
                mouseCheck = true;
            }

            Vector3 mousePos = Input.mousePosition;
            mouseDelta = (mousePos - originalMousePosition) * cameraSensitivityOrbit;

            cameraPivot.rotation = Quaternion.Euler(-mouseDelta.y + originalEuler.x, mouseDelta.x + originalEuler.y, 0);
            //mainCamera.transform.rotation = Quaternion.LookRotation(cameraPivot.position - mainCamera.transform.position, mainCamera.transform.up);
        }
        else
        {
            mouseCheck = false;
        }
    }

    void CameraZoom()
    {
        //Zooming
        Vector2 scroll = Input.mouseScrollDelta;
        if (scroll.y != 0f)
        {
            cameraZoom += scroll.y * cameraZoomSensitivity * Time.deltaTime;
            cameraZoom = Mathf.Clamp01(cameraZoom);

            //Position
            Vector3 direction = (mainCamera.transform.position - cameraPivot.position).normalized;
            float distance = cameraZoomDistance.Evaluate((1 - cameraZoom)).Remap(0, 1, cameraZoomDistanceMin, cameraZoomDistanceMax);

            Vector3 final = direction * distance;
            mainCamera.transform.position = cameraPivot.transform.position + final;

        }
    }

    void CameraMovement()
    {
        //Movement
        Vector3 forward = new Vector3(mainCamera.transform.forward.x, 0f, mainCamera.transform.forward.z);
        Vector3 right = new Vector3(mainCamera.transform.right.x, 0f, mainCamera.transform.right.z);

        Vector3 movement = Vector3.zero;
        int numberOfKeysPressed = 0;

        if (Input.GetKey(KeyCode.W))
        {
            numberOfKeysPressed++;
            movement += forward.normalized;
        }
        if (Input.GetKey(KeyCode.A))
        {
            numberOfKeysPressed++;
            movement += -right.normalized;
        }
        if (Input.GetKey(KeyCode.S))
        {
            numberOfKeysPressed++;
            movement += -forward.normalized;
        }
        if (Input.GetKey(KeyCode.D))
        {
            numberOfKeysPressed++;
            movement += right.normalized;
        }

        cameraPivot.position += movement * (numberOfKeysPressed == 2 ? sine45 : 1f) * Time.deltaTime * movementSpeed;
    }

    #endregion

    #region Object

    void ObjectHighlight()
    {
        //Check for selectable objects
        if (!movingObject)
        {
            Ray mouseToWorld = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseToWorld, out selectableRaycastHit, 1000f, smallObjects | largeObjects))
            {
                hoveringOverObject = true;

                selectableGameObject = selectableRaycastHit.transform.gameObject;
                selectableGameObjectRb = selectableRaycastHit.rigidbody;
            }
            else
            {
                hoveringOverObject = false;
            }
        }
    }

    void ObjectLifting()
    {
        //If we hold lmb, lift the object up
        if (Input.GetMouseButton(0))
        {
            if (hoveringOverObject && !movingObject)
            {
                //We divide objects into small and large:   small objects avoid large objects and structures
                //                                          large objects avoid only structures
                if (selectableGameObject.layer == 10)
                {
                    selectableObjectType = ObjectType.Small;
                }
                else
                {
                    selectableObjectType = ObjectType.Large;
                }
                selectableGameObjectRb.useGravity = false;

                isBottle = selectableGameObject.tag == "Bottle";
                if (isBottle)
                {
                    bottleTop = selectableGameObject.transform.GetChild(0);
                }

                //originalQ = selectableGameObject.transform.rotation;
                //originalV = selectableGameObject.transform.position;
                //originalPointy = selectableRaycastHit.point - selectableGameObject.transform.position;

                movingObject = true;

                //Start tracking this rigidbody
                if(selectableGameObject.layer == 11)
                {
                    NavMeshUpdater.Instance.bodiesAffected.Add(selectableGameObjectRb);
                }
                selectableGameObject.AddComponent<NavMeshUpdateTracker>();

            }
        }
        else
        {
            if (movingObject)
            {
                DisconnectGrab();
            }
        }
    }

    void ObjectPouring()
    {
        //Pouring
        if (movingObject && isBottle)
        {
            if (Input.GetMouseButton(1))
            {
                //Find the closest glass to pour towards
                //Optimize this with a mouse movement check?
                Transform[] children = glassParent.GetComponentsInChildren<Transform>();
                closestDistance = Mathf.Infinity;

                foreach (Transform t in children)
                {
                    float dist = (selectableGameObject.transform.position - t.position).magnitude;
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closest = t;
                    }
                }

                if (!pouring)
                {
                    pouring = true;

                    if (!useSameFlexContainer)
                    {
                        useSameFlexContainer = true;

                        int pointer = FluidManager.Instance.GetPointer();

                        //Load fluidContainer from FluidManager
                        pourEffectObject = FluidManager.Instance.fluidObjects[pointer];
                        pourScript = pourEffectObject.GetComponent<FlexSourceActor>();

                        //Get bottle liquidContainer info
                        fluidContainer = selectableGameObject.GetComponent<Container>();

                        //Update the material info at _auxFlexDrawFluid.cs
                        Material fluidMat = fluidContainer.liquidMix.liquids[0];
                        FluidManager.Instance.fluids[pointer].UpdateMaterial(fluidMat);
                    }
                }
            }
            else
            {
                CutPouring();
            }
        }
        else
        {
            CutPouring();
        }
    }

    void CutPouring()
    {
        if (pouring)
        {
            pouring = false;
            pourScript.isActive = false;

            //Destroy(pourEffectObject);
        }
    }

    #endregion

    //Grabbing objects and whatnot
    private void FixedUpdate()
    {
        if (movingObject)
        {
            //Position
            previousPhysicsMouseToWorld = physicsMouseToWorld;
            physicsMouseToWorld = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit rch;
            if (selectableObjectType == ObjectType.Small)
            {
                if (!Physics.Raycast(physicsMouseToWorld, out rch, 1000f, unmovable | largeObjects))
                {
                    DisconnectGrab();
                }
            }
            else //Large
            {
                if (!Physics.Raycast(physicsMouseToWorld, out rch, 1000f, unmovable))
                {
                    DisconnectGrab();
                }
            }

            //Quaternion deltaQ = selectableGameObject.transform.rotation * Quaternion.Inverse(originalQ);
            //Vector3 deltaV = selectableGameObject.transform.position - originalV;

            //Vector3 newHitPos = (deltaQ * originalPointy) + deltaV;
            //(deltaQ * originalPointy).Draw(selectableRaycastHit.point);
            //(rch.point - physicsMouseToWorld.direction * hoverDistance) -newHitPos, newHitPos, ForceMode.VelocityChange

            //selectableGameObjectRb.AddForceAtPosition(Vector3.up, (deltaQ * originalPointy), ForceMode.Acceleration);
            selectableGameObjectRb.AddForce(((rch.point - physicsMouseToWorld.direction * hoverDistance) - selectableGameObject.transform.position) * grabIntensity, ForceMode.VelocityChange);



            //Rotation
            Quaternion start = selectableGameObject.transform.rotation;
            Quaternion target = Quaternion.identity * Extensions.RotationBetween(Vector3.up, selectableGameObjectRb.velocity + Vector3.up * swayIntensity);

            if (pouring)
            {
                //Do not point the bottle head straight at the glass, a little bit above
                Vector3 fixedPosition = new Vector3(closest.position.x, closest.position.y + (closestDistance * pourHeightMultiplier), closest.position.z);

                float angleForward = Vector3.Angle(Vector3.forward, (closest.position - selectableGameObject.transform.position).RemoveHeightFromVector());
                float correction = Vector3.Cross(Vector3.right, (closest.position - selectableGameObject.transform.position)).y;

                target *= Extensions.RotationBetween(Vector3.up, fixedPosition - selectableGameObject.transform.position) * Quaternion.Euler(0, angleForward * correction, 0);

                //Fluid
                pourEffectObject.transform.position = bottleTop.position;
                pourEffectObject.transform.rotation = bottleTop.rotation * Quaternion.Euler(90,0,0);

                //Fluid enable
                bool angleCheck = Vector3.Angle(Vector3.up, selectableGameObject.transform.up) > fluidPourAngle;
                bool hasFluid = fluidContainer.fullness > 0f;
                if(angleCheck && hasFluid)
                {
                    pourScript.isActive = true;

                    //Reduce fluid in bottle
                    if(fluidContainer.volume > 0)
                    {
                        fluidContainer.fullness -= pourAmount / fluidContainer.volume;
                        fluidContainer.fullness = Mathf.Clamp01(fluidContainer.fullness);
                    }

                    //Fill container
                    RaycastHit glassHit;
                    if(Physics.Raycast(bottleTop.position, bottleTop.up + targetCorrection, out glassHit, Mathf.Infinity, smallObjects | unmovable))
                    {
                        targetObject.transform.position = glassHit.point;
                        GameObject g = glassHit.transform.gameObject;
                        if (g.tag == "Glass" /*|| g.tag == "Bottle"*/ && Vector3.Angle(glassHit.normal,Vector3.up) < 15f)
                        {
                            Container c = g.GetComponent<Container>();
                            if(c.volume > 0)
                            {
                                c.fullness += pourAmount / c.volume;
                                c.fullness = Mathf.Clamp01(c.fullness);
                            }
                        }
                    }
                }
                else
                {
                    pourScript.isActive = false;
                }
            }

            Quaternion change = target * Quaternion.Inverse(start);
            selectableGameObjectRb.AddTorque(change.x * rotationSpeed, change.y * rotationSpeed, change.z * rotationSpeed, ForceMode.VelocityChange);

            //Magic
            selectableGameObjectRb.velocity = Vector3.zero;
            selectableGameObjectRb.angularVelocity = Vector3.zero;
        }
    }

    void DisconnectGrab()
    {
        useSameFlexContainer = false;

        movingObject = false;
        isBottle = false;
        selectableGameObjectRb.useGravity = true;
    }
}
