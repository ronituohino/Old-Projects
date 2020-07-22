using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Contains the information about the socketable object, like what lights to disable and materials to swap
//Also contains the code for searching sockets to connect to

public class SocketableObject : MonoBehaviour
{
    public new string name;
    public GameObject prefab;

    [Space]

    public SocketScript socketConnectedTo;

    [Space]

    public List<GameEvent> enableSocketingEvents;
    public List<GameEvent> disableSocketingEvents;

    [Space]

    [HideInInspector]
    public bool searchForSocketConnections = false;

    SocketScript[] socketsToConnectTo;

    float[] connectionDistances;
    Transform[] positionsToCheck;
    
    PlayerControls pc;

    public void GetSockets(SocketableObject obj, PlayerControls pc)
    {
        if(socketConnectedTo == null)
        {
            socketsToConnectTo = FindObjectsOfType<SocketScript>().Where((SocketScript ss) => !ss.socketed && ss.socketables.Any((SocketScript.Socketable s) => s.socketableObject.name == obj.name)).ToArray();
            this.pc = pc;

            if(socketsToConnectTo != null)
            {
                //Nice code
                positionsToCheck = socketsToConnectTo.Select((SocketScript s) => s.socketables.First((SocketScript.Socketable so) => so.socketableObject.name == obj.name).childbject.transform).ToArray();
                connectionDistances = socketsToConnectTo.Select((SocketScript s) => s.socketables.First((SocketScript.Socketable so) => so.socketableObject.name == obj.name).connectionDistance).ToArray();

                searchForSocketConnections = true;
            }
        }
    }

    private void Update()
    {
        if (searchForSocketConnections)
        {
            for(int i = 0; i < connectionDistances.Length; i++)
            {
                float connectionDistance = connectionDistances[i];
                Transform orientation = positionsToCheck[i];

                float dist = Vector3.Distance(transform.position, orientation.position);
                if (dist < connectionDistance)
                {
                    socketsToConnectTo[i].AttachObject(this, pc);
                    searchForSocketConnections = false;

                    socketsToConnectTo = null;
                    positionsToCheck = null;
                    connectionDistances = null;
                    break;
                }
            }
        }
    }
}


