using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainmenuButton;

    private InventoryManager     inventoryManager;
    private InventoryDataManager dataManager;

    private bool saveCompleted;
    private void Start()
    {
        saveCompleted = false;
        inventoryManager = InventoryManager.Instance;
        dataManager      = FindObjectOfType<InventoryDataManager>();

        dataManager.LoadInventory(OnInventoryLoaded);
        saveButton.onClick.AddListener(OnSaveClicked);
        mainmenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    private void OnInventoryLoaded(List<SlotSaveData> slots)
    {
        foreach (var slot in slots)
            inventoryManager.LoadSlot(slot);
    }

    private void OnSaveClicked()
    {
        // 1) 현재 슬롯 상태를 JSON으로 직렬화
        var saveData = inventoryManager.GetSaveData();
        string json  = JsonUtility.ToJson(saveData);

        // 2) 비동기로 저장 시작
        dataManager
            .SaveRawJsonAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    saveCompleted = true;
                }
                else
                {
                    Debug.LogError("저장 실패: " + task.Exception);
                }
            });
    }

    private void GoToMainMenu()
    {
        if(!saveCompleted) return;
        
        SceneManager.LoadScene("MainMenu");
    }
    
}