using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabInfo : MonoBehaviour, IHoverable, ILeftClickable
{
    public RectTransform rect;
    public TextMeshProUGUI headerText;

    public Website currentWebsite { get; set; }
    public float percentagePageScrolled { get; set; } = 0f;

    public List<Website> siteHistory { get; set; } = new List<Website>();

    public int siteHistoryCount { get; set; } = 0;
    
    public int historyPointer { get; set; } = 0;

    [Space]

    [SerializeField] Color activeColor;
    [SerializeField] Color hoveredColor;
    [SerializeField] Color unhoveredColor;

    [SerializeField] float lerpMultiplier;

    [SerializeField] List<Image> targetImages = new List<Image>();

    public bool active = false;
    bool hovered = false;

    void ILeftClickable.OnClickHold() { }

    void ILeftClickable.OnClickPress() { }

    void ILeftClickable.OnClickRelease()
    {
        ComputerBrowser.Instance.SwitchTab(this);
    }

    void IHoverable.OnHoverEnter() 
    {
        hovered = true;
    }

    void IHoverable.OnHoverExit()
    {
        hovered = false;
    }

    void IHoverable.OnHoverStay() { }

    void Update()
    {
        if(active)
        {
            foreach (Image image in targetImages)
            {
                image.color = Color.Lerp(image.color, activeColor, Time.deltaTime * lerpMultiplier);
            }
        } 
        else
        {
            if(hovered)
            {
                foreach (Image image in targetImages)
                {
                    image.color = Color.Lerp(image.color, hoveredColor, Time.deltaTime * lerpMultiplier);
                }
            } 
            else
            {
                foreach (Image image in targetImages)
                {
                    image.color = Color.Lerp(image.color, unhoveredColor, Time.deltaTime * lerpMultiplier);
                }
            }
        }
    }
}