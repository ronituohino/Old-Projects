using System.Collections;
using UnityEngine;

public class MapSwiping : Interactable
{
    bool interrupted = false;
    Vector2 total = Vector2.zero;

    [SerializeField] UINavigationHandler handler;

    public override void Enter(int fingerId)
    {
        interrupted = false;
        total = Vector2.zero;
    }

    public override void  Stay(Vector2 delta)
    {
        total += delta;
    }

    public override void Complete()
    {
        if(!interrupted && handler.currentMenu == UINavigationButton.MenuToNavigate.Center)
        {
            // Check if valid swipe
            if (total.magnitude.IsDrag())
            {
                float angle = Vector2.SignedAngle(Vector2.up, total);

                switch(angle)
                {
                    case float val when val <= 45 && val > -45f:
                        MapManager.Instance.MoveParty(0);
                        break;
                    case float val when val <= 135f && val > 45f:
                        MapManager.Instance.MoveParty(1);
                        break;
                    case float val when val <= -135f || val > 135f:
                        MapManager.Instance.MoveParty(2);
                        break;
                    case float val when val <= -45f && val > -135f:
                        MapManager.Instance.MoveParty(3);
                        break;
                }
            }
        }
    }

    public override void Interrupt()
    {
        interrupted = true;
    }
}