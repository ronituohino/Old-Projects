using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public float timeSinceGameStart;

    public float dayLength; //seconds

    public int daysPassed = 0;
    int lastDaysPassed = 0;

    private void Awake()
    {
        timeSinceGameStart = 0f;
    }

    private void Start()
    {
        OnDayPassed(null);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceGameStart += Time.deltaTime;

        daysPassed = Mathf.FloorToInt(timeSinceGameStart / dayLength);

        if(daysPassed > lastDaysPassed)
        {
            OnDayPassed(null);
        }

        lastDaysPassed = daysPassed;
    }

    public event EventHandler DayPassed;

    protected virtual void OnDayPassed(EventArgs e)
    {
        EventHandler handler = DayPassed;
        handler?.Invoke(this, e);
    }
}
