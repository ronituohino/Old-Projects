using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains information about what objects can connect to this socket
//Also the rigidbody and joint to add to objects that connect to it
//It also contains the code for attaching and detaching objects

public class SocketScript : MonoBehaviour
{
    [Header("Socket options")]

    public GameObject socketedObject;
    public bool socketed = false;
    public bool turnedOn = true;

    [Space]

    [Header("Socketable settings")]
    public Socketable[] socketables;
    
    

    
    SocketableObject so;
    Socketable s;
    bool interacting;
    SpringJoint joint;
    int index;
    PlayerControls pc;
    Rigidbody otherRb;

    private void Start()
    {
        if (socketed && socketedObject != null)
        {
            so = socketedObject.GetComponent<SocketableObject>();
            otherRb = socketedObject.GetComponent<Rigidbody>();

            joint = socketedObject.GetComponent<SpringJoint>();
            index = GetSocketableIndex(so);
            s = socketables[index];
        }
    }

    public int GetSocketableIndex(SocketableObject obj) //Note that the object names should be the same before a ' ' character
    {
        int i = 0;
        foreach (Socketable s in socketables)
        {
            if (obj.name == s.socketableObject.name)
            {
                return i;
            }
            i++;
        }
        Debug.Log("Error fetching ConnectableIndex!");
        return 0;
    }



    //Is called when the player starts dragging the socketed object
    public void Interacting(PlayerControls pc)
    {
        this.pc = pc;
        pc.interactionForce = s.interactionForce;
        joint.breakForce = s.jointBreakForce;

        interacting = true;
    }

    //Called when interaction with the socketed object is stopped and the object is not detached
    public void StoppedInteraction()
    {
        pc.interactionForce = 0f;
        pc = null;

        joint.breakForce = socketables[index].childbject.GetComponent<SpringJoint>().breakForce;

        interacting = false;
        socketed = true;
    }



    //Called when the spring joint on the socketed object is broken
    public void DetachObject()
    {
        //Change parent to world physics objects
        socketedObject.transform.parent = pc.physicsObjects.transform;

        //Update the normal rigidbody
        Rigidbody newBody = so.prefab.GetComponent<Rigidbody>();
        otherRb.mass = newBody.mass;
        otherRb.drag = newBody.drag;
        otherRb.angularDrag = newBody.angularDrag;

        DisableSocketedObject();

        //Empty some values
        pc.socketedObject = false;
        so.socketConnectedTo = null;
        socketed = false;

        socketedObject = null;
        otherRb = null;

        pc.interactionForce = 0f;
        pc = null;

        so = null;
        joint = null;
        index = 0;

        interacting = false;
    }

    //Called when a socketable object is brought close and it is attached to the socket
    public void AttachObject(SocketableObject socketableObject, PlayerControls pc)
    {
        so = socketableObject;
        this.pc = pc;

        //Change the parent
        so.transform.parent = transform;

        //Socket the object
        socketedObject = so.gameObject;
        socketableObject.socketConnectedTo = this;
        socketed = true;

        //Get the index
        index = GetSocketableIndex(socketableObject);

        //Apply new rigidbody and springjoint and transform
        GameObject childBase = socketables[index].childbject;



        so.gameObject.transform.position = childBase.transform.position;
        so.gameObject.transform.rotation = childBase.transform.rotation;



        Rigidbody newBody = childBase.GetComponent<Rigidbody>();
        otherRb = so.GetComponent<Rigidbody>();

        otherRb.mass = newBody.mass;
        otherRb.drag = newBody.drag;
        otherRb.angularDrag = newBody.angularDrag;

        otherRb.interpolation = RigidbodyInterpolation.Interpolate;
        otherRb.collisionDetectionMode = CollisionDetectionMode.Discrete;



        SpringJoint newJoint = childBase.GetComponent<SpringJoint>();
        joint = so.gameObject.AddComponent<SpringJoint>();

        joint.connectedBody = newJoint.connectedBody;
        joint.anchor = newJoint.anchor;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = newJoint.connectedAnchor;
        joint.spring = newJoint.spring;
        joint.damper = newJoint.damper;
        joint.minDistance = newJoint.minDistance;
        joint.maxDistance = newJoint.maxDistance;
        joint.tolerance = newJoint.tolerance;
        joint.breakForce = newJoint.breakForce;
        joint.breakTorque = newJoint.breakTorque;
        joint.massScale = newJoint.massScale;
        joint.connectedMassScale = newJoint.connectedMassScale;



        //Cut the interaction in playercontrols
        pc.CutInteraction();

        //Remove the collision adjuster
        Destroy(so.GetComponent<CollisionAdjuster>());

        //Enable the socketed object
        EnableSocketedObject();
    }



    //Enables lights / materials on the socketed object
    public void TurnSocketOn()
    {
        turnedOn = true;
        EnableSocketedObject();
    }

    //Disables lights / materials on the socketed object
    public void TurnSocketOff()
    {
        turnedOn = false;
        DisableSocketedObject();
    }



    void EnableSocketedObject()
    {
        if (socketed && turnedOn)
        {
            foreach(int i in s.fireSocketingEvents)
            {
                so.enableSocketingEvents[i].Invoke(0);
            }
        }
    }

    void DisableSocketedObject()
    {
        if (socketed)
        {
            foreach (int i in s.fireSocketingEvents)
            {
                so.disableSocketingEvents[i].Invoke(0);
            }
        }
    }




    //See if the socketed object is disconnected
    private void Update()
    {
        if (interacting)
        {
            if (joint == null)
            {
                DetachObject();
            }
        }
    }

    //List of objects that can be connected to this socket along with some variables
    [System.Serializable]
    public class Socketable
    {
        [Tooltip("Object under this in the hierarchy, contains the rigidbody and springjoint for a socketed object")]
        public GameObject childbject;

        [Space]

        public float interactionForce;
        public float jointBreakForce;

        [Space]

        public float connectionDistance;

        [Space]

        [Tooltip("Prefabs socketable object script")]
        public SocketableObject socketableObject;

        [Space]

        [Tooltip("Which of the socketing event lists should be fired? (On the socketableObject)")]
        public List<int> fireSocketingEvents;
    }
}
