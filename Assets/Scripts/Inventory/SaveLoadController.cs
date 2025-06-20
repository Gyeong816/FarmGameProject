using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using System.Threading.Tasks;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainmenuButton;

    private bool saveCompleted;

    private void Start()
    {
        saveCompleted = false;
        mainmenuButton.interactable = false;      // 저장 전에는 돌아가기 금지

        // 씬 시작 시 전체 데이터(인벤토리·시간·하늘)를 불러와 복원
        DataSaveManager.Instance.LoadGame();

        saveButton.onClick.AddListener(OnSaveClicked);
        mainmenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void OnSaveClicked()
    {
        // 전체 상태를 비동기로 저장
        DataSaveManager.Instance
            .SaveGameAsync()
            .ContinueWithOnMainThread((Task task) =>
            {
                if (task.IsCompleted)
                {
                    saveCompleted = true;
                    mainmenuButton.interactable = true;  // 저장 완료 후에만 버튼 활성화
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
        SceneManager.LoadScene("MainMenu");
    }
}