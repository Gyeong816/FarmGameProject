using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("기본 UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private CameraController cameraController;

    [Header("상인 상호작용")]
    [SerializeField] private GameObject merchantPromptUI; // 상인 근처 이미지
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    private Transform merchantTarget;
    private bool canTrade = false;

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
        merchantPromptUI.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryToggle();
        UpdateMerchantPromptPosition();
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnInven();
        }

        if (Input.GetKeyDown(KeyCode.E) && canTrade)
        {
            OpenTradeUI();
        }
    }

    private void OnInven()
    {
        if (!isInventoryOpen)
        {
            cameraController?.IsInventoryOpen();
            isInventoryOpen = true;
            inventoryUI.SetActive(true);
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

    private void OpenTradeUI()
    {
        OnInven();
    }

    private void UpdateMerchantPromptPosition()
    {
        if (merchantTarget == null || merchantPromptUI == null)
            return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(merchantTarget.position + worldOffset);

        if (screenPos.z > 0)
        {
            merchantPromptUI.transform.position = screenPos;
        }
        else
        {
            merchantPromptUI.SetActive(false);
        }
    }

    public void ShowMerchantPrompt(Transform target)
    {
        merchantTarget = target;
        canTrade = true;
        merchantPromptUI.SetActive(true);
    }

    public void HideMerchantPrompt()
    {
        merchantTarget = null;
        canTrade = false;
        merchantPromptUI.SetActive(false);
    }
}
