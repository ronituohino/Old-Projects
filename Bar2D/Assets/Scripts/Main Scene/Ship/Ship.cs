using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Ties multiple systems together
// Reports scene references to GlobalReferencesAndSettings.cs
// Handles ship-wide events
public class Ship : MonoBehaviour
{
    public List<Service> shipServices = new List<Service>();

    [SerializeField] Transform playersParent;
    [SerializeField] Transform physicsObjectsParent;
    [SerializeField] NavMeshSurface2d navMesh;

    public Transform lobby;

    private void Awake()
    {
        GlobalReferencesAndSettings.Instance.ship = this;
        GlobalReferencesAndSettings.Instance.physicsObjectsParent = physicsObjectsParent;
        GlobalReferencesAndSettings.Instance.playersParent = playersParent;
        GlobalReferencesAndSettings.Instance.navMesh = navMesh;
    }

    public Service GetRandomAvailableService()
    {
        int serviceCount = shipServices.Count;
        int startIndex = Random.Range(0, serviceCount);

        for (int i = 0; i < serviceCount; i++)
        {
            Service service = shipServices[Extensions.WrapAroundRange(i + startIndex, 0, serviceCount)];

            // Get a service that has available spots
            if (service.HasFreeSpace())
            {
                return service;
            }
        }

        return null;
    }
}