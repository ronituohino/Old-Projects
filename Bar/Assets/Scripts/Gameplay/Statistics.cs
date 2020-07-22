using UnityEngine;

//Tracks player statistics
public class Statistics : Singleton<Statistics>
{
    [HideInInspector]
    public float balance;

   
    public float barBeauty;

    [HideInInspector]
    public int ordersReceived = 0;
    [HideInInspector]
    public int ordersFailed;
    [HideInInspector]
    public int ordersCompleted;
}
