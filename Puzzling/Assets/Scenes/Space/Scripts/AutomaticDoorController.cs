using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorController : MonoBehaviour
{
    public GameObject player;

    public float maxDistance;

    public Vector4 areaBounds = new Vector4(-4, 3, -3, 3);

    public Animator[] automaticDoors;

    Vector3 playerPos;
    Vector3 playerRotation;

    Vector3 deductionVector;

    void Update()
    {
        playerPos = player.transform.position;
        playerRotation = player.transform.eulerAngles;

        foreach(Animator a in automaticDoors)
        {
            bool open = false;

            if(CheckIfClose(a.transform.position, playerPos))
            {
                if(CheckIfInfrontOfDoor(a, deductionVector))
                {
                    open = true;
                }
            }

            if (open)
            {
                a.SetBool("State", true);
            } else
            {
                a.SetBool("State", false);
            }
        }
    }

    //Checks if the player is close to the door
    bool CheckIfClose(Vector3 doorPos, Vector3 playerPos)
    {
        deductionVector = doorPos - playerPos;
        if ((deductionVector).magnitude < maxDistance)
        {
            return true;
        } else
        {
            return false;
        }
    }

    //Checks if the player is infront of the door
    bool CheckIfInfrontOfDoor(Animator door, Vector3 doorToPlayerVector)
    {
        Vector3 relations = Vector3.Cross(Vector3.up, doorToPlayerVector);

        //Sideways distance
        if((relations.z > areaBounds.x && relations.z < areaBounds.y) && (relations.x > areaBounds.z && relations.x < areaBounds.w))
        {
            return true;
        } else
        {
            return false;
        }
    }
}
