using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Transform playerHandTransform;
    public PlayerController player;
    
    private GameObject heldItemInstance;
    private List<SlotUI> smallSlots;
    private int currentIndex = 0; 
    
    private void Start()
    {
        smallSlots = new List<SlotUI>(GetComponentsInChildren<SlotUI>());
        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }
    
    
    void Update()
    {
        for (int i = 0; i < smallSlots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSmallSlot(i);
        }

        if (!Input.GetKey(KeyCode.C))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                currentIndex = (currentIndex - 1 + smallSlots.Count) % smallSlots.Count;
                SelectSmallSlot(currentIndex);
            }
            else if (scroll < 0f)
            {
                currentIndex = (currentIndex + 1) % smallSlots.Count;
                SelectSmallSlot(currentIndex);
            }   
        }
    }

    private void SelectSmallSlot(int index)
    {
        foreach (var slot in smallSlots)
        {
            slot.ClearHighlight();
        }
        smallSlots[index].SetHighlight();
        
        if (heldItemInstance != null)
            Destroy(heldItemInstance);
        player.currentItem = PlayerController.ItemType.None;
        player.SetItem();
        
        var itemUI = smallSlots[index].GetComponentInChildren<ItemUI>();
        
        if (itemUI != null)
        {
            switch (itemUI.currentItemType)
            {
                case ItemUI.ItemType.Hoe:
                    player.currentItem = PlayerController.ItemType.Hoe;
                    player.SetItem();
                    break;
                
                case ItemUI.ItemType.WateringPot:
                    player.currentItem = PlayerController.ItemType.WateringPot;
                    player.SetItem();
                    break;
                
                case ItemUI.ItemType.Sickle:
                    player.currentItem = PlayerController.ItemType.Sickle;
                    player.SetItem();
                    break;
                
                case ItemUI.ItemType.Seed:
                    player.seedNumber = itemUI.seedNumber;
                    var seedPrefab = itemUI.GetItemPrefab();
                    if (seedPrefab != null)
                    {
                        heldItemInstance = Instantiate(seedPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = PlayerController.ItemType.Seed;
                    }
                    break;
                
                case ItemUI.ItemType.Crop:
                    var cropPrefab = itemUI.GetItemPrefab();
                    if (cropPrefab != null)
                    {
                        heldItemInstance = Instantiate(cropPrefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
                        player.currentItem = PlayerController.ItemType.Crop;
                    }
                    break;
                
                case ItemUI.ItemType.None: 
                    player.currentItem = PlayerController.ItemType.None;
                    break;
                
                default:
                    player.currentItem = PlayerController.ItemType.None;
                    break;
            }     

        }
       
        
    }
    
    
}
