using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public event Action OnDayPassed;
    public event Action<string> OnTimePeriodChanged;

    [Header("시간 설정")]
    [SerializeField] private float dayDuration = 120f;          // 하루 길이(초)
    [SerializeField] private float fastForwardMultiplier = 2f;  // 빠른 진행 배율

    private float DayStartOffset = 5f / 24f;  
    private float dayRolloverHour = 12 / 24f;
    private bool hasPassedDayStart;                             // 하루 경계 감지 플래그

    private int currentDay = 1;
    private float timeOfDay;                                    // 0.0 ~ 1.0 (0==00:00, 1==24:00)
    private string currentPeriod;

    public float NormalizedTime => timeOfDay;
    public int CurrentDay      => currentDay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // 게임 시작 시 AM 5시로 초기화
            timeOfDay         = DayStartOffset;
            hasPassedDayStart = true;  
            currentPeriod     = GetPeriodFromHour(GetCurrentHour());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 1) 시간 증가
        float delta = Time.deltaTime / dayDuration;
        if (Input.GetKey(KeyCode.T))
            delta *= fastForwardMultiplier;
        timeOfDay += delta;

        // 2) normalized 값 유지 (하루 단위 순환)
        if (timeOfDay >= 1f)
            timeOfDay -= 1f;

        // 3) AM 5시 경계 단발성 감지
        if (!hasPassedDayStart && timeOfDay >= dayRolloverHour)
        {
            hasPassedDayStart = true;
            currentDay++;
            OnDayPassed?.Invoke();
        }
        else if (hasPassedDayStart && timeOfDay < dayRolloverHour)
        {
            hasPassedDayStart = false;
        }

        // 4) 시간대 변경 판단
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
        hasPassedDayStart = true;
        currentPeriod = GetPeriodFromHour(GetCurrentHour());
        
        OnDayPassed?.Invoke();
        OnTimePeriodChanged?.Invoke(currentPeriod);
    }
    
}
