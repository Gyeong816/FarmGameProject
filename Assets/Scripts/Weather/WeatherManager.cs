using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("비 이펙트")]
    [SerializeField] private GameObject rainObj;

    [Header("주간 우천 랜덤 설정")]
    [SerializeField, Range(0, 7)] private int rainDaysPerWeek = 3;  // 주당 비 오는 날 개수

    private bool[] weeklyRainSchedule = new bool[7];
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
            Debug.LogError("WeatherManager: TimeManager 인스턴스를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        TimeManager.Instance.OnDayPassed += HandleDayPassed;
        
        GenerateWeeklyRainSchedule();
        ApplyWeatherForToday(TimeManager.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= HandleDayPassed;
    }

    private void HandleDayPassed()
    {
        int today = TimeManager.Instance.CurrentDay;
        int idx = (today - 1) % 7;
        
        // 주가 바뀌는 날(예: 인덱스 0)이면 스케줄 재생성
        if (idx == 0)
            GenerateWeeklyRainSchedule();

        ApplyWeatherForToday(today);
    }

    private void GenerateWeeklyRainSchedule()
    {
       
        for (int i = 0; i < 7; i++)
            weeklyRainSchedule[i] = false;
        
        List<int> indices = new List<int>(new int[] { 0,1,2,3,4,5,6 });
        for (int i = 0; i < rainDaysPerWeek && indices.Count > 0; i++)
        {
            int pick = UnityEngine.Random.Range(0, indices.Count);
            weeklyRainSchedule[indices[pick]] = true;
            indices.RemoveAt(pick);
        }

        Debug.Log($"[Weather] 주간 우천 스케줄 생성: {string.Join(",", weeklyRainSchedule)}");
    }

    private void ApplyWeatherForToday(int day)
    {
        int idx = (day - 1) % 7;
        if (weeklyRainSchedule[idx])
            StartRain();
        else
            StopRain();
    }

    private void StartRain()
    {
        if (isRaining) return;
        isRaining = true;
        
        rainObj.SetActive(true);
        RenderSettings.fog = true;
        SoundManager.Instance.PlayAmbience("RainLoop", 0.5f);
        OnWeatherChanged?.Invoke(true);

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain start.");
    }

    private void StopRain()
    {
        if (!isRaining) return;
        isRaining = false;
        
        rainObj.SetActive(false);
        RenderSettings.fog = false;
        SoundManager.Instance.PlayAmbience("ClearDayLoop", 1f);
        OnWeatherChanged?.Invoke(false);

        Debug.Log($"[Weather] Day {TimeManager.Instance.CurrentDay}: Rain end.");
    }
}
