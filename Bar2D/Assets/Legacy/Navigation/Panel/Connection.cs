using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public RectTransform rect;

    [HideInInspector]
    public Navigation navigationPanel;

    [HideInInspector]
    public RectTransform a;
    [HideInInspector]
    public RectTransform b;

    public void UpdateConnection()
    {
        Vector2 toVec = (b.anchoredPosition - a.anchoredPosition);
        float distance = toVec.magnitude;

        rect.sizeDelta = new Vector2(distance, 1f);
        rect.anchoredPosition = a.anchoredPosition;
        rect.rotation = Quaternion.Euler(0, 0, Extensions.LookAt(a.anchoredPosition, b.anchoredPosition) + 90f);
    }
}