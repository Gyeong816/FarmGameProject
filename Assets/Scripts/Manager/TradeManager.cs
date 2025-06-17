using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    [Header("Confirm UI")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Confirm Buttons")]
    [SerializeField] private Button buyButton;    // 구매 확정
    [SerializeField] private Button sellButton;   // 판매 확정
    [SerializeField] private Button cancelButton; // 취소

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI playerDollarText;
    [SerializeField] private TextMeshProUGUI vendorDollarText;
    [SerializeField] private int playerDollar  = 150;
    [SerializeField] private int vendorDollar  = 10000;

    private Action _onConfirm;
    private Action _onCancel;

    private void Awake()
    {
        Instance = this;

        confirmPanel.SetActive(false);
        warningPanel.SetActive(false);

        // 버튼 리스너 정리 & 연결
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnConfirm);

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnConfirm);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void Start()
    {
        UpdateDollarUI();
    }
    
    public void RequestPurchase(ShopItemUI shopItemUI)
    {
        ShowConfirm(
            isSelling:      false,
            itemName:       shopItemUI.data.itemName,
            price:          shopItemUI.data.price,
            confirmAction:  () => ExecutePurchase(shopItemUI),
            cancelAction:   () => { /* 필요 시 추가 */ }
        );
    }

    public void RequestSale(ItemData data)
    {
        ShowConfirm(
            isSelling:      true,
            itemName:       data.itemName,
            price:          data.price,
            confirmAction:  () => ExecuteSale(data),
            cancelAction:   () => { /* 필요 시 추가 */ }
        );
    }

    private void ShowConfirm(bool isSelling, string itemName, int price, Action confirmAction, Action cancelAction)
    {
        itemNameText.text = itemName;
        priceText.text    = $"${price}";

        _onConfirm = confirmAction;
        _onCancel  = cancelAction;
        confirmPanel.SetActive(true);

        buyButton.gameObject.SetActive(!isSelling);
        sellButton.gameObject.SetActive(isSelling);
    }

    private void OnConfirm()
    {
        confirmPanel.SetActive(false);
        _onConfirm?.Invoke();
        UpdateDollarUI();
    }

    private void OnCancel()
    {
        confirmPanel.SetActive(false);
        _onCancel?.Invoke();
    }

    private void ExecutePurchase(ShopItemUI shopItemUI)
    {
        if (playerDollar < shopItemUI.data.price)
        {
            warningPanel.SetActive(true);
            return;
        }
        
        playerDollar -= shopItemUI.data.price;
        vendorDollar += shopItemUI.data.price;
        InventoryManager.Instance.AddItemToBigInventory(shopItemUI.data.id);
        shopItemUI.SoldOut();
    }

    private void ExecuteSale(ItemData data)
    {
        if (vendorDollar < data.price)
        {
            warningPanel.SetActive(true);
            return;
        }

        playerDollar += data.price;
        vendorDollar -= data.price;
        InventoryManager.Instance.RemoveItemById(data.id);
    }

    private void UpdateDollarUI()
    {
        playerDollarText.text = playerDollar.ToString();
        vendorDollarText.text = vendorDollar.ToString();
    }
}
