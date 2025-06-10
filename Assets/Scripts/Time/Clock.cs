using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    
    [Header("참조")]
    public TMP_Text timeText;
    
    private void Update()
    {
        float totalHours = TimeManager.Instance.GetCurrentHour();
        
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
