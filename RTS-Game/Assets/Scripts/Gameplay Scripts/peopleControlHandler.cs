using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class peopleControlHandler : MonoBehaviour {

    public static bool selecting;
    public List<GameObject> selectedObjects = new List<GameObject>();

    

    public static void Selectmode()
    {
        if (selecting)
        {
            selecting = false;
        } else
        {
            
            selecting = true;
        }
    }

    /*public static void BuildNavMesh()
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

        for (int i = 0; i < terrains.Length; i++)
        {
            NavMeshBuildSource src = new NavMeshBuildSource();
            GameObject terrainObj = terrains[i].gameObject;
            src.sourceObject = terrainObj;
            src.transform = terrainObj.transform.localToWorldMatrix;
            src.shape = NavMeshBuildSourceShape.Terrain;
            src.size = new Vector3(200.0f, 0.1f, 200.0f);
            
            sources.Add(src);
        }
        NavMeshBuildSettings setting = new NavMeshBuildSettings();
        
        
        //Debug.Log("yeabio");
        //UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        //NavMeshBuilder.BuildNavMeshData(setting, NavMeshBuilder.CollectSources(), new Bounds(), new Vector3(), Quaternion.identity);

    }*/

    // Update is called once per frame
    void Update()
    {
        if (selecting)
        {
            //Debug.Log("yea");
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //Debug.Log("firing");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500.0f, 1 << LayerMask.NameToLayer("Unit")))  //Raycast and see if we hit something
                {

                    //Check whether we hit a person, then add it to the selected people list
                    //Debug.Log(hit.collider.gameObject.name);
                    selectedObjects.Add(hit.collider.gameObject);

                }
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500.0f)) //Raycast and see if we hit something
                {
                    NavMeshAgent nav;
                    if(hit.collider.gameObject.name == "terrainPart(Clone)")
                    {
                        for(int i = 0; i < selectedObjects.Count; i++)
                        {
                            nav = selectedObjects[i].GetComponent<NavMeshAgent>();

                            //nav.baseOffset = -0.1f;
                            //nav.height = 0f;

                            nav.SetDestination(hit.point);
                        }
                    }
                }
            }
        }
    }
}
