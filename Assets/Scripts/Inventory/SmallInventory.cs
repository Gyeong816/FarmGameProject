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
        player.currentItem = PlayerController.ItemType.None;
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
            
            var data = itemUI.data;
            switch (data.itemType)
            {
                case ItemType.Hoe:
                    player.currentItem = PlayerController.ItemType.Hoe;
                    player.SetItem();
                    break;
                
                case ItemType.WateringPot:
                    player.currentItem = PlayerController.ItemType.WateringPot;
                    player.SetItem();
                    break;
                
                case ItemType.Sickle:
                    player.currentItem = PlayerController.ItemType.Sickle;
                    player.SetItem();
                    break;
                
                case ItemType.Seed:
                    player.itemId = data.id;
                    player.seedId = data.seedId;
                    var seedPrefab = itemUI.GetItemPrefab();
                    if (seedPrefab != null)
                    {
                        heldItemInstance = Instantiate(seedPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = PlayerController.ItemType.Seed;
                    }
                    break;
                
                case ItemType.Crop:
                    var cropPrefab = itemUI.GetItemPrefab();
                    if (cropPrefab != null)
                    {
                        heldItemInstance = Instantiate(cropPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = PlayerController.ItemType.Crop;
                    }
                    break;
                
                case ItemType.Fence:
                    var fencePrefab = itemUI.GetItemPrefab();
                    if (fencePrefab != null)
                    {
                        heldItemInstance = Instantiate(fencePrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = PlayerController.ItemType.Crop;
                    }
                    break;
                
                case ItemType.None: 
                    player.currentItem = PlayerController.ItemType.None;
                    break;
                default:
                    break;   
            
            }
            
        }
        
    }
    
}
