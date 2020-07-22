using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class GameEvent : UnityEvent<int> { }

public class EventCaller : MonoBehaviour
{
    public GameEvent events;
    public void Raise(){
        events.Invoke(0);
    }
}