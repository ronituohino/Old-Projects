using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlePhysics : PhysicsObject
{
    public Bottle bottle;

    [Range(0f, 1f)]
    public float fullness;

    GameObject pourObject = null;
    ParticleSystem pourSystem = null;

    float timer;

    new void Start()
    {
        pourObject = Instantiate
                                    (
                                        GlobalReferencesAndSettings.Instance.pourObject,
                                        transform.position + transform.rotation * bottle.bottleFluidPoint,
                                        transform.rotation,
                                        transform
                                    );
        pourSystem = pourObject.GetComponent<ParticleSystem>();
        Material mat = pourSystem.GetComponent<Renderer>().material;
        mat.SetColor("_BaseColor", bottle.fluidColor); 

        base.Start();
    }

    // If this bottle falls over, spill contents
    private void FixedUpdate()
    {
        Spill();

        // Also check when the player stops pouring
        if(pourSystem.isPlaying)
        {
            if (timer > Time.fixedDeltaTime * 2)
            {
                EndPour();
            }

            timer += Time.fixedDeltaTime;
        }
    }

    void Spill()
    {

    }

    public void Pour(GlassPhysics targetGlass)
    {
        // Start pouring
        if (fullness > 0 && !pourSystem.isPlaying)
        {
            pourSystem.Play();
        }

        if (fullness <= 0 && pourSystem.isPlaying)
        {
            EndPour();
        }
        else
        {
            float units = bottle.pourRatePerSecond * Time.fixedDeltaTime;
            RaycastHit hitInfo;

            Debug.DrawRay(transform.position + transform.rotation * bottle.bottleFluidPoint - new Vector3(0, 0.1f, 0), Vector3.down, Color.red, Time.fixedDeltaTime);

            if (Physics.Raycast(transform.position + transform.rotation * bottle.bottleFluidPoint - new Vector3(0, 0.1f, 0), Vector3.down, out hitInfo, Mathf.Infinity))
            {
                FluidAddCollider fac = hitInfo.collider.GetComponent<FluidAddCollider>();

                if (fac != null)
                {
                    fac.AddFluid(bottle, units);
                }
            }

            RemoveFluid(units);
        }

        timer = 0f;
    }

    public void EndPour()
    {
        pourSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        timer = 0f;
    }

    void RemoveFluid(float units)
    {
        float fullnessRemove = units / bottle.capacity;
        fullness -= fullnessRemove;
        fullness = Mathf.Clamp01(fullness);
    }
}
