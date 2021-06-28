using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : NetworkBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] Transform ballParent;

    [SerializeField] GameObject npc;
    [SerializeField] Transform npcParent;

    private void Update()
    {
        if(Keyboard.current.gKey.isPressed)
        {
            SpawnBall();
        }
        if (Keyboard.current.hKey.isPressed)
        {
            SpawnNPC();
        }
    }

    public void SpawnBall()
    {
        print("Ball");
        GameObject g = Instantiate(ball, new Vector3(Random.Range(-14f, 14f), 1.5f, Random.Range(-14f, 14f)), Quaternion.identity, ballParent);
        NetworkServer.Spawn(g);
    }

    public void SpawnNPC()
    {
        print("NPC");
        GameObject g = Instantiate(npc, new Vector3(Random.Range(-14f, 14f), 1.5f, Random.Range(-14f, 14f)), Quaternion.identity, npcParent);
        NetworkServer.Spawn(g);
    }
}
