using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class DataSaveManager : MonoBehaviour
{

    [Header("Managers")]
    [SerializeField] private InventoryManager inventoryMgr;
    [SerializeField] private TimeManager      timeMgr;
    [SerializeField] private SkyManager       skyMgr;
    [SerializeField] private MapManager       mapMgr;

    private FirebaseAuth      auth;
    private FirebaseFirestore firestore;

    private void Awake()
    {
        
        auth      = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    }


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

        // 3) Firestore에 비동기 저장
        var docRef = firestore
            .Collection("users")
            .Document(auth.CurrentUser.UserId);

        return docRef
            .SetAsync(new { state = json }, SetOptions.MergeAll)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("[DataSaveManager] 게임 전체 저장 완료 (Firestore)");
            else
                Debug.LogError("[DataSaveManager] 저장 실패: " + task.Exception);
        });
    }


    public void LoadGame()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogWarning("[DataSaveManager] 로그인된 사용자가 없습니다. 로드 생략");
            return;
        }

        var docRef = firestore
            .Collection("users")
            .Document(auth.CurrentUser.UserId);

        docRef.GetSnapshotAsync()
              .ContinueWithOnMainThread(task =>
        {
            if (!(task.IsCompleted && task.Result.Exists))
            {
                Debug.Log("[DataSaveManager] 불러올 데이터가 없습니다.");
                return;
            }

            string raw = null;
            var snap = task.Result;
            if (snap.TryGetValue("state", out string stateJson))
                raw = stateJson;

            if (string.IsNullOrEmpty(raw))
            {
                Debug.Log("[DataSaveManager] 저장된 JSON이 비어있습니다.");
                return;
            }

            var save = JsonUtility.FromJson<GameSaveData>(raw);

            // 하늘 보간 이벤트 잠시 해제
            TimeManager.Instance.OnTimePeriodChanged -= skyMgr.OnPeriodChanged;

            // 순차 복원
            inventoryMgr.LoadFromSave(save.inventory);
            timeMgr     .LoadFromSave    (save.time);
            skyMgr      .SetPhaseImmediate(save.sky.phase);
            mapMgr      .LoadFromSave   (save.map);

            // 이벤트 재등록
            TimeManager.Instance.OnTimePeriodChanged += skyMgr.OnPeriodChanged;

            Debug.Log("[DataSaveManager] 게임 전체 로드 완료 (Firestore)");
        });
    }
}
