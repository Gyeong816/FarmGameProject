using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventory : MonoBehaviour
{
    [Header("Scroll View")]
    [SerializeField] private ScrollRect scrollRect;
    public List<ItemData> shopItemDatabase;
    public List<ShopItemUI> shopItems;
    public GameObject shopItemUIPrefab;

    public GameObject shopPanel;
    

    public void ShowItemsByType(params ItemType[] types)
    {
        foreach (var itemUI in shopItems.ToArray())
        {
            Destroy(itemUI.gameObject);
            shopItems.Remove(itemUI);
        }
        
        HashSet<string> typeStrings = new HashSet<string>();
        foreach (var t in types)
            typeStrings.Add(t.ToString());
        
        List<ItemData> filtered = shopItemDatabase.FindAll(data =>
            typeStrings.Contains(data.itemType)
        );
        
        foreach (var data in filtered)
        {
            var go     = Instantiate(shopItemUIPrefab, shopPanel.transform);
            var itemUI = go.GetComponent<ShopItemUI>();
            itemUI.Init(data);         
            shopItems.Add(itemUI);
            if (itemUI.data.isSoldOut)
                itemUI.SoldOut();
        }
        
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void ShowAllItems()
    {
        foreach (var item in shopItems.ToArray())
        {
            Destroy(item.gameObject);
            shopItems.Remove(item);
        }

        for (int i = 0;  i < shopItemDatabase.Count; i++)
        {
            GameObject go = Instantiate(shopItemUIPrefab, shopPanel.transform);
            ShopItemUI itemUI = go.GetComponent<ShopItemUI>();
            itemUI.Init(shopItemDatabase[i]);
            shopItems.Add(itemUI);
            if (itemUI.data.isSoldOut)
                itemUI.SoldOut();
        }
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnClickAllButton()
    {
        ShowAllItems();
    }

    public void OnClickToolButton()
    {
        ShowItemsByType(ItemType.Hoe, ItemType.WateringPot, ItemType.Sickle, ItemType.Hammer);
    }

    public void OnClickSeedButton()
    {
        ShowItemsByType(ItemType.Seed);
    }

    public void OnClickFoodButton()
    {
        ShowItemsByType(ItemType.Crop);
    }

    public void OnClickMiscButton()
    {
        ShowItemsByType(ItemType.Fence);
    }
}
