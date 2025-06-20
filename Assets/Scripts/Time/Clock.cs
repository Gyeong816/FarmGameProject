using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    
    [Header("참조")]
    public TMP_Text timeText;
    public TMP_Text dayText;
    
    private void Start()
    {
        // 1) 이벤트 구독 (이제 NullReferenceException 없음)
        TimeManager.Instance.OnDayPassed += UpdateDayText;
        // 2) 초기 표시
        UpdateDayText();
    }

    private void OnDestroy()
    {
        // 언구독(메모리 누수 방지)
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= UpdateDayText;
    }

    
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
        timeText.text = $"{ampmText} {displayHour:00}:{minutes:00}";
        
        
    }
    
    private void UpdateDayText()
    {
        dayText.text = $"Day {TimeManager.Instance.CurrentDay}";
    }
    
}
