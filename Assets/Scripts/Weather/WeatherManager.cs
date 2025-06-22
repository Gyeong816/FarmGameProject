using System;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    
    [Header("비 이펙트")] [SerializeField] private GameObject rainObj;

    [Header("주간 비 내리는 날 수동 설정 (0=월요일 … 6=일요일)")]
    [SerializeField, Tooltip("길이가 7인 배열을 사용하세요. true인 인덱스의 요일에 비가 옵니다.")]
    private bool[] weeklyRainSchedule = new bool[7]
    {
        false, true, false, false, true, false, false
    };

    public bool isRaining { get; private set; } 
    
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

        // 시작 시 바로 오늘 날씨 적용
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

        MapManager.Instance.WaterAllPlowedTiles();
    
        RenderSettings.fog = true;

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain start. Fog on.");
    }

    private void StopRain()
    {
        isRaining = false; 
        
        rainObj.SetActive(false);

        // 안개 끄기
        RenderSettings.fog = false;

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain end. Fog off.");
    }
}
