using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeDriver : MonoBehaviour
{
    [SerializeField] List<Image> targetImages;

    [SerializeField] Color unhoveredColor;
    [SerializeField] Color interactableColor;
    [SerializeField] Color hoveredColor;

    [SerializeField] float lerpMultiplier;

    [SerializeField] bool interactable = false;
    bool hovering = false;

    public void SetHover(bool isHovering)
    {
        hovering = isHovering;
    }

    public void SetInteractable(bool isInteractable)
    {
        interactable = isInteractable;
    }

    private void Update()
    {
        if(interactable && hovering)
        {
            foreach(Image image in targetImages)
            {
                image.color = Color.Lerp(image.color, hoveredColor, Time.deltaTime * lerpMultiplier);
            }
        } 
        else if(interactable)
        {
            foreach (Image image in targetImages)
            {
                image.color = Color.Lerp(image.color, interactableColor, Time.deltaTime * lerpMultiplier);
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