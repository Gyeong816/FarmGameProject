using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallInventory : MonoBehaviour
{
    public Transform playerHandTransform;
    public PlayerController player;
    
    private GameObject heldItemInstance;
    public List<SlotUI> Slots;
    private int currentIndex = 0; 
    
    private void Start()
    {
        Slots = new List<SlotUI>(GetComponentsInChildren<SlotUI>());
        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }
    
    
    void Update()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }

        if (!Input.GetKey(KeyCode.C))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                currentIndex = (currentIndex - 1 + Slots.Count) % Slots.Count;
                SelectSlot(currentIndex);
            }
            else if (scroll < 0f)
            {
                currentIndex = (currentIndex + 1) % Slots.Count;
                SelectSlot(currentIndex);
            }   
        }
    }
 

    public void DestroyCurrentItem()
    {
        if (heldItemInstance != null)
            Destroy(heldItemInstance);
        player.currentItem = ItemType.None;
        player.SetItem();
    }
    private void SelectSlot(int index)
    {
        foreach (var slot in Slots)
        {
            slot.ClearHighlight();
        }
        Slots[index].SetHighlight();

        DestroyCurrentItem();
        
        var itemUI = Slots[index].GetComponentInChildren<ItemUI>();
        if (itemUI != null)
        {
            
            switch (itemUI.currentItem)
            {
                case ItemType.Hoe:
                    player.currentItem = ItemType.Hoe;
                    player.SetItem();
                    break;
                
                case ItemType.WateringPot:
                    player.currentItem = ItemType.WateringPot;
                    player.SetItem();
                    break;
                
                case ItemType.Sickle:
                    player.currentItem = ItemType.Sickle;
                    player.SetItem();
                    break;
                
                case ItemType.Hammer:
                    player.currentItem = ItemType.Hammer;
                    player.SetItem();
                    break;
                
                case ItemType.Seed:
                    player.itemId = itemUI.data.id;
                    player.seedId = itemUI.data.seedId;
                    var seedPrefab = InventoryManager.Instance.GetPrefab(itemUI.data.prefabKey);
                    if (seedPrefab != null)
                    {
                        heldItemInstance = Instantiate(seedPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = ItemType.Seed;
                    }
                    player.SetItem();
                    break;
                
                case ItemType.Crop:
                    var cropPrefab = InventoryManager.Instance.GetPrefab(itemUI.data.prefabKey);
                    if (cropPrefab != null)
                    {
                        heldItemInstance = Instantiate(cropPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = ItemType.Crop;
                    }
                    player.SetItem();
                    break;
                
                case ItemType.Fence:
                    player.itemId = itemUI.data.id;
                    player.currentItem = ItemType.Fence;
                    var fencePrefab = InventoryManager.Instance.GetPrefab(itemUI.data.prefabKey);
                    if (fencePrefab != null)
                    {
                        heldItemInstance = Instantiate(fencePrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                    }
                    player.SetItem();
                    break;
                
                case ItemType.None: 
                    player.currentItem = ItemType.None;
                    break;
                default:
                    break;   
            
            }
          
        }
         
    }
    
}
