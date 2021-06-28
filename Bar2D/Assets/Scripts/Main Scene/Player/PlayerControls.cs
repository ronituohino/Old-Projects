using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControls : Character
{
    public string playerName;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Animator animator;

    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;

    [Space]

    [SerializeField] float animatorSpeedDamp;

    [Space]

    [SerializeField] float coinPickRange;
    [SerializeField] float coinPickForceMultiplier;

    [HideInInspector]
    public PhysicsObject hoveredObject = null;

    Vector2 movement;
    
    public enum Mode
    {
        Carry,
        Use
    }

    public Mode mode = Mode.Carry;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Coins
        if(collision.gameObject.layer == 13)
        {
            Destroy(collision.gameObject);
            GlobalReferencesAndSettings.Instance.moneyManager.AddCoins(1);
        }
    }

    public void Move(Vector2 movement)
    {
        this.movement = movement;
    }

    public void ChangeMode()
    {
        if(mode == Mode.Carry)
        {
            mode = Mode.Use;
        }
        else
        {
            mode = Mode.Carry;
        }
    }

    void FixedUpdate()
    {
        // Player movement
        rb.AddForce(movement * movementSpeed, ForceMode2D.Impulse);

        // Search for coins, 8192 == Layer 13 == Coins
        Collider2D[] coins = Physics2D.OverlapCircleAll(transform.position, coinPickRange, 8192);
        foreach(Collider2D coin in coins)
        {
            coin.attachedRigidbody.velocity = Vector2.zero;
            coin.attachedRigidbody.AddForce((rb.worldCenterOfMass - coin.attachedRigidbody.worldCenterOfMass).normalized * coinPickForceMultiplier);
        }
    }

    private void Update()
    {
        if(rb.velocity.x < 0)
        {
            sr.flipX = true;
        }
        else if(rb.velocity.x > 0)
        {
            sr.flipX = false;
        }

        animator.SetFloat
                    (
                        "Speed", 
                        Mathf.Lerp
                                (
                                    animator.GetFloat("Speed"), 
                                    Mathf.Clamp01(rb.velocity.magnitude), 
                                    animatorSpeedDamp
                                )
                    );

    }

    public void StartedHover(PhysicsObject physicsObject)
    {
        left.searchingForGrabbableObject = true;
        right.searchingForGrabbableObject = true;
    }

    public void StoppedHover()
    {
        left.searchingForGrabbableObject = false;
        right.searchingForGrabbableObject = false;

        hoveredObject = null;
    }
}
