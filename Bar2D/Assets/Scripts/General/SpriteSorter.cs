using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorter : MonoBehaviour
{
    public SpriteRenderer thisSpriteRenderer;

    [Header("Settings")]

    public float yOffset;

    [Space]

    public LayerMask layerInteraction;
    public int layerOffset;

    [Space]

    public Rigidbody2D thisRigidbody;

    [Space]

    public SpriteRenderer connectedSpriteRenderer;
    public int offsetToConnected;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.isStatic)
        {
            SetOrderInLayer();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!gameObject.isStatic)
        {
            if(thisRigidbody != null)
            {
                if(!thisRigidbody.IsSleeping())
                {
                    SetOrderInLayer();
                }
            }
            else
            {
                SetOrderInLayer();
            }
        }
    }

    void SetOrderInLayer()
    {
        bool normal = true;

        if (connectedSpriteRenderer != null)
        {
            normal = false;
            thisSpriteRenderer.sortingOrder = connectedSpriteRenderer.sortingOrder + offsetToConnected;
        }
        else if (layerInteraction.value != 0)
        {
            RaycastHit2D[] results = Physics2D.BoxCastAll
                                        (
                                            thisSpriteRenderer.bounds.center,
                                            thisSpriteRenderer.bounds.size,
                                            transform.rotation.eulerAngles.z,
                                            Vector2.zero,
                                            0f,
                                            layerInteraction.value
                                        );

            if(results.Length > 0)
            {
                normal = false;
                int mostOnTopSortingOrder = int.MinValue;

                foreach(RaycastHit2D rHit in results)
                {
                    SpriteRenderer sr = rHit.transform.GetComponent<SpriteRenderer>();
                    if (sr.sortingOrder > mostOnTopSortingOrder)
                    {
                        mostOnTopSortingOrder = sr.sortingOrder;
                    }
                }

                thisSpriteRenderer.sortingOrder = mostOnTopSortingOrder + layerOffset;
            }
        }

        if(normal)
        {
            thisSpriteRenderer.sortingOrder = Mathf.FloorToInt((-transform.position.y - yOffset) * 100f);
        }
    }
}
