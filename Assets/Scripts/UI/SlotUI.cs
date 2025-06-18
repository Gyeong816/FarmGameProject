using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    
    public Image highlightSlot;
    private RectTransform highlightRT;
    public ItemUI currentItemUI;
    private void Awake()
    {
        if (highlightSlot != null)
        {
            highlightRT = highlightSlot.rectTransform;  
        }
    }
    
    public bool IsEmpty => currentItemUI == null;
    
    public void SetItem(ItemUI itemUI)
    {
        currentItemUI = itemUI;
        itemUI.originalSlotUI = this;
    }
    public void UnsetItem(ItemUI itemUI)
    {
        currentItemUI = null;
        itemUI.originalSlotUI = null;
    }
    public void SetHighlight()
    {
        highlightSlot.enabled = true;
        highlightRT.SetAsLastSibling();
        Canvas.ForceUpdateCanvases();
    }
    public void ClearHighlight()
    {
        highlightSlot.enabled = false;
    }

    public void CanSell(bool canSell)
    {
        
        if (currentItemUI != null && canSell)
         currentItemUI.canSell = true;
        
        else if (currentItemUI != null && !canSell) 
            currentItemUI.canSell = false;
    }
}
