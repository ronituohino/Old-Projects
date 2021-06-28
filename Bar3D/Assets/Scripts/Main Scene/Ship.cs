using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ties multiple systems together
public class Ship : MonoBehaviour
{
    public List<Service> shipServices = new List<Service>();

    public Service GetRandomAvailableService()
    {
        int serviceCount = shipServices.Count;
        int startIndex = Random.Range(0, serviceCount);

        for (int i = 0; i < serviceCount; i++)
        {
            Service service = shipServices[Extensions.WrapAroundRange(i, startIndex, 0, serviceCount)];

            // Get a service that has available spots
            if (service.HasFreeSpace())
            {
                return service;
            }
        }

        return null;
    }
}