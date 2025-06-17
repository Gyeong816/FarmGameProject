using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
 
    [Header("데이터 참조")]
    public ItemData data; 
    
    [Header("UI 참조")]
    public TMP_Text nameText;   // 아이템 이름 표시
    public TMP_Text countText;
    
    public ItemType currentItem;
    
    private Transform originalParent;
    private Vector2 originalPosition;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvas == null)
            canvas = gameObject.GetComponentInParent<Canvas>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if(nameText == null)
            nameText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        Enum.TryParse(data.itemType, out currentItem);
  
        nameText.text = data.itemName;
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
                if (slot.currentItemUI != null)
                    slot.currentItemUI.ReturnItem();
                
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                canvasGroup.blocksRaycasts = true;
                
                slot.SetItem(this);
                return;
            }
        }
        if (target.CompareTag("B_Inven"))
        {
            var slot = target.GetComponent<SlotUI>();
            if (slot != null)
            {
                if (slot.currentItemUI != null)
                    slot.currentItemUI.ReturnItem();
                
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                canvasGroup.blocksRaycasts = true;
                
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
    
    
  /* public GameObject GetItemPrefab()
    {
       // return data.itemPrefab;
    } */
}
