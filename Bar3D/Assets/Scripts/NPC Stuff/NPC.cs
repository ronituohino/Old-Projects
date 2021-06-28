using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class NPC : MirrorHierarchyNetworkBehaviour
{
    // When changing npc destination, update this value
    // It is then synced to other clients, making everything reallllyyy smooth
    [SyncVar(hook = nameof(SetNPCDestination))]
    Vector3 npcDestination;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] float timeToNewDestination;

    [SerializeField] float arriveDistance;
    bool arrived = false;

    [SerializeField] Ship ship;

    // These need to be set for the npc to fire Arrive on Service
    public Service seekedService;
    public Transform serviceTransform;

    bool timerActive = true;
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
        }

        // New destination
        if (timer >= timeToNewDestination)
        {
            timer = 0f;
            if (ship != null)
            {
                seekedService = ship.GetRandomAvailableService();
                if (seekedService != null)
                {
                    serviceTransform = seekedService.ReserveRandomServicePoint(this);
                    npcDestination = serviceTransform.position;

                    timerActive = false;
                }
            }
        }

        // Check if we reach destination
        if (serviceTransform != null && seekedService != null)
        {
            if (Vector3.Distance(transform.position, serviceTransform.position) < 1 && agent.remainingDistance <= arriveDistance)
            {
                if (!arrived)
                {
                    seekedService.Arrive(this, serviceTransform);
                    arrived = true;
                }
            }
        }
    }

    public void ReleaseNPCFromService()
    {
        timerActive = true;
        arrived = false;
    }

    public void SetNPCDestination(Vector3 oldValue, Vector3 newValue)
    {
        agent.SetDestination(newValue);
        arrived = false;
    }

    // Dialogue
    public void ShowDialogue(string dialogue)
    {
        InputManager.Instance.sceneCanvas.ShowDialogue(transform, dialogue);
    }
}