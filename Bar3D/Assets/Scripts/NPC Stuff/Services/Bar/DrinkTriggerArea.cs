using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkTriggerArea : MonoBehaviour
{
    [SerializeField] Bar connectedBar;
    List<GlassPhysics> glasses = new List<GlassPhysics>();

    void OnTriggerEnter(Collider col)
    {
        GlassPhysics gp = col.gameObject.GetComponent<GlassPhysics>();
        if(gp != null)
        {
            glasses.Add(gp);
            CheckOrderFilled();
            gp.OnFluidUpdate += CheckOrderFilled;
        }
    }

    void CheckOrderFilled()
    {
        connectedBar.CheckIfOrderFilled(this, glasses);
    }

    void OnTriggerExit(Collider col)
    {
        GlassPhysics gp = col.gameObject.GetComponent<GlassPhysics>();
        if (gp != null)
        {
            glasses.Remove(gp);
            gp.OnFluidUpdate += CheckOrderFilled;
        }
    }
}
