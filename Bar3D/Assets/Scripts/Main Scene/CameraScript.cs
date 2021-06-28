using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles all Main.scene camerawork
public class CameraScript : MonoBehaviour
{
    [SerializeField] Camera cam;

    [Header("Settings")]
    [SerializeField] float angle;
    [SerializeField] float height;

    [SerializeField] float maxCamMoveSpeed;
    [SerializeField] float cameraDampening;
    Vector3 vel;

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.sceneCamera = cam;
    }

    void Update()
    {
        if (InputManager.Instance.controlledPlayer != null)
        {
            Vector3 toCam = Quaternion.AngleAxis(90f - angle, -Vector3.right) * Vector3.up;
            float stepsToPos = height / toCam.y;

            Vector3 camPos = InputManager.Instance.controlledPlayer.rb.position + stepsToPos * toCam;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camPos, ref vel, cameraDampening, maxCamMoveSpeed);

            cam.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
