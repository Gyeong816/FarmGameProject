using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
 
    [Header("데이터 참조")]
    public ItemData data; 
    
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
            var slot = target.GetComponent<SlotUI>();
            if (slot != null)
            {
                // 2) 이미 슬롯에 있던 아이템이 있다면 원래 자리로 돌려보냄
                if (slot.currentItemUI != null)
                    slot.currentItemUI.ReturnItem();

                // 3) 이 ItemUI를 슬롯에 자식으로 배치
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                canvasGroup.blocksRaycasts = true;

                // 4) 슬롯이 이 ItemUI를 갖도록 갱신
                slot.SetItem(this);
                return;
            }
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
        return data.itemPrefab;
    }
}
