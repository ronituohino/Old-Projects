using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks for objects that contain RoomObject.cs attached to them
//Updates bar beauty in Statistics.cs
public class RoomVolume : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> containedObjects = new List<GameObject>();
    List<GameObject> checkedObjects = new List<GameObject>();

    public float checkInterval = 3f;

    bool checkContainedObjects = true;
    float time = 0f;

    private void Update()
    {
        time += Time.deltaTime;
        if(time > checkInterval)
        {
            checkedObjects.Clear();
            checkContainedObjects = true;
            time = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkContainedObjects)
        {
            //Add any new objects
            GameObject g = other.gameObject;
            if (!containedObjects.Contains(g))
            {
                containedObjects.Add(g);
            }
            else if(checkedObjects.Count > 0 && checkedObjects[0] == g) //If the object is the same as the first object, we have checked all objects
            {
                checkContainedObjects = false;
            }



            //Remove any objects that aren't in the volume anymore
            if (checkContainedObjects)
            {
                checkedObjects.Add(g);
            }
            else
            {
                //We have looped through all objects
                int len = containedObjects.Count;

                for (int i = 0; i < len; i++)
                {
                    if (!checkedObjects.Contains(containedObjects[i]))
                    {
                        containedObjects.RemoveAt(i);
                        len--;
                        i--;
                    }
                }

                //Update the beauty amount
                float total = 0;
                foreach(GameObject obj in containedObjects)
                {
                    RoomObject r = obj.GetComponent<RoomObject>();
                    if (r)
                    {
                        total += r.beautyEffect;
                    }
                }
                Statistics.Instance.barBeauty = total;
            }
        }
    }
}
