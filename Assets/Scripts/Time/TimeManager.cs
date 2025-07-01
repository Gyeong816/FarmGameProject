using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    public event Action OnDayPassed;
    public event Action<string> OnTimePeriodChanged;
    
    
    public bool isRainingToday { get; private set; }

    [Header("시간 설정")]
    [SerializeField] private float dayDuration = 120f;          
    [SerializeField] private float fastForwardMultiplier = 2f;  
    [SerializeField] private SkyManager skyManager;
    [SerializeField] private SaveLoadController saveLoadController;
    private float DayStartOffset = 5f / 24f;  
    

    private int currentDay = 1;
    private float timeOfDay;                                   
    private string currentPeriod;

    public float NormalizedTime => timeOfDay;
    public int CurrentDay      => currentDay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timeOfDay         = DayStartOffset;
        currentPeriod     = GetPeriodFromHour(GetCurrentHour());
    }

    private void Update()
    {
        float delta = Time.deltaTime / dayDuration;
        if (Input.GetKey(KeyCode.T))
            delta *= fastForwardMultiplier;
        timeOfDay += delta;
        
      
        if (timeOfDay >= 1f)
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
            Debug.Log(currentPeriod);
        }
    }

    // normalized 값을 시(hour) 단위로 환산[
    public float GetCurrentHour() => timeOfDay * 24f;

    // 시간대 반환 (Dawn, Day, Sunset, Evening)
    private string GetPeriodFromHour(float hour)
    {
        // 19:00 이상 또는 05:00 미만 → Evening
        if (hour >= 19f)
        {
            return "Evening";
        }
        if (hour >= 17f)
        {
            return "Sunset";
        }
        if (hour >= 8f)
        {
            return "Day";
        }
        if (hour >= 6f)
        {
            return "Dawn";
        }
        return null;
    }
    
    public void SkipNight()
    {
        // 1) 날짜 증가
        currentDay++;
        OnDayPassed?.Invoke();

        // 2) 시간 초기화 (AM 5시)
        timeOfDay     = DayStartOffset;
        currentPeriod = GetPeriodFromHour(GetCurrentHour());
        OnTimePeriodChanged?.Invoke(currentPeriod);
        
        skyManager.StopAllCoroutines();
        skyManager.SetPhaseImmediate(currentPeriod);
        saveLoadController.OnSaveClicked();
        Debug.Log($"Skipped to Day {currentDay}");
    }
    
    
    // 저장 및 로드

    public TimeSaveData GetSaveData()
    {
        return new TimeSaveData
        {
            day = currentDay,
            timeOfDay = this.timeOfDay
        };
    }

    public void LoadFromSave(TimeSaveData data)
    {
        currentDay = data.day;
        timeOfDay = data.timeOfDay;
        currentPeriod = GetPeriodFromHour(GetCurrentHour());
        
        OnDayPassed?.Invoke();
        OnTimePeriodChanged?.Invoke(currentPeriod);
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
    
}
