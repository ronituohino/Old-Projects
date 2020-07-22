using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPop : MonoBehaviour
{
    public GameObject terrainObject;

    public AnimationCurve curve;
    public float speed;

    public bool showIsland;
    float trans = 0f;
    float height = -25f;

    // Update is called once per frame
    void Update()
    {
        if(showIsland && trans < 1f){
            trans += speed * Time.deltaTime;
            height = curve.Evaluate(trans).Map(0,1,-25,0);
        } else if(!showIsland && trans > 0){
            trans -= speed * Time.deltaTime;
            height = curve.Evaluate(trans).Map(0,1,-25,0);
        }
        terrainObject.transform.position = new Vector3(terrainObject.transform.position.x, height, terrainObject.transform.position.z);
    }
}
