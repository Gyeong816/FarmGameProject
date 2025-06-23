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
    

    private bool saveCompleted;

    private void Start()
    {
        saveCompleted = false; 
        
        dataSaveManager.LoadGame();
        
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