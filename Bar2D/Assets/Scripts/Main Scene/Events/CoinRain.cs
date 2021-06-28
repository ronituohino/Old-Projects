using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRain : NavigationEvent
{
    [SerializeField] GameObject coin;
    [SerializeField] float length;
    float timer = 0f;

    public override void StartEvent(string args, int callerHashCode, Navigation navigation)
    {
        if(!callerHashCodes.Contains(callerHashCode))
        {
            callerHashCodes.Add(callerHashCode);
            active = true;
        }
    }

    public override void UpdateEvent(Navigation navigation, float distance)
    {
        if(active)
        {
            Instantiate(coin, new Vector3(Random.Range(-3f, 3f), 0f), Quaternion.identity, transform);
            timer += Time.deltaTime;

            if (timer > length)
            {
                EndEvent();
            }
        }
    }

    public override void EndEvent()
    {
        active = false;
    }
}
