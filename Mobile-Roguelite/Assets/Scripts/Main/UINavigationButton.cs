using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class UINavigationButton : Interactable
{
    public enum MenuToNavigate
    {
        Left,
        Center,
        Right,
    }

    [SerializeField] MenuToNavigate menuToNavigate;
    [SerializeField] UINavigationHandler handler;

    public override void Enter(int fingerId)
    {

    }

    public override void Stay(Vector2 delta)
    {

    }

    public override void Complete()
    {
        switch (menuToNavigate)
        {
            case MenuToNavigate.Left:
                handler.Left();
                break;
            case MenuToNavigate.Center:
                handler.Center();
                break;
            case MenuToNavigate.Right:
                handler.Right();
                break;
        }
    }

    public override void Interrupt()
    {

    }
}