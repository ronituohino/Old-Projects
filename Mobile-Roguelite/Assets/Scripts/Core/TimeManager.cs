using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// This handles time scale changes from multiple sources
public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] float fixedTimeStep;
    [SerializeField] float maximumAllowedTimeStep;

    Dictionary<int, float> timeScales = new Dictionary<int, float>();
    [SerializeField] float timeLerp;
    
    // Use this to change time scale
    public void LowerTimeScale(int hashCode, float timeScale)
    {
        timeScales.Add(hashCode, timeScale);
    }

    // When slow-down is ready, use this to return to 1f.
    public void ReturnToNormal(int hashCode)
    {
        timeScales.Remove(hashCode);
    }

    private void Update()
    {
        float lowest = 1f;
        foreach(float value in timeScales.Values)
        {
            if(value < lowest)
            {
                lowest = value;
            }
        }

        Time.timeScale = Mathf.Lerp(Time.timeScale, lowest, timeLerp);

        Time.fixedDeltaTime = fixedTimeStep * Time.timeScale;
        Time.maximumDeltaTime = maximumAllowedTimeStep * Time.timeScale;
    }
}