using System.Collections;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer shadowSpriteRenderer;
    public SpriteSorter spriteSorter;

    public PhysicsObject physicsObject;
    public SpriteRenderer spriteRenderer;

    [Space]

    public Color shadowColorOnGround;
    public Color shadowColorOnHover;
    public Color shadowColorOnCarry;

    [Space]

    [SerializeField] float shadowWidthAdd = 3f;
    [SerializeField] float shadowHeight = 5f;

    [SerializeField] Vector2 shadowDimensionsMultipliers = new Vector2(3, 2);

    public Vector2 shadowPosition;

    private void Start()
    {
        shadowSpriteRenderer.color = shadowColorOnGround;


        print(physicsObject.transform.position);
        print(spriteRenderer.sprite.bounds.center);

        UpdateShadow();
    }

    // Update is called once per frame
    void Update()
    {
        if(physicsObject.pickable)
        {
            UpdateShadow();

            // SHADOW COLOR
            if (physicsObject.beingHovered)
            {
                shadowSpriteRenderer.color = Color.Lerp
                                                (
                                                    shadowSpriteRenderer.color,
                                                    shadowColorOnHover,
                                                    physicsObject.currentHeightFromGround / GlobalReferencesAndSettings.Instance.objectHoverHeight
                                                );
            }
            else if (physicsObject.carryingHand != null)
            {
                shadowSpriteRenderer.color = Color.Lerp
                                                (
                                                    shadowSpriteRenderer.color,
                                                    shadowColorOnCarry,
                                                    0.2f
                                                );
            }
            else // On ground
            {
                shadowSpriteRenderer.color = Color.Lerp
                                                (
                                                    shadowSpriteRenderer.color,
                                                    shadowColorOnGround,
                                                    0.2f
                                                );
            }
        }
        
    }

    void UpdateShadow()
    {
        Vector2 center = (physicsObject.transform.rotation * spriteRenderer.sprite.bounds.center) + physicsObject.transform.position;
        //Vector2 topRight = (transform.rotation * spriteRenderer.sprite.bounds.max) + transform.position;
        //Vector2 bottomLeft = (transform.rotation * spriteRenderer.sprite.bounds.min) + transform.position;

        // (~Shadow width)
        float xDistance = spriteRenderer.bounds.size.x;

        // (~Shadown height)
        //float yDistance = spriteRenderer.bounds.size.y;

        // SHADOW POSITION & ROTATION
        // 0-360 degrees
        float angle = physicsObject.transform.rotation.eulerAngles.z;

        float distUp = spriteRenderer.sprite.bounds.extents.y;
        float distRight = spriteRenderer.sprite.bounds.extents.x;

        Vector2 toCornerVec = (spriteRenderer.sprite.bounds.max - spriteRenderer.sprite.bounds.center).normalized;
        float cornerAngle = Vector2.Angle(Vector2.up, toCornerVec);
        float distCorner = Mathf.Sqrt(Mathf.Pow(distRight, 2) + Mathf.Pow(distUp, 2));

        float distanceToGround = CalculateDistanceToGround(angle, cornerAngle, distUp, distCorner, distRight);

        shadowPosition = center + Vector2.down * (distanceToGround + physicsObject.currentHeightFromGround);

        // Finally set shadow parameters
        transform.position = shadowPosition;
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
        transform.localScale = new Vector3
                                    (
                                        (xDistance + (shadowWidthAdd * 1 / 32f)) * shadowDimensionsMultipliers.x,
                                        shadowHeight * (1 / 32f) * shadowDimensionsMultipliers.y,
                                        1f
                                    );
    }

    float CalculateDistanceToGround(float angle, float cornerAngle, float distUp, float distCorner, float distRight)
    {
        int quarter = Mathf.FloorToInt(Mathf.Abs(angle) / 90f);

        switch (quarter)
        {
            case 0: // 0-90
                bool u1 = angle < cornerAngle;
                if (u1)
                {
                    return Mathf.Lerp(distUp, distCorner, angle.Remap(0, cornerAngle, 0, 1));
                }
                else
                {
                    return Mathf.Lerp(distCorner, distRight, angle.Remap(cornerAngle, 90f, 0, 1));
                }
            case 1: // 90-180
                bool u2 = angle < 180f - cornerAngle;
                if (u2)
                {
                    return Mathf.Lerp(distRight, distCorner, angle.Remap(90f, 180f - cornerAngle, 0, 1));
                }
                else
                {
                    return Mathf.Lerp(distCorner, distUp, angle.Remap(180f - cornerAngle, 180f, 0, 1));
                }
            case 2: // 180-270
                bool u3 = angle < 180f + cornerAngle;
                if (u3)
                {
                    return Mathf.Lerp(distUp, distCorner, angle.Remap(180f, 180f + cornerAngle, 0, 1));
                }
                else
                {
                    return Mathf.Lerp(distCorner, distRight, angle.Remap(180f + cornerAngle, 270f, 0, 1));
                }
            case 3: // 270-360
                bool u4 = angle < 360f - cornerAngle;
                if (u4)
                {
                    return Mathf.Lerp(distRight, distCorner, angle.Remap(270f, 360f - cornerAngle, 0, 1));
                }
                else
                {
                    return Mathf.Lerp(distCorner, distUp, angle.Remap(360f - cornerAngle, 360f, 0, 1));
                }
            default:
                return 0f;
        }
    }
}