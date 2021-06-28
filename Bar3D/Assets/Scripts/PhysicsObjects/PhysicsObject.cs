using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all physics objects
public class PhysicsObject : MonoBehaviour, ILeftDraggable, IRightDraggable, IHoverable
{
    List<GrabPoint> pointsThatGrab = new List<GrabPoint>();

    [HideInInspector]
    public Rigidbody rb;

    Transform originalParent;
    int originalLayer;

    internal void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    public virtual void EnteredRange()
    {
        ShowDot();
    }

    //Called every frame if this object is in range
    bool hiddenDuringGrab = false;
    public virtual void DuringRange()
    {
        //Hide the dot when grabbing the object
        if (!hiddenDuringGrab && pointsThatGrab.Count >= 1)
        {
            hiddenDuringGrab = true;
            HideDot();
        }

        if (hiddenDuringGrab && pointsThatGrab.Count < 1)
        {
            hiddenDuringGrab = false;
            ShowDot();
        }
    }

    public virtual void ExitedRange()
    {
        HideDot();
    }



    // If player comes close enough indicate that this can be grabbed
    void ShowDot()
    {
        InputManager.Instance.sceneCanvas.AnnotatePhysicsObject(transform);
    }

    void HideDot()
    {
        InputManager.Instance.sceneCanvas.RemoveElement(transform);
    }



    // If this object touches the players hand this object will stick to it
    public void GrabThis(GrabPoint p)
    {
        pointsThatGrab.Add(p);
        rb.useGravity = true;

        //If this is grabbed for the first time
        if (pointsThatGrab.Count <= 1)
        {
            originalParent = transform.parent;
            originalLayer = gameObject.layer;
        }

        transform.parent = p.transform;
        p.gameObject.layer = 8;
    }

    public void DropThis(GrabPoint p)
    {
        pointsThatGrab.Remove(p);
        rb.useGravity = true;

        //If this object is grabbed by multiple points
        if (pointsThatGrab.Count > 0)
        {
            transform.parent = pointsThatGrab[0].transform;
            //p.gameObject.layer = 8;
        }
        else
        {
            transform.parent = originalParent;
            gameObject.layer = originalLayer;
        }
    }



    void IHoverable.OnHoverEnter()
    {
        HoverEnter();
    }

    internal void HoverEnter()
    {

    }

    void IHoverable.OnHoverStay()
    {
        HoverStay();
    }

    internal void HoverStay()
    {

    }

    void IHoverable.OnHoverExit()
    {
        HoverExit();
    }

    internal void HoverExit()
    {

    }




    void ILeftDraggable.OnDragPress()
    {
        LeftDragPress();
    }

    internal void LeftDragPress()
    {
        InputManager.Instance.controlledPlayer.ObjectPress(InputManager.Instance.controlledPlayer.leftHand, this);
    }

    void ILeftDraggable.OnDrag()
    {

    }

    internal void LeftDrag()
    {

    }

    void ILeftDraggable.OnDragRelease()
    {
        LeftDragRelease();
    }

    internal void LeftDragRelease()
    {
        InputManager.Instance.controlledPlayer.ObjectRelease(InputManager.Instance.controlledPlayer.leftHand, this);
    }



    void IRightDraggable.OnDragPress()
    {
        RightDragPress();
    }

    internal void RightDragPress()
    {
        InputManager.Instance.controlledPlayer.ObjectPress(InputManager.Instance.controlledPlayer.rightHand, this);
    }

    void IRightDraggable.OnDrag()
    {

    }

    internal void RightDrag()
    {

    }

    void IRightDraggable.OnDragRelease()
    {
        RightDragRelease();
    }

    internal void RightDragRelease()
    {
        InputManager.Instance.controlledPlayer.ObjectRelease(InputManager.Instance.controlledPlayer.rightHand, this);
    }
}
