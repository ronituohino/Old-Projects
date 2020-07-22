using UnityEngine;

[System.Serializable]
public class Chair
{
    public Transform transform;
    public bool occupied;

    public Chair(Transform transform, bool occupied)
    {
        this.transform = transform;
        this.occupied = occupied;
    }
}

