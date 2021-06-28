using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour, IHoverable, ILeftDraggable
{
    [Header("Hover Settings")]

    public bool pickable = true;

    [HideInInspector]
    public bool beingHovered = false;

    [HideInInspector]
    public float currentHeightFromGround = 0f;
    Coroutine pickCoroutine;
    Coroutine dropCoroutine;

    [HideInInspector]
    public List<Hand> handsInRange = null;
    public Hand carryingHand = null;

    [HideInInspector]
    public bool moveToTarget = false;
    [HideInInspector]
    public Vector2 moveTarget;
    [HideInInspector]
    public Vector2 moveAnchor;



    [Header("References")]

    public SpriteRenderer spriteRenderer;
    public SpriteSorter spriteSorter;
    public BoxCollider2D clickCollider;

    public Rigidbody2D rb;
    public CapsuleCollider2D capsuleCollider2D;

    public Shadow shadowScript;



    internal void Update()
    {
        
    }

    internal void FixedUpdate()
    {
        // Magic
        if(moveToTarget)
        {
            rb.velocity = Vector2.zero;

            float multiplier = 1f;
            if(carryingHand == null)
            {
                multiplier = GlobalReferencesAndSettings.Instance.objectMovementMultiplier;
            }

            rb.velocity = (moveTarget - (transform.position.ToV2() + (transform.rotation * moveAnchor).ToV2())) * multiplier;
        }
        rb.angularVelocity = 0f;

        // Object rotation
        if (!ignoreRotationCall)
        {
            RotateObject();
        }


        // Carrying
        if(pickable)
        {
            if (carryingHand != null)
            {
                moveTarget = carryingHand.transform.position;
            }
        }
    }

    // Keep object rotated upright
    internal bool ignoreRotationCall = false;
    internal void RotateObject()
    {
        if (beingHovered)
        {
            rb.AddTorque(Extensions.CalculateTorque(0f, transform, GlobalReferencesAndSettings.Instance.rotationMultiplier, GlobalReferencesAndSettings.Instance.rotationLimits));
        }
    }

    void IHoverable.OnHoverEnter()
    {

    }

    void IHoverable.OnHoverStay()
    {

    }

    void IHoverable.OnHoverExit()
    {

    }



    
    void ILeftDraggable.OnDragPress()
    {
        // Object hovering
        if(pickable && InputManager.Instance.controlledPlayer.mode == PlayerControls.Mode.Carry)
        {
            if (carryingHand != null)
            {
                transform.parent = GlobalReferencesAndSettings.Instance.physicsObjectsParent;
                rb.bodyType = RigidbodyType2D.Dynamic;

                carryingHand.StartedHover();
                carryingHand = null;
            }
            else
            {
                rb.gameObject.layer = 10;

                moveToTarget = true;
            }

            moveAnchor = transform.InverseTransformPoint(InputManager.Instance.mousePositionInWorld);
            moveTarget = InputManager.Instance.mousePositionInWorld;

            if(dropCoroutine != null)
            {
                StopCoroutine(dropCoroutine);
            }
            pickCoroutine = StartCoroutine(PickCoroutine());

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            spriteSorter.layerInteraction = 8;
            spriteSorter.layerOffset = 3;

            beingHovered = true;
        }
    }

    void ILeftDraggable.OnDrag()
    {
        if(pickable && InputManager.Instance.controlledPlayer.mode == PlayerControls.Mode.Carry)
        {
            moveTarget = InputManager.Instance.mousePositionInWorld;
        }
    }

    void ILeftDraggable.OnDragRelease()
    {
        if(pickable && beingHovered)
        {
            if (handsInRange.Count == 0)
            {
                // Drop object on ground
                moveToTarget = false;

                rb.gameObject.layer = 0;

                if(pickCoroutine != null)
                {
                    StopCoroutine(pickCoroutine);
                }
                dropCoroutine = StartCoroutine(DropCoroutine());

                spriteSorter.layerInteraction = 0;
                spriteSorter.layerOffset = 0;

                rb.drag = 0f;
                rb.gravityScale = 1f;
            }
            else
            {
                // Carry object
                moveAnchor = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;

                // Find the closest hand and attach this to that
                float closestDist = float.MaxValue;
                foreach(Hand h in handsInRange)
                {
                    float d = Vector2.Distance(h.transform.position, rb.worldCenterOfMass);
                    if(d < closestDist)
                    {
                        closestDist = d;
                        carryingHand = h;
                    }
                }

                carryingHand.StoppedHover(this);
            }

            rb.interpolation = RigidbodyInterpolation2D.None;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            beingHovered = false;
        }
    }

    IEnumerator PickCoroutine()
    {
        int fixedUpdateFramesToWait = Mathf.RoundToInt(Extensions.CalculateDropTime(GlobalReferencesAndSettings.Instance.objectHoverHeight) / Time.fixedDeltaTime);
        float heightStep = (GlobalReferencesAndSettings.Instance.objectHoverHeight - currentHeightFromGround) / fixedUpdateFramesToWait;

        for (int i = 0; i < fixedUpdateFramesToWait; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;

            currentHeightFromGround += heightStep;
        }

        currentHeightFromGround = GlobalReferencesAndSettings.Instance.objectHoverHeight;
    }

    IEnumerator DropCoroutine()
    {
        int fixedUpdateFramesToWait = Mathf.RoundToInt(Extensions.CalculateDropTime(currentHeightFromGround) / Time.fixedDeltaTime);
        float heightStep = currentHeightFromGround / fixedUpdateFramesToWait;

        for (int i = 0; i < fixedUpdateFramesToWait; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;

            currentHeightFromGround -= heightStep;
        }

        currentHeightFromGround = 0f;

        rb.gravityScale = 0f;
        rb.drag = 5f;
        rb.velocity *= GlobalReferencesAndSettings.Instance.groundHitForceMultiplier;
    }
}
