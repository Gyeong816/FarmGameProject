using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType { Hoe, WateringPot, Sickle, Seed, Crop, None}
    public Canvas canvas;
    public GameObject itemPrefab;
    public ItemType currentItemType;
    public int seedNumber;
    
    private Transform originalParent;
    private Vector2 originalPosition;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (canvas == null)
            canvas = gameObject.GetComponentInParent<Canvas>();
        
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
    
    
    public GameObject GetItemPrefab()
    {
        return itemPrefab;
    }
}
