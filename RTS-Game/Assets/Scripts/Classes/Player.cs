using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName { get; set; }
    //Store resources and buildings etc...

    private void Start()
    {
        playerName = gameObject.name;
    }
}
