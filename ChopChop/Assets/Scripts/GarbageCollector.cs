using UnityEngine;
using System;
using System.Collections;

public class GarbageCollector : Singleton<GarbageCollector>
{
    public void CollectGarbage()
    {
        GC.Collect();
    }

    public void CollectInOneFrame()
    {
        Instance.StartCoroutine(Delay());
    }

    WaitForSeconds wait = new WaitForSeconds(0.017f);

    IEnumerator Delay()
    {
        yield return wait;
        Instance.CollectGarbage();
    }
}
