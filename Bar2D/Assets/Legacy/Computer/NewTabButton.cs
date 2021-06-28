using UnityEngine;

public class NewTabButton : MonoBehaviour, IHoverable, ILeftClickable
{
    [SerializeField] ColorChangeDriver colorChangeDriver;
    bool interactable = false;

    private void Update()
    {
        //Max amount of tabs
        interactable = ComputerBrowser.Instance.tabs.Count < 4;
        colorChangeDriver.SetInteractable(interactable);
    }

    void ILeftClickable.OnClickHold() { }

    void ILeftClickable.OnClickPress() { }

    void ILeftClickable.OnClickRelease()
    {
        if (interactable)
        {
            ComputerBrowser.Instance.AddTab();
        }
    }

    void IHoverable.OnHoverEnter()
    {
        colorChangeDriver.SetHover(true);
    }

    void IHoverable.OnHoverExit()
    {
        colorChangeDriver.SetHover(false);
    }

    void IHoverable.OnHoverStay() { }
}