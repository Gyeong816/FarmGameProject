using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    public List<ShopSlot> shopSlots;
    public List<ItemData> shopItemDatabase;
    public GameObject shopItemUIPrefab;

    private void Awake()
    {
        shopSlots = new List<ShopSlot>(GetComponentsInChildren<ShopSlot>());
    }
    

    public void ShowItemsByType(params ItemType[] types)
    {
        foreach (var slot in shopSlots)
            slot.ClearSlot();
        
        HashSet<string> typeStrings = new HashSet<string>();
        foreach (var t in types)
            typeStrings.Add(t.ToString());
        
        List<ItemData> filtered = shopItemDatabase.FindAll(data =>
            typeStrings.Contains(data.itemType)
        );
        
        for (int i = 0; i < shopSlots.Count && i < filtered.Count; i++)
        {
            GameObject go = Instantiate(shopItemUIPrefab, shopSlots[i].transform);
            ShopItemUI itemUI = go.GetComponent<ShopItemUI>();
            itemUI.data = filtered[i];
            shopSlots[i].SetItem(itemUI);
        }
    }

    public void ShowAllItems()
    {
        foreach (var slot in shopSlots)
            slot.ClearSlot();

        for (int i = 0; i < shopSlots.Count && i < shopItemDatabase.Count; i++)
        {
            GameObject go = Instantiate(shopItemUIPrefab, shopSlots[i].transform);
            ShopItemUI itemUI = go.GetComponent<ShopItemUI>();
            itemUI.data = shopItemDatabase[i];
            shopSlots[i].SetItem(itemUI);
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
