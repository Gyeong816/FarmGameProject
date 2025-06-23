using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("기본 UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject merchantInven;
    [SerializeField] private GameObject smallInven;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button closePauseMenuPanel;
   

    [Header("상인 상호작용")]
    [SerializeField] private GameObject housePromptUI;
    [SerializeField] private GameObject boxPromptUI;
    [SerializeField] private GameObject shopPromptUI;
    [SerializeField] private BigInventory bigInventory;
    
    [Header("수면 UI")]
    [SerializeField] private Button sleepButton;
    
    private GameObject currentPromptUI;
    
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    private Transform promptTarget;
    private bool canTrade;
    private bool canSleep;
    private bool canOpenBox;
    private bool openMenu;
    private bool openSleepPanel;
    
    public enum PromptType { House, Box, Shop }

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen;
    private Camera mainCam;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        mainCam = Camera.main;
        closePauseMenuPanel.onClick.AddListener(OnPauseMenuPanel);
        sleepButton.onClick.AddListener(OnSleep);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryToggle();
        UpdatePromptUIPosition();
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnInven(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canTrade)
            {
                OnInven(true);
            }
            if (canSleep)
            {
                OnSleepPanel();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseMenuPanel();
        }
    }

    private void OnSleepPanel()
    {
        if (!openSleepPanel)
        {
            cameraController?.IsInventoryOpen();
            openSleepPanel = true;
            sleepPanel.SetActive(true);
            
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            cameraController?.IsInventoryClose();
            openSleepPanel = false;
            sleepPanel.SetActive(false);
            
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    private void OnPauseMenuPanel()
    {
        if (!openMenu)
        {
            cameraController?.IsInventoryOpen();
            openMenu = true;
            pauseMenuPanel.SetActive(true);
            
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            cameraController?.IsInventoryClose();
            openMenu = false;
            pauseMenuPanel.SetActive(false);
            
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    private void OnInven(bool isTrading)
    {
        if (!isInventoryOpen)
        {
            cameraController?.IsInventoryOpen();
            isInventoryOpen = true;
            inventoryUI.SetActive(true);

            if (!isTrading)
            {
                merchantInven.SetActive(false);
                smallInven.SetActive(true);
                bigInventory.CanSell(false);
            }
            else
            {
                smallInven.SetActive(true);
                merchantInven.SetActive(true);
                bigInventory.CanSell(true);
            }
               
            
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            cameraController?.IsInventoryClose();
            isInventoryOpen = false;
            inventoryUI.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    

    private void UpdatePromptUIPosition()
    {
        if (promptTarget == null || currentPromptUI == null)
            return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(promptTarget.position + worldOffset);

        if (screenPos.z > 0)
        {
            currentPromptUI.transform.position = screenPos;
        }
        else
        {
            currentPromptUI.SetActive(false);
        }
    }

    public void ShowPromptUI(Transform target, PromptType type)
    {
    
        promptTarget = target;
        
        switch (type)
        {
            case PromptType.House:
                currentPromptUI = housePromptUI;
                canSleep = true;
                break;
            case PromptType.Box:
                currentPromptUI = boxPromptUI;
                break;
            case PromptType.Shop:
                currentPromptUI = shopPromptUI;
                canTrade = true;
                break;
        }
        
        currentPromptUI.SetActive(true);
    }
    

    public void HidePromptUI()
    {
        promptTarget = null;
        canTrade = false;
        canSleep = false;
        
        if (currentPromptUI != null)
        {
            currentPromptUI.SetActive(false);
            currentPromptUI = null;
        }
    }
    
    private void OnSleep()
    {
        // 2) 시간 스킵
        TimeManager.Instance.SkipNight();
        

        // 3) 게임 재시작(시간 정상 진행)
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Player slept. Time skipped to next morning.");
    }

}
