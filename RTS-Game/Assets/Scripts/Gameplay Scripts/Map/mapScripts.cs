using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class mapScripts : MonoBehaviour {
    public GameObject mapGameObject;
    public RawImage mapImageObject;

    private static GameObject map;
    private static RawImage mapImage;

    static GameObject discoveryMask;
    static GameObject camPos;

    public GameObject camMarker;
    private static GameObject cameraMarker;

    static int mapFScale; //not 2049, 800 brooo

    static float finalX; //-36 to 36
    static float finalY; //-45 to 45
    static float camPosX;
    static float camPosY;

    public void Start()
    {
        map = mapGameObject;//GameObject.Find("Map");
        mapImage = mapImageObject;//map.GetComponentInChildren<RawImage>();
        cameraMarker = camMarker;
        //cameraMarker = GameObject.Find("CameraMarker");

        //UpdateCameraPosition(cameraControls.cam.transform);
        //map.transform.localPosition = new Vector3(screenWidth * (-258.5f / screenWidth), screenHeight * (-111.5f / screenHeight)); //Calculate map position in 3d space, with 1366 x 768, optimal pos is -258.5, -111.5
    }

    public static void PassMapScale(int mapFysicalScale)
    {
        mapFScale = mapFysicalScale;
    }

    public static void UpdateCameraPosition(Transform cameraPos)
    {
        camPosX = cameraPos.localPosition.x;
        camPosY = cameraPos.localPosition.z;
        //Mapping: (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min

        if (camPosX >= mapFScale / 2)
        {
            finalX = (camPosX - mapFScale/2) * 45f / (mapFScale / 2); //(camPosX - mapFScale/2) * (36f / (mapFScale / 2));
        } else
        {
            finalX = camPosX * 45f / (mapFScale / 2) -45f; //Maybe swap out these static values for variables, to achieve scalable maps n shit
        }

        if (camPosY >= mapFScale / 2)
        {
            finalY = (camPosY - mapFScale / 2) * 45f / (mapFScale / 2);
        }
        else
        {
            finalY = camPosY * 45f / (mapFScale / 2) - 45f;
        }

        //UnityEngine.Debug.Log(cameraPos.eulerAngles);
        cameraMarker.transform.localPosition = new Vector3(finalX, finalY, 0);
        cameraMarker.transform.eulerAngles = new Vector3(0,0,-cameraPos.eulerAngles.y);
    }

    public static void LoadMap(Texture2D image)
    {
        mapImage.texture = image;
    }
}
