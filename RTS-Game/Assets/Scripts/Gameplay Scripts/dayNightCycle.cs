using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dayNightCycle : MonoBehaviour {
    public GameObject sun;
    float time;
    
    public float dayNightSpeed;
    public float angles;

    public static bool day = true;

    void Update () {
        time += Time.deltaTime;
        //Debug.Log(sun.transform.rotation.ToEuler().x);
        if(time >= 1 / dayNightSpeed) //It's day
        {
            if(sun.transform.rotation.ToEuler().x > -0.1f) //Fix to show sunrise (a little rotation should do (Done))
            {
                day = true;
                time -= 1 / dayNightSpeed;
                sun.transform.Rotate(angles, 0, 0);
            }
            else //It's night
            {
                day = false;
                //Debug.Log("night");
                time -= 1 / dayNightSpeed;
                sun.transform.Rotate(angles * 2, 0, 0);
            }
        } 
	}
}
