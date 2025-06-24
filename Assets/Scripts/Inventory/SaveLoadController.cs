using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using System.Threading.Tasks;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private DataSaveManager dataSaveManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject bigInventory;
    

    private bool saveCompleted;

    private async void Start()
    {
        saveCompleted = false; 
        mainmenuButton.interactable = false;
        try
        {
            await inventoryManager.LoadDatabaseAsync();
            await dataSaveManager.LoadGameAsync();

        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadController] 로드 중 예외 발생: {e}");
        }
        
        bigInventory.SetActive(false);
        saveButton.onClick.AddListener(OnSaveClicked);
        mainmenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void OnSaveClicked()
    {
    
        dataSaveManager
            .SaveGameAsync()
            .ContinueWithOnMainThread((Task task) =>
            {
                if (task.IsCompleted)
                {
                    saveCompleted = true; 
                    mainmenuButton.interactable = true;
                    Debug.Log("[SaveLoadController] 저장 완료");
                }
                else
                {
                    Debug.LogError("[SaveLoadController] 저장 실패: " + task.Exception);
                }
            });
    }

    private void GoToMainMenu()
    {
        if (!saveCompleted) return;
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}