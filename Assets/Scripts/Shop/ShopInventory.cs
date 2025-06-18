using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    //public List<ShopSlot> shopSlots;
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
        
        for (int i = 0; i < filtered.Count; i++)
        {
            GameObject go = Instantiate(shopItemUIPrefab, shopPanel.transform);
            ShopItemUI itemUI = go.GetComponent<ShopItemUI>();
            itemUI.data = filtered[i];
            shopItems.Add(itemUI);
            
            if (itemUI.data.isSoldOut)
                itemUI.SoldOut();
            
        }
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
            itemUI.data = shopItemDatabase[i];
            shopItems.Add(itemUI);
            
            if (itemUI.data.isSoldOut)
                itemUI.SoldOut();
        }
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
