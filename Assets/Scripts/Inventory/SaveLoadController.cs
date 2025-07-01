using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using System.Threading.Tasks;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;    
    [SerializeField] private Slider progressSlider; 
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private DataSaveManager dataSaveManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject bigInventory;
    

    private bool saveCompleted;

    private async void Start()
    { 
        SoundManager.Instance.PlayBgm("BGM_InGameBgm");
        saveCompleted = false; 
        mainmenuButton.interactable = false;
        
        loadingPanel.SetActive(true);
        progressSlider.value = 0f;
        
        loadingPanel.SetActive(true);
        progressSlider.value = 0f;

      
        var progress = new Progress<float>(p => progressSlider.value = p);

        try
        {
            await LoadAllDataAsync(progress);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadController] 로드 중 예외 발생: {e}");
        }
        finally
        {
            bigInventory.SetActive(false);
            loadingPanel.SetActive(false);
        }
        
        saveButton.onClick.AddListener(OnSaveClicked);
        mainmenuButton.onClick.AddListener(GoToMainMenu);
    }

    private async Task LoadAllDataAsync(IProgress<float> progress)
    {
        progress.Report(0.5f);
        await inventoryManager.LoadDatabaseAsync();
        
        progress.Report(0.8f);
        await dataSaveManager.LoadGameAsync();
        
        progress.Report(1f);
        await Task.Delay(100);
    }

    public void OnSaveClicked()
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