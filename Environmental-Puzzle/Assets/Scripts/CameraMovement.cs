using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera cam;
    [SerializeField] Transform cameraRig;

    [Space]

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [SerializeField] Vector2 camMovementSpeedMultiplier;

    [Space]

    [SerializeField] float maxYAngle;
    [SerializeField] float minYAngle;

    [Space]

    [SerializeField] float camLerp;

    [Space]

    [SerializeField] float maxCamDistance;
    [SerializeField] float minCamDistance;
    [SerializeField] float camZoomLerp;

    [SerializeField] float camZoomSpeedMultiplier;

    [Space]

    [SerializeField] float maxHeight;
    [SerializeField] float minHeight;
    [SerializeField] float heightChangeSpeedMultiplier;
    float height = 0f;

    bool startedDrag = true;
    Vector2 originalLocalEuler;

    float xAngle = 30f;
    float yAngle = 30f;

    float camDistance = 5f;
    float targetZoom = 5f;

    private void Update()
    {
        // Movement
        Vector2 lerpStartEul = GetLocalEuler();

        cameraRig.localEulerAngles = new Vector3
                                        (
                                            Mathf.LerpAngle(lerpStartEul.x, yAngle, camLerp),
                                            Mathf.LerpAngle(lerpStartEul.y, xAngle, camLerp),
                                            0
                                        );

        // Zooming
        if(!InputManager.Instance.holdingShift && !InputManager.Instance.interactingWithPhysicsObject)
        {
            targetZoom = targetZoom - (InputManager.Instance.mouseScroll * camZoomSpeedMultiplier);
            targetZoom = Mathf.Clamp(targetZoom, minCamDistance, maxCamDistance);

            camDistance = Mathf.Lerp(camDistance, targetZoom, camZoomLerp);

            // Perspective cam
            //cam.transform.localPosition = new Vector3(0, 0, -camDistance);

            // Orthographic cam
            cam.orthographicSize = camDistance;
        }
        // Up / Down
        else if(InputManager.Instance.holdingShift && !InputManager.Instance.interactingWithPhysicsObject)
        {
            height += InputManager.Instance.mouseScroll * heightChangeSpeedMultiplier;
            height = Mathf.Clamp(height, minHeight, maxHeight);

            cameraRig.position = Vector3.Lerp(cameraRig.position, new Vector3(0, height, 0), camLerp);
        } 
    }

    public void MoveCamera(Vector2 totalMouseMovement)
    {
        if(startedDrag)
        {
            originalLocalEuler = GetLocalEuler();
        }

        xAngle = originalLocalEuler.y + (totalMouseMovement.x * camMovementSpeedMultiplier.x) * (invertX ? -1 : 1);

        yAngle = originalLocalEuler.x + (totalMouseMovement.y * camMovementSpeedMultiplier.y) * (invertY ? -1 : 1);
        yAngle = Mathf.Clamp(yAngle, minYAngle, maxYAngle);

        startedDrag = false;
    }

    Vector2 GetLocalEuler()
    {
        Vector2 eul = cameraRig.localEulerAngles.ToVector2();
        if (eul.x > 180f)
        {
            eul.x -= 360f;
        }
        if (eul.y > 180f)
        {
            eul.y -= 360f;
        }

        return eul;
    }

    public void ReleaseDrag()
    {
        startedDrag = true;
    }
}
