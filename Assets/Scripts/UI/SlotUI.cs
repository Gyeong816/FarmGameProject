using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    
    public Image highlightSlot;
    private RectTransform highlightRT;

    private void Awake()
    {
        highlightRT = highlightSlot.rectTransform;
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
}
