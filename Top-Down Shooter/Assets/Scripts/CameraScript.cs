using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera cam;
    public PlayerController pc;

    public bool moveCamera = true;

    [Space]

    public float cameraLerp;
    public float cameraLerpEdgeMultiplier;
    public float maxCameraDistance;

    [Space]

    public float cameraZoomMin;
    public float cameraZoomMax;
    public float zoomDampening;
    float zoom;
    Vector3 targetPreviousPos;

    private void LateUpdate()
    {
        //Camera movement
        if (moveCamera && !ControlPanel.Instance.controlPanelOpen)
        {
            Vector3 cameraTargetPosition = (pc.rb.position + pc.target.position.ToVector2()) / 2f;
            Vector3 camPos = cam.transform.position;

            Vector3 vel = Vector3.zero;
            Vector3 targetLerp = Vector3.SmoothDamp(
                camPos, 
                new Vector3(cameraTargetPosition.x, cameraTargetPosition.y, -10f), 
                ref vel, 
                cameraLerp * Time.deltaTime
            );

            vel = Vector3.zero;
            float cameraDistance = (targetLerp.ToVector2() - pc.rb.position).magnitude;
            if (cameraDistance >= maxCameraDistance)
            {
                Vector3 v = (pc.target.position.ToVector2() - pc.rb.position).normalized * maxCameraDistance + pc.rb.position;
                targetLerp = Vector3.SmoothDamp(camPos, new Vector3(v.x, v.y, -10), ref vel, Time.deltaTime * cameraLerp * cameraLerpEdgeMultiplier);
            }
            cam.transform.position = targetLerp;

            //Zoom
            zoom += Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.RoundToInt(zoom * 100) / 100f == 0f)
            {
                zoom = 0f;
            }
            else if (zoom > 0)
            {
                zoom -= zoomDampening * Time.deltaTime;
            }
            else if (zoom < 0)
            {
                zoom += zoomDampening * Time.deltaTime;
            }
            cam.orthographicSize -= zoom;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, cameraZoomMin, cameraZoomMax);
        }
    }
}
