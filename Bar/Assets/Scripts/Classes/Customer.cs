using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Attached to every npc coming in to the bar, contains the order they wish and the port they parked their ship in
[System.Serializable]
public class Customer
{
    public Transform transform;

    public NavMeshAgent navMeshAgent;
    public bool enteredBar = false;

    public bool hasOrder = false; 
    public Order order; //if the customer doesn't have an order this is null
    public bool leftOrder = false;

    public int parkedPort;
    public bool leaving = false;

    public Chair seat = null;

    public Party party = null; //this is null if the customer is alone

    public Customer(Transform transform, NavMeshAgent navMeshAgent, bool enteredBar, int parkedPort, bool leaving)
    {
        this.transform = transform;
        this.navMeshAgent = navMeshAgent;
        this.enteredBar = enteredBar;
        this.parkedPort = parkedPort;
        this.leaving = leaving;
    }
}
