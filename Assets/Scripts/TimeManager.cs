using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public event Action OndayPassed;
    
    private int currentDay = 1;

    private bool isDayPassed;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (isDayPassed)
        {
            currentDay++;
            OndayPassed?.Invoke();
            isDayPassed = false;
        }
    }

    public void DayPassed()
    {
        isDayPassed = true;
    }

}
