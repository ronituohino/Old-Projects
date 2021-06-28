using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NavigationHoverCheck : MonoBehaviour, IHoverable
{
    [SerializeField] Navigation navigation;

    void IHoverable.OnHoverEnter()
    {
        navigation.hovered = true;
    }

    void IHoverable.OnHoverExit()
    {
        navigation.hovered = false;
    }

    void IHoverable.OnHoverStay()
    {
        
    }
}