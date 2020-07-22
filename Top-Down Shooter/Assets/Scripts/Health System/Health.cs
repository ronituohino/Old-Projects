using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Range(0f, 1f)]
    public float health = 1f;

    //100 == the player health, human
    [Range(1, 500)]
    public float maxHealth = 100f;

    public bool isPlayer;
    const float playerMaxHealth = 100f;

    public HealthProfile healthProfile;

    [Space]
    
    public Limb[] limbs;
    //0 = Head
    //1 = Torso

    //2 = Right Leg
    //3 = Left Leg

    //4 = Right Arm
    //5 = Left Arm

    public Action OnDie;

    public enum Severity
    {
        Minor,
        Severe,
        Extreme
    }

    private void Start()
    {
        int limbAmount = healthProfile.limbInitialDamageMultipliers.Length;
        limbs = new Limb[limbAmount];
        for(int i = 0; i < limbAmount; i++)
        {
            limbs[i] = new Limb(new List<Hit>());
        }
    }

    private void Update()
    {
        health = CalculateOverallHealth();
        if(health <= 0f)
        {
            OnDie?.Invoke();
        }
    }

    //Add a hit to limb
    public void CallHit(int limb, float bulletThreat)
    {
        Severity s = CalculateSeverity(bulletThreat);

        //Debug.Log("Hit! Severity: " + s.ToString() + " Limb: " + limb);

        float initialDamage = 0f;
        float bleed = 0f;
        float regen = 0f;

        if(s == Severity.Minor)
        {
            initialDamage = UnityEngine.Random.Range(healthProfile.minorWoundInitialDamage.x, healthProfile.minorWoundInitialDamage.y);
            bleed = UnityEngine.Random.Range(healthProfile.minorWoundBleed.x, healthProfile.minorWoundBleed.y);
            regen = UnityEngine.Random.Range(healthProfile.minorWoundRegenerate.x, healthProfile.minorWoundRegenerate.y);
        }
        else if(s == Severity.Severe)
        {
            initialDamage = UnityEngine.Random.Range(healthProfile.severeWoundInitialDamage.x, healthProfile.severeWoundInitialDamage.y);
            bleed = UnityEngine.Random.Range(healthProfile.severeWoundBleed.x, healthProfile.severeWoundBleed.y);
            regen = UnityEngine.Random.Range(healthProfile.severeWoundRegenerate.x, healthProfile.severeWoundRegenerate.y);
        }
        else
        {
            initialDamage = UnityEngine.Random.Range(healthProfile.extremeWoundInitialDamage.x, healthProfile.extremeWoundInitialDamage.y);
            bleed = UnityEngine.Random.Range(healthProfile.extremeWoundBleed.x, healthProfile.extremeWoundBleed.y);
            regen = UnityEngine.Random.Range(healthProfile.extremeWoundRegenerate.x, healthProfile.extremeWoundRegenerate.y);
        }

        limbs[limb].hits.Add(new Hit(Time.time, s, initialDamage, bleed, regen, limb, (isPlayer ? GetRandomWoundLocation(limb) : Vector2.zero)));
    }

    Severity CalculateSeverity(float bulletThreat)
    {
        float final = bulletThreat + UnityEngine.Random.Range(-healthProfile.severitySway, healthProfile.severitySway);
        if(final < healthProfile.severeWoundThreshold)
        {
            return Severity.Minor;
        }
        else if(final < healthProfile.extremeWoundThreshold)
        {
            return Severity.Severe;
        }
        else
        {
            return Severity.Extreme;
        }
    }

    Vector2 GetRandomWoundLocation(int limb)
    {
        return Vector2.zero; //Come back to this later when the UI is done
    }

    public float CalculateOverallHealth()
    {
        float health = 1f;
        float scaling = playerMaxHealth / maxHealth;

        foreach (Limb limb in limbs)
        {
            int hitCount = limb.hits.Count;
            for (int i = 0; i < hitCount; i++)
            {
                Hit h = limb.hits[i];

                if (!h.treated)
                {
                    float healthBleed = h.bleed * (Time.time - h.timeWhenGotHit) * scaling;
                    health -= h.initialDamage * scaling;
                    health -= healthBleed;
                }
                else
                {
                    float healthBleed = h.bleed * (h.timeWhenTreated - h.timeWhenGotHit) * scaling;
                    float healthRegenerated = h.regenerateSpeed * (Time.time - h.timeWhenTreated) * scaling;

                    //Health regenerated is more than the initial damage + bleed so, the wound has healed
                    if (healthRegenerated > h.initialDamage + healthBleed)
                    {
                        limb.hits.RemoveAt(i);
                        hitCount--;
                    }
                    else
                    {
                        health -= h.initialDamage * scaling;
                        health += -healthBleed + healthRegenerated;
                    }
                }
            }
        }

        return health;
    }
}

[System.Serializable]
public struct Limb
{
    public List<Hit> hits;

    public Limb(List<Hit> hits)
    {
        this.hits = hits;
    }
}

[System.Serializable]
public struct Hit
{
    public bool treated;
    public float timeWhenTreated;
    public float regenerateSpeed;

    public float timeWhenGotHit;
    public Health.Severity hitSeverity;

    public float initialDamage;
    public float bleed;

    int limb;
    public Vector2 location;

    public Hit(float time, Health.Severity hitSeverity, float initialDamage, float bleed, float regenerateSpeed, int limb, Vector2 location)
    {
        this.timeWhenGotHit = time;
        this.hitSeverity = hitSeverity;

        this.initialDamage = initialDamage;
        this.bleed = bleed;

        this.regenerateSpeed = regenerateSpeed;

        this.limb = limb;
        this.location = location;

        treated = false;
        this.timeWhenTreated = 0f;
    }
}

public struct Condition
{
    public string name;
}