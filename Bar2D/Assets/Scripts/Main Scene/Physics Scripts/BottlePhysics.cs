using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlePhysics : PhysicsObject, IHoverable
{
    [Header("Bottle Settings")]

    public Bottle bottle;

    [HideInInspector]
    public int fluidContained;

    float pourTimer = 0f;

    bool opened = true;
    bool pouring = false;

    float targetAngle = 0f;

    bool hoveredWithCursor = false;
    bool lockBottleInfo = false;

    [HideInInspector]
    public BottleInfo shownInfo = null;

    GameObject pourTargetCross = null;

    Coroutine hoverCoroutine = null;
    Coroutine pourCoroutine = null;

    void Start()
    {
        ignoreRotationCall = true;
        fluidContained = bottle.fluidCapacity;

        // Spawn pour target cross transform
        pourTargetCross = Instantiate(GlobalReferencesAndSettings.Instance.pourTargetCrossInstance, transform);
        pourTargetCross.SetActive(false);
    }

    // Update is called once per frame
    new internal void Update()
    {
        base.Update();

        if(beingHovered && hoverCoroutine == null)
        {
            hoverCoroutine = StartCoroutine(Hover());
        }

        if (beingHovered && pouring)
        {
            if(pourCoroutine == null)
            {
                pourCoroutine = StartCoroutine(Pour());
            }

            // Find closest glass to pour to
            // Layer 11 == Glasses == 2^11 == 2048
            RaycastHit2D[] glasses = Physics2D.CircleCastAll
                                            (
                                                InputManager.Instance.mousePositionInWorld,
                                                GlobalReferencesAndSettings.Instance.pourRange,
                                                Vector2.zero,
                                                0f,
                                                2048
                                            );

            RaycastHit2D closestGlass = new RaycastHit2D();
            float closest = float.MaxValue;

            foreach (RaycastHit2D hit in glasses)
            {
                float distance = Vector2.Distance(InputManager.Instance.mousePositionInWorld, hit.collider.gameObject.transform.position);

                if (distance < closest)
                {
                    closest = distance;
                    closestGlass = hit;
                }
            }

            // Calculate bottle fluid spawn position and landing position
            Vector2 bottleOffset = transform.rotation * bottle.bottleFluidPoint;
            Vector2 spawnPosition = transform.position.ToV2() + bottleOffset;
            float heightToGroundHit = currentHeightFromGround + bottle.bottleFluidPoint.y;
            Vector2 fluidGroundHitPosition = spawnPosition - new Vector2(0, heightToGroundHit);

            if (closestGlass.collider != null)
            {
                // We found a glass
                if (opened && fluidContained > 0f)
                {
                    bool onRightSide = closestGlass.transform.position.x > InputManager.Instance.mousePositionInWorld.x;
                    targetAngle = GlobalReferencesAndSettings.Instance.pourCurve.Evaluate((float)fluidContained / (float)bottle.fluidCapacity)
                                * 180f
                                * (onRightSide ? 1f : -1f);

                    // Pour!
                    if (Vector2.Angle(transform.up, Vector2.up) >= Mathf.Abs(targetAngle) - 5f)
                    {
                        pourTimer += Time.deltaTime;

                        if(pourTimer >= 1f / bottle.fluidPerSecond)
                        {
                            pourTimer = 0f;

                            GlobalReferencesAndSettings.Instance.fluidManager.CreateFluid
                                (
                                    spawnPosition,
                                    bottle,
                                    Extensions.CalculateDropTime(heightToGroundHit)
                                );

                            fluidContained--;
                        }
                    }
                }
            }

            if (pourTargetCross.activeInHierarchy)
            {
                pourTargetCross.transform.position = fluidGroundHitPosition;
                pourTargetCross.transform.rotation = Quaternion.identity;
            }
        }
    }

    new internal void FixedUpdate()
    {
        RotateObject();

        base.FixedUpdate();
    }

    new internal void RotateObject()
    {
        if (beingHovered)
        {
            float torque = Extensions.CalculateTorque
                                    (
                                        targetAngle,
                                        transform,
                                        GlobalReferencesAndSettings.Instance.rotationMultiplier,
                                        GlobalReferencesAndSettings.Instance.rotationLimits
                                    );
            rb.AddTorque(torque);
        }
        else
        {
            base.RotateObject();
        }
    }

    void IHoverable.OnHoverEnter()
    {
        hoveredWithCursor = true;
        ShowInfo();
    }

    void ShowInfo()
    {
        if (!lockBottleInfo && InputManager.Instance.controlledPlayer.mode == PlayerControls.Mode.Carry)
        {
            Vector2 position = spriteRenderer.sprite.bounds.center.ToV2() + (spriteRenderer.sprite.bounds.size.ToV2() / 2f);

            if(shownInfo == null)
            {
                shownInfo = InputManager.Instance.sceneCanvas.ShowBottleInfo
                                        (
                                            transform,
                                            position,
                                            this
                                        );
            }
            else
            {
                shownInfo.Return();
            }
        }
    }

    void IHoverable.OnHoverStay() { }

    void IHoverable.OnHoverExit()
    {
        hoveredWithCursor = false;
        HideInfo();
    }

    void HideInfo()
    {
        if (!lockBottleInfo)
        {
            shownInfo?.Delete();
        }
    }

    void PourCall(bool pressed)
    {
        pouring = pressed;
    }



    IEnumerator Hover()
    {
        // Bottle info and hover
        InputManager.Instance.RightMouse += PourCall;
        HideInfo();
        lockBottleInfo = true;
        rb.constraints = RigidbodyConstraints2D.None;

        yield return new WaitUntil(() => !beingHovered);

        InputManager.Instance.RightMouse -= PourCall;
        if (hoveredWithCursor)
        {
            ShowInfo();
        }
        lockBottleInfo = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        hoverCoroutine = null;
    }

    IEnumerator Pour()
    {
        // Spawn target cross
        pourTargetCross.SetActive(true);

        yield return new WaitUntil(() => !pouring);

        // Destroy target cross
        pourTargetCross.SetActive(false);

        // Reset here so that if no glass is in range, the bottle is rotated up-right
        targetAngle = 0f;

        pourCoroutine = null;
    }
}
