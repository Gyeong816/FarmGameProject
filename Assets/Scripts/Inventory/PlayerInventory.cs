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
        player.currentTool = PlayerController.ToolType.None;
        player.SetTool();
        
        var itemUI = smallSlots[index].GetComponentInChildren<ItemUI>();
        var toolUI = smallSlots[index].GetComponentInChildren<ToolUI>();
        if (itemUI != null)
        {
            var prefab = itemUI.GetItemPrefab();
            heldItemInstance = Instantiate(prefab, playerHandTransform.position, playerHandTransform.rotation, playerHandTransform);
            return;  
        }
        if (toolUI != null)
        {
            toolUI.SetUITool();
            return;
        }
        

    }
}
