using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class UINavigationHandler : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] float smoothTime;

    Vector2 targetAnchors = new Vector2(0, 1);
    Vector2 vel;

    public UINavigationButton.MenuToNavigate currentMenu;

    public void Left()
    {
        if(GameManager.Instance.canMove)
        {
            targetAnchors = new Vector2(1, 2);
            PartyManager.Instance.Open();

            currentMenu = UINavigationButton.MenuToNavigate.Left;
        }
    }

    public void Center()
    {
        targetAnchors = new Vector2(0, 1);
        if(PartyManager.Instance.opened)
        {
            PartyManager.Instance.Close();
        }

        currentMenu = UINavigationButton.MenuToNavigate.Center;
    }

    public void Right()
    {
        if(GameManager.Instance.canMove)
        {
            targetAnchors = new Vector2(-1, 0);
            if (PartyManager.Instance.opened)
            {
                PartyManager.Instance.Close();
            }

            currentMenu = UINavigationButton.MenuToNavigate.Right;
        }
    }

    private void Update()
    {
        Vector2 anchors = Vector2.SmoothDamp(new Vector2(rect.anchorMin.x, rect.anchorMax.x), targetAnchors, ref vel, smoothTime);
        rect.anchorMin = new Vector2(anchors.x, 0);
        rect.anchorMax = new Vector2(anchors.y, 1);

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}