using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to GameObjects that serve as bars, contains positions for customers to leave orders
public class Bar : MonoBehaviour
{
    public Transform[] servingPoints;
    int len;

    public bool[] occupied;

    private void Awake()
    {
        len = servingPoints.Length;
        occupied = new bool[len];
        occupied.Populate(false);
    }

    public Vector3 GetFreeServingPoint()
    {
        int position = 0;
        while (position < len && occupied[position])
        {
            position++;
        }

        if(position < len)
        {
            occupied[position] = true;
            return servingPoints[position].position;
        } else
        {
            return Vector3.zero;
        }
    }
}
