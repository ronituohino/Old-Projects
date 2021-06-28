using UnityEngine;

public class UndoReturnButton : MonoBehaviour, IHoverable, ILeftClickable
{
    [SerializeField] ColorChangeDriver colorChangeDriver;
    bool interactable = false;

    private void Update()
    {
        interactable = ComputerBrowser.Instance.currentTab.historyPointer < ComputerBrowser.Instance.currentTab.siteHistoryCount;
        colorChangeDriver.SetInteractable(interactable);
    }

    void ILeftClickable.OnClickHold() { }

    void ILeftClickable.OnClickPress() { }

    void ILeftClickable.OnClickRelease()
    {
        if (interactable)
        {
            ComputerBrowser.Instance.UndoReturn();
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