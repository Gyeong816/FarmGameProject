using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TMP_Text nameText;  
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject soldOutImage;
    
    public ItemData data; 
    public ItemType itemType;
    
    private Button buyButton;
   
    public bool isSoldOut;

    private void Awake()
    {
        buyButton = GetComponent<Button>();
    }
    private void Start()
    {
        
        nameText.text = data.itemName;
        priceText.text = data.price.ToString();

        Enum.TryParse(data.itemType, out itemType);
       
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => TradeManager.Instance.RequestPurchase(this));
        
    }


    public void SoldOut()
    {
        buyButton.interactable = false;
        soldOutImage.SetActive(true);
    }
}
