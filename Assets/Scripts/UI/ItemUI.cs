using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler 
{
    public Canvas canvas;
 
    [Header("데이터 참조")]
    public ItemData data; 
    
    [Header("UI 참조")]
    public TMP_Text nameText;   // 아이템 이름 표시
    public TMP_Text countText;
    public SlotUI originalSlotUI;
    public ItemType currentItem;
    public Image iconImage; 
    
    public int itemCount;
    public bool canSell;
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
    
    public void Init(ItemData data, Sprite iconSprite)
    {
        this.data = data;
        canSell = true;
        Enum.TryParse(data.itemType, out currentItem);
        itemCount       = 0;
        nameText.text = data.itemName;
        countText.text = itemCount.ToString();
        iconImage.sprite = iconSprite;
    }
    public void AddItemCount(int count)
    {
        itemCount += count;
        countText.text = itemCount.ToString();
    }

    public void SetCount(int count)
    {
        itemCount = count;
        countText.text = itemCount.ToString();
    }
    public void SubtractItemCount(int count)
    {
        itemCount -= count;
        countText.text = itemCount.ToString();
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
        
        var targetItem = target?.GetComponentInParent<ItemUI>();
        if (targetItem != null && targetItem != this && targetItem.data.id == this.data.id)
        {
            // 같은 아이템끼리 합치기
            targetItem.AddItemCount(this.itemCount);
            originalSlotUI.UnsetItem(this);
            Destroy(gameObject);
            return;
        }
        
        
        if (target.CompareTag("S_Inven"))
        {
            var slot = target.GetComponent<SlotUI>();
            if (slot != null)
            {
                if (slot.currentItemUI != null)
                    slot.currentItemUI.ReturnItem();
                
                originalSlotUI.UnsetItem(this);
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                canvasGroup.blocksRaycasts = true;
                
                slot.SetItem(this);
                originalSlotUI = slot;
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
                
                originalSlotUI.UnsetItem(this);
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                canvasGroup.blocksRaycasts = true;
                
                slot.SetItem(this);
                originalSlotUI = slot;
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
    
    
  public void OnPointerClick(PointerEventData eventData)
  {
      if (canSell)
      {
          TradeManager.Instance.RequestSale(this); 
      }
  }
}
