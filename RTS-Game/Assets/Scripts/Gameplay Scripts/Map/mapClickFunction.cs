using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mapClickFunction : MonoBehaviour, IDragHandler, IPointerDownHandler {

    public GameObject cam;
    public GameObject controlObject;
    float finalScale;

    //private void Start()
    //{
        //controlObject = GameObject.Find("ControlObject");
        //cam = GameObject.Find("MainCamera");
    //}

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        finalScale = mapGeneration.mapFSize;
        //Debug.Log(finalScale);
        //Debug.Log(finalScale * (eventData.position.y / 100f));
        controlObject.transform.localPosition = new Vector3(finalScale * (eventData.position.x / 100f), controlObject.transform.position.y, finalScale * (eventData.position.y / 100f));
        cam.transform.localPosition = new Vector3(finalScale * (eventData.position.x / 100f), cam.transform.position.y, finalScale * (eventData.position.y / 100f));
        mapScripts.UpdateCameraPosition(cam.transform);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        finalScale = mapGeneration.mapFSize;
        //Debug.Log(finalScale);
        //Debug.Log(finalScale * (eventData.position.y / 100f));
        controlObject.transform.localPosition = new Vector3(finalScale * (eventData.position.x / 100f), controlObject.transform.position.y, finalScale * (eventData.position.y / 100f));
        cam.transform.localPosition = new Vector3(finalScale * (eventData.position.x / 100f), cam.transform.position.y, finalScale * (eventData.position.y / 100f));
        mapScripts.UpdateCameraPosition(cam.transform);
    }
}
