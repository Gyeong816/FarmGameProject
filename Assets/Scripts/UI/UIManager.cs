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
    [SerializeField] private CameraController cameraController;

    [Header("상인 상호작용")]
    [SerializeField] private GameObject housePromptUI;
    [SerializeField] private GameObject boxPromptUI;
    [SerializeField] private GameObject shopPromptUI;
    private GameObject currentPromptUI;
    
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    private Transform promptTarget;
    private bool canTrade = false;
    
    public enum PromptType { House, Box, Shop }

    private bool isInventoryOpen = false;
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

        if (Input.GetKeyDown(KeyCode.E) && canTrade)
        {
            OnInven(true);
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
            }
            else
            {
                smallInven.SetActive(false);
                merchantInven.SetActive(true);
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
        
        if (currentPromptUI != null)
        {
            currentPromptUI.SetActive(false);
            currentPromptUI = null;
        }
    }
}
