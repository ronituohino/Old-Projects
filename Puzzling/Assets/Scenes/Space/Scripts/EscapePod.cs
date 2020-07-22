using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapePod : MonoBehaviour
{
    public GameEvent launchEvents;

    public float timeToSceneChange;

    public void Launch(){
        launchEvents.Invoke(0);
        StartCoroutine(StartWait());
    }
    
    IEnumerator StartWait(){
        yield return new WaitForSecondsRealtime(timeToSceneChange);
        SceneManager.LoadScene("Island");
    }
}
