using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public event Action OnDayPassed;
    public event Action<string> OnTimePeriodChanged;

    [Header("시간 설정")]
    [SerializeField] private float dayDuration = 120f;
    [SerializeField] private float fastForwardMultiplier = 2f;
    
    
    private string currentPeriod = "Morning";
    private int currentDay = 1;
    private float timeOfDay = 0f;
    public float NormalizedTime => timeOfDay;
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
        float delta = Time.deltaTime / dayDuration;
        if (Input.GetKey(KeyCode.T))
            delta *= fastForwardMultiplier;

        timeOfDay += delta;
        
        while (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
            currentDay++;
            OnDayPassed?.Invoke();
        }
        
        string newPeriod = GetPeriodFromHour(GetCurrentHour());
        if (newPeriod != currentPeriod)
        {
            currentPeriod = newPeriod;
            OnTimePeriodChanged?.Invoke(currentPeriod);
            Debug.Log($"{currentPeriod}");
        }
    }
    
    
    private string GetPeriodFromHour(float hour)
    {
        if (hour >= 5f && hour < 7f) return "Dawn";        // 새벽
        if (hour >= 8f && hour < 17f) return "Day";        // 낮
        if (hour >= 17f && hour < 19f) return "Sunset";    // 해질녘
        return "Evening";                                  // 그 외: 저녁 또는 밤
    }
    
    
    public float GetCurrentHour() => timeOfDay * 24f;

}
