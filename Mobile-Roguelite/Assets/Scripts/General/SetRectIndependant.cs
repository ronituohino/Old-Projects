using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRectIndependant : MonoBehaviour
{
    [SerializeField] RectTransform rect;

    void Start()
    {
        Vector2 pos = rect.localPosition;
        Vector2 dim = rect.rect.size;

        Vector2 v = new Vector2(0.5f, 0.5f);
        rect.anchorMin = v;
        rect.anchorMax = v;
        rect.pivot = v;

        rect.localPosition = pos;
        rect.sizeDelta = dim;
    }
}
