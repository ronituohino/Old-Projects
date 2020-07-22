using UnityEngine;

public class torchScript : MonoBehaviour {
    public GameObject torch;
    public Light gLight;
    public ParticleSystem parSys;

    bool flicker;

    float time;
    //float flickerRand;
    //float flickerTime;
    float scale = 0.1f;
    //bool set;
    int i = 0;

    public bool torchOverride;

    private void Start()
    {
        parSys.Play();
        //gLight = torch.transform.GetChild(0).GetComponent<Light>();
        //parSys = torch.transform.GetChild(1).GetComponent<ParticleSystem>();
        //toggleTorch(torchOverride); //Debug override
    }
    public void toggleTorch(bool on) //Toggles the torch on/off
    {
        flicker = on;
        if (on) //Enable / Disable particles
        {
            parSys.enableEmission = true; //Turns false down there
            
        } 
    }
    private void Update() //Flicker the light range
    {
        if (!dayNightCycle.day) //Toggle torh during nighttime (Temp)
        {
            toggleTorch(true);
        } else
        {
            toggleTorch(false);
        }

        if (flicker)
        {
            if (gLight.range < 50) //Smoothly fade out the torch upon sunrise
            {
                if (parSys.emissionRate <= 10)
                {
                    //Debug.Log(i);
                    i++;
                    if (i >= 30)
                    {
                        parSys.emissionRate++;
                        i -= 30;
                    }
                }
                else
                {
                    if (parSys.enableEmission)
                    {
                        parSys.enableEmission = true;
                        i = 0;
                    }
                }

                gLight.range += 0.1f;
                scale = 0.1f; //Reset scaling for lighing it up again
            }
        } else
        {
            if(gLight.range > 0) //Smoothly fade out the torch upon sunrise
            {
                if (parSys.emissionRate > 0)
                {
                    //Debug.Log(i);
                    i++;
                    if (i >= 30)
                    {
                        parSys.emissionRate--;
                        i -= 30;
                    }
                } else
                {
                    if (parSys.enableEmission)
                    {
                        parSys.enableEmission = false;
                        i = 0;
                    }
                }
                gLight.range -= 0.1f;
                scale = 0.1f; //Reset scaling for lighing it up again
            }
        }
    }
}
