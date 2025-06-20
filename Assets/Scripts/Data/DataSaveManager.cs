using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class DataSaveManager : MonoBehaviour
{
    public static DataSaveManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private InventoryManager inventoryMgr;
    [SerializeField] private TimeManager      timeMgr;
    [SerializeField] private SkyManager       skyMgr;
    [SerializeField] private MapManager      mapMgr;

    private FirebaseAuth      auth;
    private DatabaseReference dbRoot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
            // Firebase 초기화
            auth   = FirebaseAuth.DefaultInstance;
            dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 전체 게임 상태(인벤토리, 시간, 하늘)를 저장합니다.
    /// </summary>
    public Task SaveGameAsync()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogWarning("[DataSaveManager] 로그인된 사용자가 없습니다. 저장 생략");
            return Task.CompletedTask;
        }

        // 1) 각 매니저에서 저장 데이터 수집
        var save = new GameSaveData
        {
            inventory = inventoryMgr.GetSaveData(),
            time      = timeMgr.GetSaveData(),
            sky       = skyMgr.GetSaveData(),
            map       = mapMgr.GetSaveData()
        };

        // 2) JSON 직렬화
        string json = JsonUtility.ToJson(save);

        // 3) Firebase에 비동기 저장
        string path = $"users/{auth.CurrentUser.UserId}/game";
        return dbRoot.Child(path)
                     .SetRawJsonValueAsync(json)
                     .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("[DataSaveManager] 게임 전체 저장 완료");
            else
                Debug.LogError("[DataSaveManager] 저장 실패: " + task.Exception);
        });
    }

    /// <summary>
    /// 저장된 전체 게임 상태를 불러와서 복원합니다.
    /// </summary>
    public void LoadGame()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogWarning("[DataSaveManager] 로그인된 사용자가 없습니다. 로드 생략");
            return;
        }

        string path = $"users/{auth.CurrentUser.UserId}/game";
        dbRoot.Child(path)
              .GetValueAsync()
              .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string raw = task.Result.GetRawJsonValue();
                var save  = JsonUtility.FromJson<GameSaveData>(raw);

                TimeManager.Instance.OnTimePeriodChanged -= skyMgr.OnPeriodChanged;
                
                // 인벤토리 복원
                inventoryMgr.LoadFromSave(save.inventory);

                // 시간 복원
                timeMgr.LoadFromSave(save.time);

                // 하늘 복원
                skyMgr.SetPhaseImmediate(save.sky.phase);
                
                // 타일정보 복원
                mapMgr.LoadFromSave(save.map);

                TimeManager.Instance.OnTimePeriodChanged += skyMgr.OnPeriodChanged;
                Debug.Log("[DataSaveManager] 게임 전체 로드 완료");
            }
            else
            {
                Debug.Log("[DataSaveManager] 불러올 데이터가 없습니다.");
            }
        });
    }
}
