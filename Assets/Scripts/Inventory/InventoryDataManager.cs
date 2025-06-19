using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class InventoryDataManager : MonoBehaviour
{
    private DatabaseReference dbRoot;
    private FirebaseAuth      auth;

    private void Awake()
    {
        // Firebase Auth, Database 초기화
        auth   = FirebaseAuth.DefaultInstance;
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// raw JSON 문자열을 users/{uid}/inventory 경로에 저장합니다.
    /// </summary>
    public void SaveRawJson(string rawJson)
    {
        if (auth.CurrentUser == null) return;

        string path = $"users/{auth.CurrentUser.UserId}/inventory";
        dbRoot.Child(path)
              .SetRawJsonValueAsync(rawJson)
              .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("인벤토리 저장 성공");
            else
                Debug.LogError($"인벤토리 저장 실패: {task.Exception}");
        });
    }

    /// <summary>
    /// users/{uid}/inventory 경로에서 슬롯별 데이터를 불러와 List<SlotSaveData>로 전달합니다.
    /// </summary>
    public void LoadInventory(Action<List<SlotSaveData>> onLoaded)
    {
        if (auth.CurrentUser == null)
        {
            onLoaded?.Invoke(new List<SlotSaveData>());
            return;
        }

        string path = $"users/{auth.CurrentUser.UserId}/inventory";
        dbRoot.Child(path)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                var list = new List<SlotSaveData>();
                if (task.IsCompleted && task.Result.Exists)
                {
                    var savedNode = task.Result.Child("savedInvenDatas");
                    Debug.Log($"[LoadInventory] ChildrenCount = {savedNode.ChildrenCount}");

                    foreach (var child in savedNode.Children)
                    {
                        string json = child.GetRawJsonValue();
                        var slot = JsonUtility.FromJson<SlotSaveData>(json);
                        list.Add(slot);
                        Debug.Log($"  Slot #{child.Key} → itemId={slot.itemId}, count={slot.count}");
                    }
                }
                else
                {
                    Debug.Log("[LoadInventory] No data or task failed");
                }
                onLoaded?.Invoke(list);
            });
    }
}
