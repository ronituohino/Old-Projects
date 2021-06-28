using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkTriggerArea : MonoBehaviour
{
    [SerializeField] Bar connectedBar;
    List<GlassPhysics> glasses = new List<GlassPhysics>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        GlassPhysics gp = collision.gameObject.GetComponent<GlassPhysics>();
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

    void OnTriggerExit2D(Collider2D collision)
    {
        GlassPhysics gp = collision.gameObject.GetComponent<GlassPhysics>();
        if (gp != null)
        {
            glasses.Remove(gp);
            gp.OnFluidUpdate += CheckOrderFilled;
        }
    }
}
