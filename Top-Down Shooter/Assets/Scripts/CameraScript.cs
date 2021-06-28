using UnityEngine;

public class CameraScript : Singleton<CameraScript>
{
    public Camera cam;

    [SerializeField] bool moveCamera = true;

    [Space]

    [SerializeField] float cameraLerp;
    public float maxCameraDistance;

    [Space]

    [SerializeField] float cameraZoomMin;
    [SerializeField] float cameraZoomMax;
    [SerializeField] float zoomDampening;
    float zoom;

    Vector2 vel = Vector2.zero;

    private void Update()
    {
        //Camera movement
        if (moveCamera && !ControlPanel.Instance.controlPanelOpen)
        {
            Vector2 cameraTargetPosition = 
                (
                    InputManager.Instance.controlledPlayer.rb.position + 
                    InputManager.Instance.controlledPlayer.target.position.ToVector2()
                ) / 2f;

            Vector2 camPos = cam.transform.position;

            Vector2 targetLerp = Vector2.zero;

            float cameraDistance = (cameraTargetPosition - InputManager.Instance.controlledPlayer.rb.position).magnitude;
            if (cameraDistance >= maxCameraDistance)
            {
                Vector2 v = 
                    (
                        InputManager.Instance.controlledPlayer.target.position.ToVector2() - 
                        InputManager.Instance.controlledPlayer.rb.position
                    ).normalized * maxCameraDistance + InputManager.Instance.controlledPlayer.rb.position;

                targetLerp = Vector2.SmoothDamp
                    (
                        camPos,
                        v,
                        ref vel,
                        cameraLerp * Time.deltaTime
                    );
            }
            else
            {
                targetLerp = Vector2.SmoothDamp
                    (
                        camPos,
                        cameraTargetPosition,
                        ref vel,
                        cameraLerp * Time.deltaTime
                    );
            }
            


            cam.transform.position = new Vector3(targetLerp.x, targetLerp.y, -10f);

            //Zoom
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
