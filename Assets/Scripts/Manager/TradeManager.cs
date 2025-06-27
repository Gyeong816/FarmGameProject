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
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Confirm Buttons")]
    [SerializeField] private Button buyButton;    
    [SerializeField] private Button sellButton;   
    [SerializeField] private Button cancelButton; 
    [SerializeField] private Button plusButton; 
    [SerializeField] private Button minusButton; 
    [SerializeField] private TMP_Text tradeCountText;

    [Header("Money")]
    [SerializeField] private TMP_Text playerDollarText;
    [SerializeField] private TMP_Text vendorDollarText;
    [SerializeField] private int playerDollar  = 150;
    [SerializeField] private int vendorDollar  = 10000;

    private Action _onConfirm;
    private Action _onCancel;

    private int tradeCount;
    private int tradePrice;
    private int itemCount;
    private int itemPrice;

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
        
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(AddTradeCount);
        
        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(SubtractTradeCount);
        
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
            itemCount:      shopItemUI.itemCount,
            confirmAction:  () => ExecutePurchase(shopItemUI),
            cancelAction:   () => { /* 필요 시 추가 */ }
        );
    }

    public void RequestSale(ItemUI itemUI)
    {
        ShowConfirm(
            isSelling: true,
            itemName: itemUI.data.itemName,
            price:  itemUI.data.price,
            itemCount:   itemUI.itemCount,
            confirmAction:  () => ExecuteSale(itemUI),
            cancelAction:   () => { /* 필요 시 추가 */ }
        );
    }

    private void ShowConfirm(bool isSelling, string itemName, int price, int itemCount, Action confirmAction, Action cancelAction)
    {
        tradeCount = 1;
        itemPrice = price;
        tradePrice = itemPrice;
        itemNameText.text = itemName;
        this.itemCount = itemCount;
        tradeCountText.text = tradeCount.ToString();
        priceText.text    = itemPrice.ToString();
        
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
        var data = shopItemUI.data;
        if (playerDollar < tradePrice)
        {
            warningPanel.SetActive(true);
            return;
        }
        
        playerDollar -= tradePrice;
        vendorDollar += tradePrice;
        
        InventoryManager.Instance.AddItemToBigInventory(shopItemUI.data.id, tradeCount);
        shopItemUI.itemCount -= tradeCount;
        shopItemUI.UpdateItemCount();
        if (shopItemUI.itemCount <= 0)
        {
            data.isSoldOut = true;
            shopItemUI.SoldOut();
        }
    }

    private void ExecuteSale(ItemUI itemUI)
    {
        if (vendorDollar < tradePrice)
        {
            warningPanel.SetActive(true);
            return;
        }
        playerDollar += tradePrice;
        vendorDollar -= tradePrice;
        
        InventoryManager.Instance.SubtractItemFromBigInventory(itemUI.data.id, tradeCount);
    }

    private void UpdateDollarUI()
    {
        playerDollarText.text = playerDollar.ToString();
        vendorDollarText.text = vendorDollar.ToString();
    }

    public void AddTradeCount()
    {
        if (tradeCount < itemCount)
        {
            tradeCount++;
            tradePrice += itemPrice;
            tradeCountText.text = tradeCount.ToString();
            priceText.text = tradePrice.ToString();   
        }
    }

    public void SubtractTradeCount()
    {
        if (tradeCount > 1)
        {
            tradeCount--;
            tradePrice -= itemPrice;
            tradeCountText.text = tradeCount.ToString();
            priceText.text = tradePrice.ToString();
        }
    }

    public void AddRewardCoin(int amount)
    {
        playerDollar += amount;
        UpdateDollarUI();
    }

    public int GetCoinCount()
    {
        return playerDollar;
    }
}
