using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Handles all customers and their actions
public class CustomerManager : Singleton<CustomerManager>
{
    //Instantiated in Awake(), contains customers' ships, ordered as they are in ports
    public ShipData[] ships; 

    [Header("Spawning")]

    public MeshRenderer[] spawnPoints;
    public Transform customerParent;

    public Ship[] customerShips;
    int shipAmount;

    public GameObject[] randomCharacters;

    [HideInInspector]
    public float business;

    public AnimationCurve businessCurve; //How are the customers spread out through the day
    float dayLength;

    public int customerAmount; //Amount of customers that go by per day
    public float customerSpawnMultiplier; //Multiplies customerAmount

    [HideInInspector]
    public int actualCustomers = 0;

    [Header("Movement")]

    public float minFlightSpeed;
    public float maxFlightSpeed;

    public float superSpeed;
    public float superSpeedChance;

    [Header("Drawn Customers")]

    public float percentageOfPeopleComingIn = 0.25f; //Percentage of the ships that come in

    [Space]

    public Transform[] ports;
    int portAmount;
    [HideInInspector]
    public bool[] occupiedPorts; //Occupied ports

    public Transform[] portAccessPoints; //Vectors that tell where people can jump into their ships at the ports

    [Space]

    public MeshRenderer entrance; //Bar entrance, every npc goes here when they enter the bar

    [Space]

    public List<Bar> servingBars = new List<Bar>(); //Active bars where people leave orders

    [Header("Customer variables")]

    public List<Customer> customers = new List<Customer>(); //All npcs that are coming in
    public List<Party> parties = new List<Party>(); //Parties that have formed

    public float percentageOfPeopleWantSomething = 0.8f;

    private void Awake()
    {
        dayLength = TimeManager.Instance.dayLength;

        shipAmount = customerShips.Length;


        portAmount = ports.Length;

        occupiedPorts = new bool[portAmount];
        occupiedPorts.Populate(false);

        ships = new ShipData[portAmount];
    }

    #region Ships

    private void Update()
    {
        business = businessCurve.Evaluate((TimeManager.Instance.timeSinceGameStart % dayLength).Remap(0, dayLength, 0, 1));

        if (Random.Range(0f, 1f) < (customerAmount / (dayLength / Time.deltaTime)) * customerSpawnMultiplier * business)
        {
            SpawnShip();
        }
    }

    void SpawnShip()
    {
        if (spawnPoints.Length > 0)
        {
            //Spawn ship
            int spawn = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPoint = Extensions.RandomPointInBounds(spawnPoints[spawn].GetComponent<MeshRenderer>().bounds);

            Ship s = customerShips[Random.Range(0, shipAmount)];
            GameObject g = Instantiate(s.prefab, spawnPoint, Quaternion.identity, customerParent);

            //Navigation
            NavMeshAgent nma = g.GetComponent<NavMeshAgent>();

            nma.radius = s.shipRadius;
            nma.stoppingDistance = 0.5f;

            if (Random.Range(0f, 1f) < superSpeedChance) //Fast flight
            {
                nma.speed = superSpeed + Random.Range(-0.3f, 0.3f);
            }
            else //Regular flight
            {
                nma.speed = Random.Range(minFlightSpeed, maxFlightSpeed);
            }

            nma.agentTypeID = -334000983; //Ship

            //var count = NavMesh.GetSettingsCount();
            //var agentTypeNames = new string[count + 2];
            //for (var i = 0; i < count; i++)
            //{
            //    var id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            //    var name = NavMesh.GetSettingsNameFromID(id);
            //    agentTypeNames[i] = name;
            //    Debug.Log(id + " " + name);
            //}

            //Is customer?
            bool isCustomer = false;
            int freePort = -1;
            if (Random.Range(0f, 1f) < percentageOfPeopleComingIn)
            {
                freePort = GetFreePortPosition();

                if (freePort != -1)
                {
                    occupiedPorts[freePort] = true;
                    isCustomer = true;
                    nma.SetDestination(ports[freePort].position);
                }
            }
            else
            {
                nma.SetDestination(Extensions.RandomPointInBounds(spawnPoints[spawn == 0 ? 1 : 0].GetComponent<MeshRenderer>().bounds));
            }

            //Spawn characters inside
            List<GameObject> passengers = null;
            Transform[] seats = GetShipSeats(g.transform);

            if (isCustomer || s.showPassengersInShip)
            {
                passengers = new List<GameObject>();
                int seatAmount = seats.Length;

                if (s.onlySpecifics)
                {
                    int specificAmount = s.specificPassengers.Length;
                    for (int i = 0; i < specificAmount; i++)
                    {
                        passengers.Add(
                            Instantiate(s.specificPassengers[i], seats[i].position, Quaternion.identity, g.transform)
                            );
                        seats[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    List<int> occupiedSeats = new List<int>();

                    int specificAmount = s.specificPassengers.Length;
                    int amount = 0;

                    //Spawn specific characters
                    if (specificAmount > 0)
                    {
                        for (int i = 0; i < specificAmount; i++)
                        {
                            int seat = Random.Range(0, seatAmount);
                            while (occupiedSeats.Contains(seat))
                            {
                                seat = Random.Range(0, seatAmount);
                            }
                            occupiedSeats.Add(seat);

                            passengers.Add(
                                Instantiate(s.specificPassengers[i], seats[seat].position, Quaternion.identity, g.transform)
                                );
                            seats[seat].gameObject.SetActive(false);
                        }

                        amount = Random.Range(0, seatAmount - specificAmount);
                    }
                    else
                    {
                        amount = Random.Range(1, seatAmount);
                    }

                    //Spawn randoms
                    for (int i = 0; i < amount; i++)
                    {
                        int seat = 0;
                        while (occupiedSeats.Contains(seat))
                        {
                            seat = Random.Range(0, seatAmount);
                        }
                        occupiedSeats.Add(seat);


                        passengers.Add(
                            Instantiate(randomCharacters[Random.Range(0, randomCharacters.Length)], seats[seat].position, Quaternion.identity, g.transform)
                            );
                        seats[i].gameObject.SetActive(false);
                    }
                }

                if (!s.showPassengersInShip)
                {
                    foreach (GameObject p in passengers)
                    {
                        p.SetActive(false);
                    }
                }
            }

            //Store all data
            if (isCustomer)
            {
                ShipData ship = new ShipData(g, nma, passengers.ToArray(), s.showPassengersInShip, seats, false, spawn);
                ships[freePort] = ship;
            }

            actualCustomers++;
        }
        else
        {
            Debug.LogError("No spawnpoints set!");
        }
    }


    //Returns all ship seat transforms
    Transform[] GetShipSeats(Transform obj)
    {
        Transform t = obj.Find("Seats");

        if (t == null)
        {
            Debug.LogError("Ship " + obj.name + " doesn't have seats set!");
        }

        int len = t.childCount;
        List<Transform> seats = new List<Transform>();
        for (int i = 0; i < len; i++)
        {
            Transform s = t.GetChild(i);
            if (s.gameObject.activeSelf)
            {
                seats.Add(s);
            }
            
        }

        return seats.ToArray();
    }



    int GetFreePortPosition()
    {
        int len = ports.Length;
        for (int i = 0; i < len; i++)
        {
            if (!occupiedPorts[i])
            {
                return i;
            }
        }
        return -1;
    }

    #endregion

    //Handle npcs
    private void FixedUpdate()
    {
        //Ships and parking
        for (int i = 0; i < portAmount; i++)
        {
            ShipData ship = ships[i];
            if (ship != null
                && ship.navigation != null 
                && !ship.parked 
                && NavMeshReachedGoal(ship.navigation))
            {
                //Unload passengers
                ship.parked = true;

                //Set seats active (available)
                int len = ship.seats.Length;
                for(int s = 0; s < len; s++)
                {
                    ship.seats[s].gameObject.SetActive(true);
                }

                StartCoroutine(Unload(ship.passengers, i, ship.showPassengers));
            }
        }

        //Customers and orders
        int customerAmount = customers.Count;
        for (int i = 0; i < customerAmount; i++)
        {
            Customer c = customers[i];
            bool reachedGoal = NavMeshReachedGoal(c.navMeshAgent);

            //First check if they are leaving and have reached their ship
            if(c.leaving && reachedGoal)
            {
                StartCoroutine(Load(c, ships[c.parkedPort]));
                customers.RemoveAt(i);
                customerAmount--;
            }

            //If not then:
            //First the customer enters the bar
            if (!c.enteredBar && reachedGoal)
            {
                //Find a table and go to it
                c.enteredBar = true;

                //The customer will leave if there are no seats available for the entire party
                int len = c.party.partyMembers.Length;
                if(c.party.seats == null)
                {
                    Chair[] seats = TableManager.Instance.GetBestSeats(len);
                    if(seats == null)
                    {
                        LeaveBar(c);
                    }
                    c.party.seats = seats;
                }

                //There is a seat available, go to it!
                if (!c.leaving)
                {
                    int seatLen = c.party.seats.Length;
                    for (int s = 0; s < seatLen; s++)
                    {
                        if (!c.party.seats[s].occupied)
                        {
                            c.party.seats[s].occupied = true;
                            c.seat = c.party.seats[s];
                            break;
                        }
                    }

                    c.navMeshAgent.SetDestination(c.seat.transform.position);
                }

                
                


                //Vector3 pos = GetBarServingPoint(servingBars.Count);

                ////Find a serving point
                //if (pos != Vector3.zero) 
                //{
                //    c.navMeshAgent.SetDestination(pos);
                //}
                //else
                //{
                //    LeaveBar(c);
                //}
            }

            //Then he goes to the counter and leaves his order
            if (c.enteredBar && !c.leftOrder && reachedGoal)
            {
                //Leave order
                OrderManager.Instance.AddOrder(c.order);
                c.leftOrder = true;
            }


        }
    }

    bool NavMeshReachedGoal(NavMeshAgent agent)
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.magnitude == 0f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Moves customers from ship to port
    WaitForSeconds unloadTime = new WaitForSeconds(0.33f);
    IEnumerator Unload(GameObject[] customerObjects, int port, bool showPassengers)
    {
        int len = customerObjects.Length;

        Party party = new Party(new Customer[len], null);

        for (int i = 0; i < len; i++)
        {
            yield return unloadTime;

            //Instantiate a customer
            GameObject g = customerObjects[i];
            g.transform.position = portAccessPoints[GetPortAccessPoint(port)].transform.position;

            if (!showPassengers)
            {
                g.SetActive(true);
            }

            //Go into the bar
            NavMeshAgent nma = g.GetComponent<NavMeshAgent>();
            nma.enabled = true;
            nma.agentTypeID = 1479372276; //Person
            nma.stoppingDistance = 0.5f;
            nma.SetDestination(Extensions.RandomPointInBounds(entrance.bounds));

            //Chance that he doesn't want anything
            bool wantsSomething = (i > 0 ? Random.Range(0f, 1f) < percentageOfPeopleWantSomething : true);

            //Create a customer class
            Customer c = new Customer(g.transform, nma, false, port, false);
            party.partyMembers[i] = c;
            c.party = party;

            if (wantsSomething)
            {
                c.hasOrder = true;
                c.order = OrderManager.Instance.GenerateOrder();
            }
            else
            {
                c.hasOrder = false;
            }

            customers.Add(c);
        }

        parties.Add(party);
    }

    //Moves customers from port to ship, removes party and customer stuff if everyone is in the ship
    WaitForSeconds loadTime = new WaitForSeconds(0.33f);
    IEnumerator Load(Customer c, ShipData shipData)
    {
        yield return loadTime;

        int passengersInsideShip = 0;

        bool seated = false;
        int len = shipData.seats.Length;
        for(int i = 0; i < len; i++)
        {
            Transform s = shipData.seats[i];
            if (s.gameObject.activeSelf)
            {
                if (!seated)
                {
                    seated = true;
                    c.transform.position = s.position;
                    s.gameObject.SetActive(false);
                    passengersInsideShip++;
                }
            }
            else
            {
                //Disabled seats are taken
                passengersInsideShip++;
            }
        }

        if (!shipData.showPassengers)
        {
            c.transform.gameObject.SetActive(false);
        }

        //There are as many passengers inside the ship as there were passengers in the first place
        //Time to leave!
        if (passengersInsideShip == shipData.passengers.Length) 
        {
            parties.Remove(c.party);
            shipData.navigation.SetDestination(Extensions.RandomPointInBounds(spawnPoints[shipData.spawnInt == 0 ? 1 : 0].GetComponent<MeshRenderer>().bounds));
            ships[c.parkedPort] = null;
            occupiedPorts[c.parkedPort] = false;
        }
    }


    int GetPortAccessPoint(int port)
    {
        switch (port)
        {
            case 0:
                return Random.Range(0, 3);
            case 1:
                return Random.Range(0, 6);
            case 2:
                return Random.Range(3, 9);
            case 3:
                return Random.Range(6, 9);
            default:
                Debug.LogError("Unable to get port access point at port: " + port);
                return 0;
        }
    }

    Vector3 GetBarServingPoint(int barAmount)
    {
        int barNum = 0;

        while (barNum < barAmount)
        {
            Vector3 pos = servingBars[barNum].GetFreeServingPoint();
            if (pos != Vector3.zero)
            {
                return pos;
            }
            else
            {
                barNum++;
            }
        }

        return Vector3.zero;
    }

    void LeaveBar(Customer c)
    {
        c.navMeshAgent.SetDestination(portAccessPoints[GetPortAccessPoint(c.parkedPort)].transform.position);
        c.leaving = true;
    }
}


