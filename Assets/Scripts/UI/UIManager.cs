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
    [SerializeField] private GameObject questPanel;
    
    
    [Header("기본 참조")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button closePauseMenuPanel;
    [SerializeField] private NPCInteractionManager npcInteractionManager;
    
   

    [Header("상인 상호작용")]
    [SerializeField] private GameObject housePromptUI;
    [SerializeField] private GameObject boxPromptUI;
    [SerializeField] private GameObject talkPromptUI;
    [SerializeField] private BigInventory bigInventory;
    
    [Header("버튼 UI")]
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button closeShopButton;
    private GameObject currentPromptUI;
    
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    private Transform promptTarget;
    
    public enum PromptType { House, Box, Npc, None }
    
    private PromptType currentPromptType = PromptType.None;
    public bool IsPanelOpen => isPanelOpen;
    
    private bool isPanelOpen; 
    private bool isSmallPanelOpen;
    private bool isInventoryOpen;
    private bool isMenuPanelOpen;
    private bool isShopPanelOpen;
    private Camera mainCam;
    private int npcId;
    private string npcName;
    private NpcData npcData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        

        mainCam = Camera.main;
        
        closePauseMenuPanel.onClick.AddListener(() => TogglePanel(pauseMenuPanel));
        sleepButton.onClick.AddListener(OnSleep);
        closeShopButton.onClick.AddListener(CloseShopPanel);
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
            if (isSmallPanelOpen||isMenuPanelOpen || isShopPanelOpen) return;
            isInventoryOpen = !isInventoryOpen;
            
            TogglePanel(bigInvenPanel,questPanel);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInventoryOpen || isMenuPanelOpen || isShopPanelOpen || currentPromptType == PromptType.None) return;
            
            isSmallPanelOpen = !isSmallPanelOpen;
            switch (currentPromptType)
            {
                case PromptType.House:
                    TogglePanel(sleepPanel);
                    break;
                case PromptType.Box:
                    //TogglePanel(boxPanel);
                    break;
                case PromptType.Npc:
                    npcInteractionManager.ShowNpcDialogue(npcData, npcData.npcId, 0);
                    TogglePanel(dialoguePanel);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOpen||isSmallPanelOpen || isShopPanelOpen) return;
            
            isMenuPanelOpen = !isMenuPanelOpen;
            TogglePanel(pauseMenuPanel);
        }
    }


    public void OpenShopPanel()
    {
        if (isShopPanelOpen) return;
        isShopPanelOpen = true;

        CloseDialoguePanel();
        TogglePanel(bigInvenPanel,shopInvenPanel);
    }
    
    private void CloseShopPanel()
    {
        if (!isShopPanelOpen) return;
        isShopPanelOpen = false;
        TogglePanel(bigInvenPanel,shopInvenPanel);
    }

    public void CloseDialoguePanel()
    {
        isSmallPanelOpen = false;
        TogglePanel(dialoguePanel);
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

    
    
    public void ShowPromptUI(Transform target, PromptType type, NpcData npcData)
    {
        this.npcData = npcData;
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
                currentPromptUI = talkPromptUI;  
                currentPromptType = PromptType.Npc;
                break;
        }
        currentPromptUI.SetActive(true);
    }
    
    public void HidePromptUI()
    {
        npcData = null;
        promptTarget = null;
        
        if (currentPromptUI != null)
        {
            currentPromptUI.SetActive(false);
            currentPromptUI = null;
            currentPromptType = PromptType.None;
        }
    }
    
    private void OnSleep()
    {
        TimeManager.Instance.SkipNight();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
