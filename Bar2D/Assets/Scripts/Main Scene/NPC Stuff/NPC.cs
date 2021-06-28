using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

// TO DO MAKE THESE GUYS HAVE HANDS TOO AND ORDERS ARE DELIVERED TO THEIR HANDS

public class NPC : Character, IHoverable
{
    // When changing npc destination, update this value
    // It is then synced to other clients, making everything reallllyyy smooth
    [SyncVar(hook = nameof(SetNPCDestination))]
    Vector2 npcDestination;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] float timeToNewDestination;

    [SerializeField] float arriveDistance;
    bool arrived = false;

    float probabilityOfGoingToAnotherService = 0f;

    // These need to be set for the npc to fire Arrive on Service
    public Service seekedService;
    public Transform serviceTransform;

    DialogueBubble dialogueBubble;

    public bool timerActive = true;
    float timer = 0f;

    private void Start()
    {
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

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
            SetNewDestination();
        }

        // Check if we reach destination
        if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= arriveDistance)
        {
            if (!arrived)
            {
                arrived = true;

                if (seekedService != null)
                {
                    seekedService.Arrive(this, serviceTransform);
                }
                else
                {
                    timerActive = true;
                    arrived = false;
                }
            }
        }
    }

    public void CallToShip()
    {
        npcDestination = GlobalReferencesAndSettings.Instance.ship.lobby.position.ToV2() + Random.insideUnitCircle;
        timerActive = false;
    }

    void SetNewDestination()
    {
        timer = 0f;

        bool randomPass = Random.Range(0, 1f) < probabilityOfGoingToAnotherService;
        bool foundAvailableService = false;

        if (randomPass)
        {
            seekedService = GlobalReferencesAndSettings.Instance.ship.GetRandomAvailableService();
            if (seekedService != null)
            {
                foundAvailableService = true;

                serviceTransform = seekedService.ReserveRandomServicePoint(this);
                npcDestination = serviceTransform.position;

                probabilityOfGoingToAnotherService = 0f;
            }
        }
        
        if(!randomPass || !foundAvailableService)
        {
            npcDestination = RandomNavMeshPoint.GetRandomPointOnNavMesh();

            if(!randomPass)
            {
                probabilityOfGoingToAnotherService += GlobalReferencesAndSettings.Instance.probabilityIncreaseForGoingToService;
            }
        }

        timerActive = false;
    }

    public void ReleaseNPCFromService()
    {
        arrived = false;
        seekedService = null;
        serviceTransform = null;

        SetNewDestination();
    }

    public void SetNPCDestination(Vector2 oldValue, Vector2 newValue)
    {
        agent.SetDestination(newValue);
        arrived = false;
    }

    public void ShowDialogue(string dialogue)
    {
        dialogueBubble = InputManager.Instance.sceneCanvas.ShowBubble
                                        (
                                            transform, 
                                            Vector2.zero, 
                                            dialogue, 
                                            transform
                                        );
    }

    public void HideDialogue()
    {
        if(dialogueBubble != null)
        {
            dialogueBubble.Delete();
            dialogueBubble = null;
        }
    }

    void IHoverable.OnHoverEnter()
    {
        dialogueBubble?.SetHover(true);
    }

    void IHoverable.OnHoverStay() { }

    void IHoverable.OnHoverExit()
    {
        dialogueBubble?.SetHover(false);
    }
}