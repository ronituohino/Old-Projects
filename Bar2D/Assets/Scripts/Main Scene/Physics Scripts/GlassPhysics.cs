using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GlassPhysics : PhysicsObject
{
    [Header("Glass Settings")]
    public Glass glass;

    [System.Serializable]
    public class Contents
    {
        public Bottle bottle;
        public int fluidContained;
    }

    public List<Contents> contents = new List<Contents>();
    int contentsCount = 0;

    public UnityAction OnFluidUpdate;

    public SpriteRenderer fullnessSprite;
    Material fullnessMaterialInstance;

    void Start()
    {
        fullnessMaterialInstance = new Material(GlobalReferencesAndSettings.Instance.glassFillMaterial);
        fullnessSprite.material = fullnessMaterialInstance;

        fullnessMaterialInstance.SetFloat("_Height", fullnessSprite.sprite.rect.height);

        gameObject.layer = 11;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Fluid"))
        {
            Vector2 fromGlassToParticle = collision.transform.position - transform.position;

            float angle = Vector2.Angle(transform.up, fromGlassToParticle);

            if(angle < glass.angleForAcceptibleParticle)
            {
                Fluid f = collision.GetComponent<Fluid>();
                AddFluid(f.bottle);

                GlobalReferencesAndSettings.Instance.fluidManager.DestroyFluid(f);
            }
        }
    }

    // Called when a single 1x1 particle of fluid is added
    public void AddFluid(Bottle bottle)
    {
        // Check if we have similar fluid in glass already, if so add to unitsContained
        bool foundFluid = false;
        for(int i = 0; i < contentsCount; i++)
        {
            Contents content = contents[i];

            if(content.bottle == bottle)
            {
                content.fluidContained += 1;

                foundFluid = true;
                break;
            }
        }

        // The glass doesn't contain similar fluid, add new entry
        if(!foundFluid)
        {
            Contents newContent = new Contents();

            newContent.bottle = bottle;
            newContent.fluidContained = 1;

            contents.Add(newContent);
            contentsCount++;
        }

        // Check spillage
        CheckSpillage();

        UpdateMaterial();
        OnFluidUpdate?.Invoke();
    }

    void CheckSpillage()
    {
        int totalFluid = 0;
        for (int i = 0; i < contentsCount; i++)
        {
            totalFluid += contents[i].fluidContained;
        }
        if (totalFluid > glass.fluidCapacity)
        {
            int over = totalFluid - glass.fluidCapacity;

            int randomStartElement = Random.Range(0, contentsCount);
            while (over > 0)
            {
                for (int i = 0; i < contentsCount; i++)
                {
                    int randomIndex = Extensions.WrapAroundRange(i + randomStartElement, 0, contentsCount);
                    Contents content = contents[randomIndex];

                    int newFluidAmount = content.fluidContained - 1;

                    // Delete if new amount is 0 fluid units
                    if (newFluidAmount == 0)
                    {
                        contents.RemoveAt(randomIndex);

                        i--;
                        contentsCount--;
                    }
                    else
                    {
                        content.fluidContained = newFluidAmount;
                    }

                    over--;
                    if(over <= 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    void UpdateMaterial()
    {
        Dictionary<Color, float> pairs = new Dictionary<Color, float>();

        int sum = 0;
        for(int i = 0; i < contentsCount; i++)
        {
            Contents c = contents[i];

            float relation = (float)c.fluidContained / (float)glass.fluidCapacity;

            pairs.Add(c.bottle.fluidColor, relation);

            sum += c.fluidContained;
        }

        float total = (float)sum / (float)glass.fluidCapacity;
        float overPerEntry = (1f - total) / contentsCount;

        Color fluidColor = Color.clear;

        foreach(var entry in pairs)
        {
            fluidColor += entry.Key * (entry.Value + overPerEntry);
        }

        // Bug with shader
        /*float firstStep = 1f / fullnessMaterialInstance.GetFloat("_Height");
        if (total < firstStep)
        {
            total = -0.1f;
        }*/

        fullnessMaterialInstance.SetFloat("_Fullness", total);
        fullnessSprite.color = fluidColor;
    }
}
