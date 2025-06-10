using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    [Header("하루 시간 설정")]
    public float dayDuration = 120f;
    
    [Header("참조")]
    public TMP_Text timeText;
    
    public TimeManager timeManager;
    
    [Header("빠른 감기 설정")]
    public float fastForwardMultiplier = 2f;
    
    public float timeOfDay = 0f;

    private void Start()
    {
        if(timeText == null)
            timeText = GetComponentInChildren<TMP_Text>();
        if (timeManager == null)
            timeManager = FindObjectOfType<TimeManager>();
    }

    private void Update()
    {
        float delta = Time.deltaTime / dayDuration;
        
        if (Input.GetKey(KeyCode.T))
            delta *= fastForwardMultiplier;
        
        timeOfDay += delta;
        while (timeOfDay >= 1f)
        {
            timeManager.DayPassed();
            timeOfDay -= 1f;
        }
        
        UpdateTimeDisplay();
    }
    
    void UpdateTimeDisplay()
    {
        float totalHours = (timeOfDay * 24f) % 24f;
        
        int hours24 = (int)totalHours;                     
        int minutes = (int)((totalHours - hours24) * 60f);  
        
        bool isPM = hours24 >= 12;
        
        int displayHour;
        if (!isPM && hours24 == 0)
        {
            displayHour = 0;
        }
        else
        {
            displayHour = hours24 % 12;
            if (displayHour == 0) displayHour = 12;
        }
        
        string ampmText = isPM ? "PM" : "AM";
        timeText.text = $"{displayHour:00}:{minutes:00} {ampmText}";
    }
}
