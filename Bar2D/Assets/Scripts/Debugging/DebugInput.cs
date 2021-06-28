using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.gKey.wasPressedThisFrame)
        {
            StartCoroutine(GlobalReferencesAndSettings.Instance.moneyManager.SpawnCoins(3, transform.position));
        }
    }
}
