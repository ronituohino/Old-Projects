using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Table
{
    public Transform table;
    public List<Chair> closeChairs;

    public Table(Transform table, List<Chair> closeChairs)
    {
        this.table = table;
        this.closeChairs = closeChairs;
    }
}
