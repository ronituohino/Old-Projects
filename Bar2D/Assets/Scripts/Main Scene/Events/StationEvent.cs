using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StationEvent : NavigationEvent
{
    Station station = null;

    [SerializeField] Transform stationParent;

    [SerializeField] Transform stationStart;
    [SerializeField] Transform stationTarget;

    StationScript ss;

    public override void StartEvent(string eventArgs, int callerHashCode, Navigation navigation)
    {
        foreach (Station s in MapGenerator.Instance.stations)
        {
            if (s.eventArgs.Equals(eventArgs))
            {
                station = s;
                break;
            }
        }

        GameObject stObject = Instantiate(station.stationObject, stationParent);
        ss = stObject.GetComponent<StationScript>();

        int npcAdd = Random.Range(1, 3);
        for(int i = 0; i < npcAdd; i++)
        {
            GameObject g = Instantiate(ss.npc, ss.npcParent);
            g.transform.position = ss.npcSpawnPoints[Random.Range(0, ss.npcSpawnPoints.Length)].position;
            NPC n = g.GetComponent<NPC>();
            n.timerActive = false;
            ss.newNPCs.Add(n);
        }
        
        StartCoroutine(ApporachStation());
    }

    public override void UpdateEvent(Navigation navigation, float distance) { }

    // We need to check that all players are aboard before leaving
    public void BeforeLeave()
    {
        foreach(NPC n in ss.newNPCs)
        {
            n.CallToShip();
        }
    }

    public override void EndEvent()
    {
        StartCoroutine(LeaveStation());
    }

    IEnumerator ApporachStation()
    {
        int steps = Mathf.RoundToInt(1f / Time.fixedDeltaTime);

        for (int i = 0; i <= steps; i++)
        {
            stationParent.position = Vector2.Lerp(stationStart.position, stationTarget.position, (float)i / (float)steps);
            yield return GlobalReferencesAndSettings.Instance.wait;
        }

        // Rebuild navmesh when entering station
        ss.surface.ignoreFromBuild = false;
        GlobalReferencesAndSettings.Instance.navMesh.BuildNavMesh();
        RandomNavMeshPoint.UpdateTriangulation();

        foreach(NPC n in ss.newNPCs)
        {
            n.timerActive = true;
        }
    }

    IEnumerator LeaveStation()
    {
        int steps = Mathf.RoundToInt(1f / Time.fixedDeltaTime);

        for (int i = 0; i <= steps; i++)
        {
            stationParent.position = Vector2.Lerp(stationStart.position, stationTarget.position, 1f - (float)i / (float)steps);
            yield return GlobalReferencesAndSettings.Instance.wait;
        }

        ss.surface.ignoreFromBuild = true;
        GlobalReferencesAndSettings.Instance.navMesh.BuildNavMesh();
        RandomNavMeshPoint.UpdateTriangulation();
    }
}