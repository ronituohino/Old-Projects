using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class turnHandler : MonoBehaviour {
    public Player[] players;
    public int curPlayer = 0;
    public static bool waitingForTurn = true;
    public static string localPlayerName;

    public TextMeshProUGUI turnWaitText;

    private void Start()
    {
        localPlayerName = "R";
        startSetup();
    }
    //public static bool myTurn = false;
    void startSetup() //Called on startup, sets gameobjects and players where they should be
    {
        if (players[curPlayer].playerName == localPlayerName) //Local player's turn
        {
            //waitingForTurn = false;
            //Debug.Log("ok");
            waitingForTurn = false;
            resourceHandler.loadResources();
        }
    }

    public void endTurn() //Called whenever a turn ends
    {
        waitingForTurn = true;
        turnWaitText.text = "Waiting for turn...";
        if(curPlayer == players.Length - 1) //Set the currentPlayer integer
        {
            curPlayer = 0;
        } else
        {
            curPlayer++;
        }
        
        if(players[curPlayer].playerName == localPlayerName) //Local player's turn
        {
            turnWaitText.text = "End turn";
            waitingForTurn = false;

            resourceHandler.calculateResources(); //Calculate resources gained (also count down buildTime)
        }
    }
}
