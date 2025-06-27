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
    [SerializeField] private TradeManager       tradeMgr;
    [SerializeField] private PlayerController       playerController;

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
        
        var save = new GameSaveData
        {
            inventory = inventoryMgr.GetSaveData(),
            time      = timeMgr.GetSaveData(),
            sky       = skyMgr.GetSaveData(),
            map       = mapMgr.GetSaveData(),
            trade      = tradeMgr.GetSaveData(),
            player     = playerController.GetSaveData()
        };
        
        string json = JsonUtility.ToJson(save);
        
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


    public async Task LoadGameAsync()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogWarning("[DataSaveManager] 로그인된 사용자가 없습니다. 로드 생략");
            return;
        }

        var docRef = firestore
            .Collection("users")
            .Document(auth.CurrentUser.UserId);

        var snap = await docRef
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted) throw t.Exception;
                return t.Result;
            });

        if (!snap.Exists)
        {
            Debug.Log("[DataSaveManager] 불러올 데이터가 없습니다.");
            return;
        }

        if (!snap.TryGetValue("state", out string raw) || string.IsNullOrEmpty(raw))
        {
            Debug.Log("[DataSaveManager] 저장된 JSON이 비어있습니다.");
            return;
        }

        var save = JsonUtility.FromJson<GameSaveData>(raw);
        
        TimeManager.Instance.OnTimePeriodChanged -= skyMgr.OnPeriodChanged;
        
        inventoryMgr.LoadFromSave(save.inventory);
        timeMgr.LoadFromSave(save.time);
        skyMgr.SetPhaseImmediate(save.sky.phase);
        mapMgr.LoadFromSave   (save.map);
        tradeMgr.LoadFromSave(save.trade);
        playerController.LoadFromSave(save.player);


        TimeManager.Instance.OnTimePeriodChanged += skyMgr.OnPeriodChanged;
        Debug.Log("[DataSaveManager] 게임 전체 로드 완료 (Async)");
    }
}
