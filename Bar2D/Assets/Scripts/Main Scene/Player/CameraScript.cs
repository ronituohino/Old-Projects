using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    bool setCamera = false;

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.sceneCamera = cam;
    }

    private void Update()
    {
        if (InputManager.Instance.controlledPlayer != null && !setCamera)
        {
            virtualCamera.Follow = InputManager.Instance.controlledPlayer.transform;
        }
        else if(setCamera && InputManager.Instance.controlledPlayer == null)
        {
            virtualCamera.Follow = null;
        }
    }
}
