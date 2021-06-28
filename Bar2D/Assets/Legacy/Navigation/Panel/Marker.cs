using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Markers make up a path that the ship is going to travel through
public class Marker : MonoBehaviour
{
    public RectTransform rect;

    [HideInInspector]
    public Navigation navigation;
}
