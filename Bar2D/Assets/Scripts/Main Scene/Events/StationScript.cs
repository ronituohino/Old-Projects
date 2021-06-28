using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StationScript : MonoBehaviour
{
    public NavMeshModifier surface;

    public List<NPC> newNPCs = new List<NPC>();
    public GameObject npc;
    public Transform npcParent;

    public Transform[] npcSpawnPoints;
}