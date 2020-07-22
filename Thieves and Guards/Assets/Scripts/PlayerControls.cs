using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public GameObject skeletalParent;
    public GameObject camFocusObject;
    public GameObject headRotateObj;
    public CharacterController cc;
    public GameObject cameraRotation;
    Camera cam;

    [Header("Movement")]
    public AnimationCurve movementCurve;

    [HideInInspector]
    public bool movementLock = false;

    public float walkingSpeed;
    public float strafingMultiplier;
    public float runningMultiplier;

    public float rotationSpeed;

    [Space]

    float speed;
    float direction;
    
    float targetSpeed;
    float previousTargetSpeed;

    float targetDirection;
    float previousTargetDirection;

    public float animationMovingSharpness;
    public float animationStrafingSharpness;

    bool moving = false;
    bool running = false;
    bool strafing = false;
    float movementKeysPressed = 0f;

    bool lockSpeed = false;
    bool lockDirection = false;

    [Header("Camera settings")]
    public float maxAnglesX;
    public float minAnglesX;

    [Space]

    public float distance;
    public float focusRotationSpeed;


    [Header("Miscellaneous")]

    public float headMaxAnglesY; //These ones only affect camera head rotation, not camera
    public float headMinAnglesY;

    public float headTurnRate;

    Quaternion headVertical = Quaternion.identity;
    Quaternion headHorizontal = Quaternion.identity;

    LayerMask playerLayer;

    bool cameraLock = false;

    bool focusingCam = false;
    Vector3 pos;

    float totalCamXZRotation = 0f;


    Vector3 curPos;
    Vector3 lastPos;
    float velocity;


    public struct InputMap
    {
        public bool w;
        public bool a;
        public bool s;
        public bool d;
        public bool shift;

        public InputMap(bool w, bool a, bool s, bool d, bool shift)
        {
            this.w = w;
            this.a = a;
            this.s = s;
            this.d = d;
            this.shift = shift;
        }
    }

    InputMap currentInput;

    private void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");

        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    //Rigidbody movement and rotation
    private void FixedUpdate()
    {
        if (!movementLock)
        {
            curPos = cc.transform.position;
            velocity = (curPos - lastPos).magnitude;

            movementKeysPressed = 0.33f;
            if (currentInput.w)
            {
                movementKeysPressed += 0.66f;
            }
            if (currentInput.a)
            {
                movementKeysPressed += 0.66f;
            }
            if (currentInput.s)
            {
                movementKeysPressed += 0.66f;
            }
            if (currentInput.d)
            {
                movementKeysPressed += 0.66f;
            }
            
            //Movement
            if (currentInput.w) //w
            {
                cc.SimpleMove(-skeletalParent.transform.right * Time.deltaTime * walkingSpeed * (running ? runningMultiplier : 1f) * (1f / movementKeysPressed));
            }
            if (currentInput.s) //s
            {
                cc.SimpleMove(skeletalParent.transform.right * Time.deltaTime * walkingSpeed * (running ? runningMultiplier : 1f) * (1f / movementKeysPressed));
            }
            if(currentInput.a) //a
            {
                cc.SimpleMove(skeletalParent.transform.up * Time.deltaTime * walkingSpeed * (running ? runningMultiplier : 1f) * strafingMultiplier * (1f / movementKeysPressed));
            }
            if(currentInput.d) //d
            {
                cc.SimpleMove(-skeletalParent.transform.up * Time.deltaTime * walkingSpeed * (running ? runningMultiplier : 1f) * strafingMultiplier * (1f / movementKeysPressed));
            }

            //Rotation
            if (!cameraLock)
            {
                if (moving || strafing) //just w
                {
                    skeletalParent.transform.rotation = Quaternion.RotateTowards(skeletalParent.transform.rotation, Quaternion.Euler(-90, cameraRotation.transform.eulerAngles.y + 90, 0), Time.deltaTime * rotationSpeed * (running ? runningMultiplier : 1f));
                }
            }

            lastPos = curPos;
        }
    }

    //Camera positioning, running switch and animation stuff
    void Update()
    {
        //Camera and head animations
        CameraFunction();

        //Get input
        currentInput = new InputMap(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.S), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.LeftShift));
        running = currentInput.shift;
        moving = currentInput.w || currentInput.s;
        strafing = currentInput.d || currentInput.a;

        //Debug
        if (Input.GetKey(KeyCode.Space))
        {
            GetComponent<Character>().FallDown();
        }

        //Handle all the animations associated with moving
        MovingAnimations();
    }

    void CameraFunction()
    {
        if (!focusingCam)
        {
            //Camera rotation
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //Y axis
            cam.transform.RotateAround(camFocusObject.transform.position, Vector3.up, mouseDelta.x);



            //Set head rotation of the character head
            float angleBodyForwardCamera = Vector3.Angle(-skeletalParent.transform.right, (cam.transform.forward - (cam.transform.position - skeletalParent.transform.position)));

            headVertical = Quaternion.AngleAxis(totalCamXZRotation, Vector3.forward);
            if (angleBodyForwardCamera < headMaxAnglesY && angleBodyForwardCamera > headMinAnglesY)
            {
                headHorizontal = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y + 90, 0);
            }
            headRotateObj.transform.rotation = Quaternion.Lerp(headRotateObj.transform.rotation, headHorizontal * headVertical, headTurnRate);

            float verticalRotation = CameraRotation(cam.transform.rotation.eulerAngles.x);



            //Camera restrictions (X and Z axis)
            if (!(verticalRotation > maxAnglesX && mouseDelta.y > 0) && !(verticalRotation < minAnglesX && mouseDelta.y < 0))
            {
                totalCamXZRotation += mouseDelta.y;
                cam.transform.RotateAround(camFocusObject.transform.position, cam.transform.right, mouseDelta.y);
            }

            //Camera lock
            if (Input.GetMouseButton(1))
            {
                if (!cameraLock)
                {
                    cameraLock = true;
                }
            }
            else
            {
                cameraLock = false;
            }
            cameraRotation.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

            //Place camera infront of objects
            RaycastHit rch;

            bool hitObstacle = false;
            Vector3 camToPlayerDir = (camFocusObject.transform.position - cam.transform.position).normalized;
            if (Physics.Raycast(new Ray(camFocusObject.transform.position, -camToPlayerDir), out rch, distance, ~playerLayer))
            {
                if (rch.collider.gameObject.layer != 10)
                {
                    hitObstacle = true;
                    cam.transform.position = rch.point + (camToPlayerDir * cam.nearClipPlane * 1.5f);
                }
            }

            if (!hitObstacle)
            {
                cam.transform.position = camFocusObject.transform.position - camToPlayerDir * distance;
            }
        }
        else
        {
            cam.transform.position = pos;

            Quaternion oldRotation = cam.transform.rotation;
            cam.transform.LookAt(camFocusObject.transform);
            Quaternion newRotation = cam.transform.rotation;
            cam.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, Time.deltaTime * focusRotationSpeed);
        }
    }

    void MovingAnimations()
    {
        //Set target speed (animation)
        if (!moving && !strafing)
        {
            targetSpeed = 0f;
        }
        else if (moving && !running)
        {
            targetSpeed = 0.5f;
        }
        else if (moving && running)
        {
            targetSpeed = 1f;
        }
        else if (strafing && !running)
        {
            targetSpeed = 0f;
        }
        else if(strafing && running)
        {
            targetSpeed = 0.5f;
        }

        float curveSpeed = movementCurve.Evaluate(speed);
        if (targetSpeed != previousTargetSpeed && lockSpeed)
        {
            lockSpeed = false;
        }

        //Animation speed drivers
        if (!lockSpeed)
        {
            if (curveSpeed < targetSpeed)
            {
                speed += Time.deltaTime * animationMovingSharpness;
                curveSpeed = movementCurve.Evaluate(speed);
                animator.SetFloat("Speed", curveSpeed);

                if (curveSpeed > targetSpeed)
                {
                    lockSpeed = true;
                    animator.SetFloat("Speed", targetSpeed);
                }
            }
            else if (curveSpeed > targetSpeed)
            {
                speed -= Time.deltaTime * animationMovingSharpness;
                curveSpeed = movementCurve.Evaluate(speed);
                animator.SetFloat("Speed", curveSpeed);

                if (curveSpeed < targetSpeed)
                {
                    lockSpeed = true;
                    animator.SetFloat("Speed", targetSpeed);
                }
            }
        }

        previousTargetSpeed = targetSpeed;





        //Set target direction (animation)
        if (!currentInput.a && !currentInput.d)
        {
            targetDirection = 0.5f;
        }
        else if (currentInput.a && !currentInput.d)
        {
            targetDirection = 0f;
        }
        else if (!currentInput.a && currentInput.d)
        {
            targetDirection = 1f;
        } else
        {
            targetDirection = 0.5f;
        }

        float curveDirection = movementCurve.Evaluate(direction);
        if (targetDirection != previousTargetDirection && lockDirection)
        {
            lockDirection = false;
        }

        //Animation direction drivers
        if (!lockDirection)
        {
            if (curveDirection < targetDirection)
            {
                direction += Time.deltaTime * animationStrafingSharpness;
                curveDirection = movementCurve.Evaluate(direction);
                animator.SetFloat("Direction", curveDirection);

                if (curveDirection > targetDirection)
                {
                    lockDirection = true;
                    animator.SetFloat("Direction", targetDirection);
                }
            }
            else if (curveDirection > targetDirection)
            {
                direction -= Time.deltaTime * animationStrafingSharpness;
                curveDirection = movementCurve.Evaluate(direction);
                animator.SetFloat("Direction", curveDirection);

                if (curveDirection < targetDirection)
                {
                    lockDirection = true;
                    animator.SetFloat("Direction", targetDirection);
                }
            }
        }

        previousTargetDirection = targetDirection;
    }

    float CameraRotation(float rotation)
    {
        if(rotation > 180)
        {
            return rotation - 360;
        } else
        {
            return rotation;
        }
    }

    public void FocusCameraOnBody()
    {
        pos = cam.transform.position;
        focusingCam = true;
    }
}
