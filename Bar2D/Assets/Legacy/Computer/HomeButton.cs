using UnityEngine;

public class HomeButton : MonoBehaviour, IHoverable, ILeftClickable
{
    [SerializeField] ColorChangeDriver colorChangeDriver;
    bool interactable = false;

    private void Update()
    {
        interactable = ComputerBrowser.Instance.currentTab.currentWebsite.siteObject != ComputerBrowser.Instance.registeredSites[0].siteObject;
        colorChangeDriver.SetInteractable(interactable);
    }

    void ILeftClickable.OnClickHold() { }

    void ILeftClickable.OnClickPress() { }

    void ILeftClickable.OnClickRelease()
    {
        if (interactable)
        {
            ComputerBrowser.Instance.ReturnToHomePage();
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