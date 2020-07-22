using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class buildingSelection : MonoBehaviour {

    public static GameObject selectedObject;
    public GameObject actionCircle;
    public GameObject actionsList;
    public TextMeshProUGUI actionTip;
    string tip;

    public GameObject[] actions;

    [Range(0,200)]
    public float fadeSpeed;
    public TextMeshProUGUI circleText;

    Vector3 mouseMovement;
    Vector3 originPos;
    float diff;
    public float actionMouseMovementRange;

    static bool circleDrawn = false;

    public GraphicRaycaster gr;
    PointerEventData ped;
    List<RaycastResult> results;

    private void Start()
    {
        //gr = this.GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(null);
    }
    void Update () {
        if(modeHandler.mode == 1)
        {
            if (circleDrawn) //Fade in/out
            {
                actionTip.text = ""; //Reset tooltip text
                CheckForToolTip();
                if (actionCircle.transform.localScale.x < 2f) //Fade in
                {
                    actionCircle.transform.localScale += new Vector3(0.1f * fadeSpeed * Time.deltaTime, 0.1f * fadeSpeed * Time.deltaTime, 0.1f * fadeSpeed * Time.deltaTime);
                    actionCircle.GetComponent<Image>().color += new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);
                    circleText.color += new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime); //Fade building name
                    actionTip.color += new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime); //Fade action tip

                    for (int i = 0; i < actionsList.transform.childCount; i++) //Fade all actions
                    {
                        actionsList.transform.GetChild(i).GetComponent<Image>().color += new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);
                    }

                }
                else //Circle has been drawn, conditions to remove it are here
                {
                    //if(Input.GetAxisRaw("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0) //Calculate the distance from the origin only if the mouse was moved
                    //{
                    mouseMovement = Input.mousePosition; //Store the mouse movement

                    diff = Mathf.Sqrt(Mathf.Pow(mouseMovement.x - originPos.x, 2) + Mathf.Pow(mouseMovement.y - originPos.y, 2));
                    //}

                    if (CheckKeysDown() || Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0 || diff > actionMouseMovementRange)
                    {
                        circleDrawn = false;
                        diff = 0;
                    }
                }
            }
            else
            {
                if (actionCircle.transform.localScale.x > 0) //Fade out
                {
                    actionCircle.transform.localScale -= new Vector3(0.1f * fadeSpeed * Time.deltaTime, 0.1f * fadeSpeed * Time.deltaTime, 0.1f * fadeSpeed * Time.deltaTime);
                    actionCircle.GetComponent<Image>().color -= new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);
                    circleText.color -= new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);
                    actionTip.color -= new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);

                    for (int i = 0; i < actionsList.transform.childCount; i++)
                    {
                        actionsList.transform.GetChild(i).GetComponent<Image>().color -= new Color(0, 0, 0, 0.1f * fadeSpeed * Time.deltaTime);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500.0f)) //Raycast to mousePos, if we hit something filter it
                {
                    if (hit.collider.gameObject.name != "Terrain")
                    {
                        selectedObject = hit.collider.gameObject;
                        Filter(selectedObject); //Check if the building has actions, if so bring up the action circle
                    }
                }
            }
        }
    }

    private void Filter(GameObject obj) //Check for any actions for that building, draw a little circle on the UI.
    {
        if(obj.GetComponent<Structure>() != null) //If selected object has a structure component
        {
            if(obj.GetComponent<Structure>().owner == turnHandler.localPlayerName) //If selected object is owned by the player
            {
                if (obj.GetComponent<Structure>().hasActions) //If selected object has actions
                {
                    DrawUICircle(obj);
                }
            }
            
        }
        
    }

    public void CheckForToolTip() //Update the tooltip if we are highlighting an action
    {
        ped.position = Input.mousePosition;
        results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        if(results.Count > 0)
        {
            tip = results[0].gameObject.name;
            if(tip != "SelectionCircle") //Filter the other ui elements out
            {
                if(tip != "ActionTip")
                {
                    if(tip != "Text")
                    {
                        actionTip.text = tip;
                    }
                }
            }
        }
    }

    private void DrawUICircle(GameObject obj) //Enable the circle to be drawn
    {
        FilterActions(obj);
        originPos = Input.mousePosition; //Take mouse originpos
        if (obj.GetComponent<Structure>().beingBuilt)
        {
            circleText.text = obj.name + " (Building)"; //Set actionCircle text to building name and show that it is being built
        } else
        {
            circleText.text = obj.name; //Set actionCircle text to building name
        }
        actionCircle.transform.position = Input.mousePosition; //Set the circle position to mouse position
        circleDrawn = true;
    }

    private void FilterActions(GameObject obj) //Filter the actions for that building
    {
        foreach(Structure.Action a in obj.GetComponent<Structure>().actions) //Go through every action
        {
            //Optimize somehow maybe?
            if (a._enabled) //Set action gameobject to true
            {
                foreach(GameObject act in actions)
                {
                    if(act.name == a._name)
                    {
                        act.SetActive(true);
                    }
                }
            } else //Set action gameobject to false
            {
                foreach (GameObject act in actions)
                {
                    if (act.name == a._name)
                    {
                        act.SetActive(false);
                    }
                }
            }
        }
    }

    private bool CheckKeysDown() //All banned keys, if pressed, the circle will disappear
    {
        if (Input.GetMouseButton(3)) //Scroll press
        {
            return true;
        } else if (Input.GetMouseButtonUp(0))
        {
            return true;
        } else if (Input.GetKey(KeyCode.W))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        } else
        {
            return false;
        }
    } 
}
