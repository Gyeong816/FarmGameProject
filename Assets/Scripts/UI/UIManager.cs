using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("기본 Panel")]
    [SerializeField] private GameObject shopInvenPanel;
    [SerializeField] private GameObject bigInvenPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject backGround;
    
    
    [Header("기본 참조")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button closePauseMenuPanel;
    [SerializeField] private DialogueManager dialogueManager;
    
   

    [Header("상인 상호작용")]
    [SerializeField] private GameObject housePromptUI;
    [SerializeField] private GameObject boxPromptUI;
    [SerializeField] private GameObject shopPromptUI;
    [SerializeField] private BigInventory bigInventory;
    
    [Header("버튼 UI")]
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;
    private GameObject currentPromptUI;
    
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    private Transform promptTarget;
    
    public enum PromptType { House, Box, Npc, None }
    
    private PromptType currentPromptType = PromptType.None;
    public bool IsPanelOpen => isPanelOpen;
    
    private bool isPanelOpen; 
    private Camera mainCam;
    private int npcId;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        

        mainCam = Camera.main;
        
        closePauseMenuPanel.onClick.AddListener(() => TogglePanel(pauseMenuPanel));
        sleepButton.onClick.AddListener(OnSleep);
        confirmButton.onClick.AddListener(() => TogglePanel(dialoguePanel));
        openShopButton.onClick.AddListener(OpenShopPanel);
        closeShopButton.onClick.AddListener(() => TogglePanel(bigInvenPanel,shopInvenPanel));
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            TogglePanel(bigInvenPanel);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (currentPromptType)
            {
                case PromptType.House:
                    TogglePanel(sleepPanel);
                    break;
                case PromptType.Box:
                    //TogglePanel(boxPanel);
                    break;
                case PromptType.Npc:
                    dialogueManager.ShowNpcDialogue(npcId, 0);
                    TogglePanel(dialoguePanel);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel(pauseMenuPanel);
        }
    }


    public void OpenShopPanel()
    {
        TogglePanel(dialoguePanel);
        TogglePanel(bigInvenPanel,shopInvenPanel);
    }
    
    private void TogglePanel(params GameObject[] panels)
    {
        if (!isPanelOpen)
        {
            isPanelOpen = true;
            
            foreach (var p in panels)
                p.SetActive(true);
            backGround.SetActive(true);
            cameraController?.IsInventoryOpen();
            Time.timeScale       = 0f;
            Cursor.lockState     = CursorLockMode.Confined;
            Cursor.visible       = true;
        }
        else
        {
            isPanelOpen = false;
            
            foreach (var p in panels)
                p.SetActive(false);
            backGround.SetActive(false);
            cameraController?.IsInventoryClose();
            Time.timeScale       = 1f;
            Cursor.lockState     = CursorLockMode.Locked;
            Cursor.visible       = false;
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

    
    
    public void ShowPromptUI(Transform target, PromptType type, int npcId)
    {
        this.npcId = npcId;
        promptTarget = target;
        switch (type)
        {
            case PromptType.House:
                currentPromptUI = housePromptUI;
                currentPromptType = PromptType.House;
                break;
            case PromptType.Box:
                currentPromptUI = boxPromptUI;
                currentPromptType = PromptType.Box;
                break;
            case PromptType.Npc:
                currentPromptUI = shopPromptUI;  
                currentPromptType = PromptType.Npc;
                break;
        }
        currentPromptUI.SetActive(true);
    }
    
    public void HidePromptUI()
    {
        promptTarget = null;
        npcId = 0;
        
        if (currentPromptUI != null)
        {
            currentPromptUI.SetActive(false);
            currentPromptUI = null;
        }
    }
    
    private void OnSleep()
    {
        TimeManager.Instance.SkipNight();
        MapManager.Instance.WaterAllPlowedTiles();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
