using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject switchObject;
    public PlayerControls pc;

    [Tooltip("TRUE: button works with a press, FALSE: button works with press and release")]
    public bool buttonPress = true;

    [Space]

    [Header("Disable interaction for this amount of seconds after release")]
    public float timeToWait;

    [Space]

    [Header("Current switch state")]
    public bool switchState = false;

    [Space]

    [Header("Events called no matter the switch state")]
    public GameEvent holdEvents;
    public GameEvent releaseEvents;

    [Header("Turning switch TRUE events")]
    public GameEvent trueHoldEvents;
    public GameEvent trueReleaseEvents;

    [Space]

    [Header("Turning switch FALSE events")]
    public GameEvent falseHoldEvents;
    public GameEvent falseReleaseEvents;

    [Space]

    [Header("Events to launch after timed wait")]

    public float time;
    public GameEvent timedEvents;

    

    public void HoldEvents()
    {
        holdEvents.Invoke(0);
        if (!switchState) //Switch is at off state, do true events
        {
            trueHoldEvents.Invoke(0);
        } else //The reverse
        {
            falseHoldEvents.Invoke(0);
        }

        if(buttonPress)
        {
            switchState = !switchState;
            StartCoroutine(SwitchDisable(timeToWait));
            StartCoroutine(RunTimedEvents());
        }
    }

    public void ReleaseEvents()
    {
        releaseEvents.Invoke(0);
        if (!switchState) //Switch is at off state, do true events
        {
            trueReleaseEvents.Invoke(0);
        }
        else //The reverse
        {
            falseReleaseEvents.Invoke(0);
        }

        if (!buttonPress)
        {
            switchState = !switchState;
            StartCoroutine(SwitchDisable(timeToWait));
            StartCoroutine(RunTimedEvents());
        }
    }

    //Disable the interaction for the switch
    IEnumerator SwitchDisable(float time)
    {
        switchObject.layer = 0;
        float curTime = 0;

        while(curTime < time)
        {
            curTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        switchObject.layer = (int)Mathf.Log(pc.interactableMask.value, 2);
    }

    IEnumerator RunTimedEvents()
    {
        yield return new WaitForSeconds(time);
        timedEvents.Invoke(0);
    }
}

