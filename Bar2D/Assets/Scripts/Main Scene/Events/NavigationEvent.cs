using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationEvent : MonoBehaviour
{
    public string eventName;

    internal bool active = false;

    // Used to identify which points have called this evetn already
    internal List<int> callerHashCodes = new List<int>();

    public abstract void StartEvent(string eventArgs, int callerHashCode, Navigation navigation);
    public abstract void UpdateEvent(Navigation navigation, float distance);
    public abstract void EndEvent();
}