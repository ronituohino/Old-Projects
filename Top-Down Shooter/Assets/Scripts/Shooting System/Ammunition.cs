using UnityEngine;

[CreateAssetMenu()]
public class Ammunition : ScriptableObject
{
    public GameObject bullet;
    public float speed;

    [Range(0f,1f)]
    public float threat; //0 being a bb, 1 being a .50 cal sniper rifle anitmatter bullet
}