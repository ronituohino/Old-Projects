using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionBox : MonoBehaviour
{
    public SocketScript greenSocket;
    public SocketScript blueSocket;

    public SwitchScript switchScript;

    public MeshRenderer baseLight;

    public Material greenMat;
    public Material redMat;

    public bool boxRunning;

    public void StartBox()
    {
        if(greenSocket.socketed && greenSocket.socketedObject.name.Contains("Green") && blueSocket.socketed && blueSocket.socketedObject.name.Contains("Blue") && !switchScript.switchState)
        {
            baseLight.sharedMaterial = greenMat;
            boxRunning = true;
        } else
        {
            baseLight.sharedMaterial = redMat;
            boxRunning = false;
        }
    }
}
