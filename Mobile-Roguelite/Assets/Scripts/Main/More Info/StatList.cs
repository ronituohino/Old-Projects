using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatList : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public string name;

        public RectTransform textRect;
        public TextMeshProUGUI textMesh;

        public bool hidden;
    }

    [SerializeField] Stat[] stats;

    private void OnEnable()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        float pos = 1f;

        foreach(Stat s in stats)
        {
            s.textRect.gameObject.SetActive(!s.hidden);

            if(!s.hidden)
            {
                s.textRect.offsetMin = new Vector2(s.textRect.offsetMin.x, pos);
                s.textRect.offsetMax = new Vector2(s.textRect.offsetMax.x, pos);

                pos -= 0.1f;
            }
        }
    }

    public void SetText(string statName, string text)
    {
        foreach(Stat s in stats)
        {
            if(s.name == statName)
            {
                s.textMesh.text = text;

                break;
            }
        }
    }
}
