using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI 참조")]
    public TMP_Text nameText;  
    public TMP_Text priceText;
    
    public ItemData data; 
    public ItemType itemType;
    private void Start()
    {
        nameText.text = data.itemName;
        priceText.text = data.price.ToString();
        
        if (!Enum.TryParse(data.itemType, out itemType))
        {
            itemType = ItemType.None; 
        }
    }
    
}
