using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Collider2D col;

    [HideInInspector]
    public Bottle bottle;

    [HideInInspector]
    public float groundHitSeconds;

    int enabledWaitFUI;
    int groundHitWaitFUI;
    
    [HideInInspector]
    public float hitForceMultiplier;
    [HideInInspector]
    public int disappearWaitFUI;
    [HideInInspector]
    public float endEnabledPercentage;

    private void Start()
    {
        sr.color = bottle.fluidColor;

        enabledWaitFUI = Mathf.RoundToInt(((1 - endEnabledPercentage) * groundHitSeconds) / Time.fixedDeltaTime);
        groundHitWaitFUI = Mathf.RoundToInt(groundHitSeconds / Time.fixedDeltaTime);

        StartCoroutine(EnableWait());
        StartCoroutine(GroundHit());
        StartCoroutine(DisappearWait());

        col.enabled = false;
    }

    IEnumerator EnableWait()
    {
        for(int i = 0; i < enabledWaitFUI; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;
        }

        col.enabled = true;
    }

    IEnumerator GroundHit()
    {
        for (int i = 0; i < groundHitWaitFUI; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;
        }

        rb.drag = 5f;

        rb.gravityScale = 0f;
        rb.velocity = Random.insideUnitCircle * hitForceMultiplier;
    }
    


    IEnumerator DisappearWait()
    {
        for(int i = 0; i < disappearWaitFUI; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;
        }

        StartCoroutine(FadeDisappear());
    }

    IEnumerator FadeDisappear()
    {
        int iterations = 30;
        for(int i = 0; i < iterations; i++)
        {
            yield return GlobalReferencesAndSettings.Instance.wait;

            sr.color = Color.Lerp(sr.color, Color.clear, (float)i / (float)iterations);
        }

        GlobalReferencesAndSettings.Instance.fluidManager.DestroyFluid(this);
    }
}
