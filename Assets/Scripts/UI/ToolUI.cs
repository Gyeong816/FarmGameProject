using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public enum ToolUIType { Hoe, WateringPot, Sickle }
    public Canvas canvas;
    public PlayerController player;
    
    public ToolUIType currentTool;
    private Transform originalParent;
    private Vector2 originalPosition;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (canvas == null)
            canvas = gameObject.GetComponentInParent<Canvas>();
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;
        
        if (target.CompareTag("S_Inven"))
        {
            transform.SetParent(target.transform);
            transform.position = target.transform.position;
            canvasGroup.blocksRaycasts = true; 
        }
        else
        {
            ReturnItem();
        }
    }
    
    private void ReturnItem()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        canvasGroup.blocksRaycasts = true; 
    }

    public void SetUITool()
    {
        switch (currentTool)
        {
            case ToolUIType.Hoe:
                player.currentTool = PlayerController.ToolType.Hoe;
                player.SetTool();
                break;
            case ToolUIType.WateringPot:
                player.currentTool = PlayerController.ToolType.WateringPot;
                player.SetTool();
                break;
            case ToolUIType.Sickle:
                player.currentTool = PlayerController.ToolType.Sickle;
                player.SetTool();
                break;
            default:
                break;
        }
        
    }
}