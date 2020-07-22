using System.Collections;
using UnityEngine;

public class buildHandler : MonoBehaviour {

    public GameObject[] buildings; //List of all the game's buildings
    public int s_BuildingInt; //Selected building number
    GameObject s_Building; //Selected building gameobject

    public Material b_mat; //Building material (r/g)
    private Material[] ba_mat;

    public Mesh buildingMesh;
    private static bool enoughResources;

    private Vector3 w_mousePos;
    public static bool building = false;

    public GameObject markerList;
    public GameObject builtBuildings;
    static GameObject marker_w;
    public static Vector3 b_rotation = new Vector3(0,0,0);

    //public GameObject placableIndicator;
    public static bool exists = false;
    //bool hasBeenFound = false;

    bool buildWaiting = false; //Building booleans
    public static bool isColliding = false;
    bool onGround = false;

	public static void Build()
    {
        if (building) //Going out of build mode
        {
            building = false;
            b_rotation = new Vector3(0, 0, 0); //Reset object rotation for convinience
            if (exists)
            {
                Destroy(marker_w.gameObject, 0.01f);
                exists = false;
            }
        } else //Going into build mode
        {
            building = true;
        }
    }

    private void Update()
    {
        if (!turnHandler.waitingForTurn)
        {
            if (building)   //Building stuff
            {
                if (Input.mousePresent)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 500.0f))  //Raycast and see if we hit something
                    {
                        w_mousePos = hit.point;
                        s_Building = buildings[s_BuildingInt - 1];

                        if (!exists)    //Make the marker if it doesn't exist yet
                        {
                            GameObject marker = Instantiate(s_Building, w_mousePos, s_Building.transform.rotation, markerList.transform); //Local marker object
                                                                                                                                          //marker.GetComponent<BoxCollider>().enabled = false;
                            b_rotation = marker.transform.rotation.eulerAngles;
                            //Debug.Log(b_rotation);
                            marker.layer = 2;   //Set the marker to ignore raycasts
                            marker.AddComponent<collisionDetection>();
                            marker.AddComponent<Rigidbody>();
                            marker.GetComponent<Rigidbody>().useGravity = false;

                            try
                            {
                                ba_mat = marker.GetComponent<Renderer>().sharedMaterials; //Set the marker material
                                for (int i = 0; i < ba_mat.Length; i++)
                                {
                                    ba_mat[i] = b_mat;
                                }
                                marker.GetComponent<Renderer>().sharedMaterials = ba_mat;
                            }
                            catch (MissingComponentException)
                            {
                                //Debug.Log("We got problems...");
                            }

                            try
                            {
                                if (marker.GetComponent<Transform>().childCount > 0)
                                {
                                    for (int i = 0; i < marker.GetComponentsInChildren<Renderer>().Length; i++)
                                    {
                                        ba_mat = marker.transform.GetChild(i).GetComponent<Renderer>().sharedMaterials; //Set the marker child component materials
                                        for (int p = 0; p < ba_mat.Length; p++)
                                        {
                                            ba_mat[p] = b_mat;
                                        }
                                        marker.transform.GetChild(i).GetComponent<Renderer>().sharedMaterials = ba_mat;
                                    }
                                }
                            }
                            catch (MissingComponentException)
                            {
                                //Debug.Log("We got problems...");
                            }


                            exists = true;
                        }

                        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.anyKey)
                        {
                            //Debug.Log(w_mousePos);
                            marker_w = GameObject.Find(s_Building.gameObject.name + "(Clone)"); //Set the marker position !!!OPTIMIZE FIND (if possible)!!!
                            marker_w.transform.position = w_mousePos;
                            marker_w.transform.rotation = Quaternion.Euler(b_rotation);

                            if (HasEnoughResources(marker_w.GetComponent<Structure>())) //Check if the player has enough resources, maybe optimize somehow?
                            {
                                enoughResources = true;
                            }
                            else
                            {
                                enoughResources = false;
                            }
                        }
                        if (hit.collider.name == "terrainPart(Clone)") //If raycasthit is on terrain
                        {
                            onGround = true;
                        }
                        else
                        {
                            onGround = false;
                        }

                        if (!buildWaiting) //Check if we can build here
                        {
                            if (onGround) //Is the building on the ground
                            {
                                if (!isColliding) //Is the building colliding with anything
                                {
                                    b_mat.color = Color.green;
                                    if (enoughResources) //Check if player has enough resources
                                    {
                                        if (Input.GetMouseButton(0))    //Build something
                                        {
                                            DeductResources(marker_w.GetComponent<Structure>());
                                            GameObject obj = Instantiate(s_Building, w_mousePos, marker_w.transform.rotation, builtBuildings.transform) as GameObject; //Instatiate the object
                                            obj.GetComponent<Structure>().owner = turnHandler.localPlayerName;
                                            if (obj.GetComponent<Structure>().buildTime > 0) //If buildTime isn't instant then start counting down turns
                                            {
                                                obj.GetComponent<MeshFilter>().sharedMesh = buildingMesh;
                                                obj.GetComponent<Structure>().beingBuilt = true;
                                            }

                                            obj.name = s_Building.name;

                                            buildWaiting = true;
                                            StartCoroutine(buildWait());
                                        }
                                    } else
                                    {
                                        BuildingError("Not enough resources!");
                                    }
                                    
                                }
                                else
                                {
                                    BuildingError("Colliding with something!");
                                }
                            }
                            else
                            {
                                BuildingError("Not on the ground!");
                            }
                        }
                        else
                        {
                            BuildingError("Wait!");
                        }
                    }
                    else //Cursor not in gameboard
                    {
                        try
                        {
                            Destroy(GameObject.Find(s_Building.gameObject.name + "(Clone)"), 0.001f);  //!!!OPTIMIZE FIND (if possible)!!!
                            exists = false;
                        }
                        catch (System.NullReferenceException)
                        {

                        }
                    }
                }
            }
        } 
    }

    IEnumerator buildWait() //Build wait timer
    {
        yield return new WaitForSeconds(0.2f);
        buildWaiting = false;
    }

    private void BuildingError(string error)
    {
        b_mat.color = Color.red;
        Debug.Log(error);
    }

    public static void RotateBuilding(float rotation)
    {
        //Debug.Log("Rotating");
        b_rotation = new Vector3(b_rotation.x, b_rotation.y - rotation, b_rotation.z);
    }

    private static bool HasEnoughResources(Structure obj)
    {
        for(int i = 0; i < obj.resourceCostType.Length; i++) //Go through each resource type
        {
            string resourceName = obj.resourceCostType[i].resource;
            int rAmount = obj.resourceCostType[i].amount;

            if (resourceName == "Gold")
            {
                if(resourceHandler.gold < rAmount)
                {
                    return false;
                }
            }
            if(resourceName == "Wood")
            {
                if (resourceHandler.wood < rAmount)
                {
                    return false;
                }
            }
            if (resourceName == "Stone")
            {
                if (resourceHandler.stone < rAmount)
                {
                    return false;
                }
            }
        }

        return true; //The cycle has checked everything and has decided that we have enough resources
    }

    public static void DeductResources(Structure obj)
    {
        for (int i = 0; i < obj.resourceCostType.Length; i++) //Go through each resource type
        {
            string resourceName = obj.resourceCostType[i].resource;
            int rAmount = obj.resourceCostType[i].amount;

            if (resourceName == "Gold")
            {
                resourceHandler.gold -= rAmount;
            }
            if (resourceName == "Wood")
            {
                resourceHandler.wood -= rAmount;
            }
            if (resourceName == "Stone")
            {
                resourceHandler.stone -= rAmount;
            }
        }
        resourceHandler.UpdateResources();
    }
}
