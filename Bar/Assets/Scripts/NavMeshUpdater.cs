using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Updates the bar NavMesh when large physics objects are moved
public class NavMeshUpdater : Singleton<NavMeshUpdater>
{
    public NavMeshSurface surface;

    public float updateInterval;
    float timer = 0f;

    public List<Rigidbody> bodiesAffected = new List<Rigidbody>();

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > updateInterval)
        {
            UpdateNavMesh();
        }
    }

    public void UpdateNavMesh()
    {
        int count = bodiesAffected.Count;
        for(int i = 0; i < count; i++)
        {
            Rigidbody r = bodiesAffected[i];
            if (r.IsSleeping())
            {
                NavMeshUpdateTracker tracker = r.GetComponent<NavMeshUpdateTracker>();
                if (tracker)
                {
                    Destroy(tracker);
                }

                bodiesAffected.RemoveAt(i);

                i--;
                count--;
            }
        }

        if (count > 0)
        {
            surface.BuildNavMesh();
        }
    }
}
