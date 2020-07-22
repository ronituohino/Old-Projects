using UnityEngine;

public class modeHandler : MonoBehaviour { //Handles the mode changing between regular mode, buildmode and warmode

    public static int mode = 1; //public static mode integer

	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (mode != 1)
                {
                    mode = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (mode != 2)
                {
                    mode = 2;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (mode != 3)
                {
                    mode = 3;
                }
            }
            UpdateMode(mode);   //Update the mode if button was pressed
            
        }
        //Debug.Log(mode);
    }

    public void UpdateMode(int mode) 
    {
        if(mode == 1) //Regular mode
        {
            if (peopleControlHandler.selecting)
            {
                peopleControlHandler.Selectmode();
            }
            if (buildHandler.building) //Check if we were in build mode
            {
                buildHandler.Build(); //Turn it off
            }
        } else if(mode == 2) //Build mode
        {
            if (peopleControlHandler.selecting)
            {
                peopleControlHandler.Selectmode();
            }
            if (!buildHandler.building) //Check if we were NOT in build mode
            {
                buildHandler.Build(); //Turn it on
            }
        } else if(mode == 3) //Unit mode (war and regular troops)
        {
            if (!peopleControlHandler.selecting)
            {
                peopleControlHandler.Selectmode();
            }
            if (buildHandler.building)  //Check if we were in build mode
            {
                buildHandler.Build(); //Turn it off
            }
        }
    }
}
