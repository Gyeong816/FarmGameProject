using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TMP_Text nameText;  
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GameObject soldOutImage;
    
    public ItemData data; 
    public ItemType itemType;
    
    private Button buyButton;
   
    public int itemCount;
    public bool isSoldOut;
    
    
    public void Init(ItemData data)
    {
        this.data = data;
        itemCount = data.itemCount;
        nameText.text = data.itemName;
        priceText.text = data.price.ToString();
       
        countText.text = itemCount.ToString();
        Enum.TryParse(data.itemType, out itemType);

        buyButton = GetComponent<Button>();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => TradeManager.Instance.RequestPurchase(this));
    }

    public void UpdateItemCount()
    {
        countText.text = itemCount.ToString();
    }

    public void SoldOut()
    {
        buyButton.interactable = false;
        soldOutImage.SetActive(true);
    }
}
