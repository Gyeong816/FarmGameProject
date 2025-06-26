using System;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    
    [Header("비 이펙트")]
    [SerializeField] private GameObject rainObj;



    [Header("주간 비 내리는 날 수동 설정")]
    [SerializeField] private bool[] weeklyRainSchedule;

    public bool isRaining { get; private set; } 
    
    public event Action<bool> OnWeatherChanged;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("WeatherController: TimeManager 인스턴스를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        TimeManager.Instance.OnDayPassed += HandleDayPassed;
        
        ApplyWeatherForToday(TimeManager.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= HandleDayPassed;
    }

    private void HandleDayPassed()
    {
        ApplyWeatherForToday(TimeManager.Instance.CurrentDay);
    }

    private void ApplyWeatherForToday(int day)
    {
        int idx = (day - 1) % 7;
        if (weeklyRainSchedule.Length != 7)
        {
            Debug.LogWarning("weeklyRainSchedule 배열 크기를 7로 설정하세요.");
            return;
        }

        if (weeklyRainSchedule[idx])
            StartRain();
        else
            StopRain();
    }

    private void StartRain()
    {
        isRaining = true; 
        
        rainObj.SetActive(true);
        
        OnWeatherChanged?.Invoke(true);
        
        RenderSettings.fog = true;

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain start. Fog on.");
    }

    private void StopRain()
    {
        isRaining = false; 
        
        rainObj.SetActive(false);
        
        OnWeatherChanged?.Invoke(false);
        
        RenderSettings.fog = false;

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain end. Fog off.");
    }
}
