using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PointOfInterest : ScriptableObject
{
    public float mapSpawnProbability;
    public Sprite[] mapIcons;

    [Space]

    public float sensorProfile; // Determines how far this is seen, negative values make this less visible
    public float triggerDistance;

    public enum TriggerBehaviour
    {
        Nothing,
        StopShip,
        DirectToCenter,
    }

    public TriggerBehaviour triggerBehaviour;

    [Space]

    public string eventName;
    public string eventArgs;
}