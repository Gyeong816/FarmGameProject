using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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


    public Task SaveRawJsonAsync(string rawJson)
    {
        if (auth.CurrentUser == null)
            return Task.CompletedTask;

        string path = $"users/{auth.CurrentUser.UserId}/inventory";
        // SetRawJsonValueAsync 자체가 Task를 반환하므로, 이를 그대로 리턴합니다.
        return dbRoot
            .Child(path)
            .SetRawJsonValueAsync(rawJson);
    }
    
    public void LoadRawJson(Action<string> onLoaded)
    {
        if (auth.CurrentUser == null)
        {
            onLoaded?.Invoke(null);
            return;
        }

        string path = $"users/{auth.CurrentUser.UserId}/inventory";
        dbRoot.Child(path)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                    onLoaded?.Invoke(task.Result.GetRawJsonValue());
                else
                    onLoaded?.Invoke(null);
            });
    }

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
