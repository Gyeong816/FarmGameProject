using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  public static InventoryManager Instance;

  [Header("아이템 데이터베이스")] 
  public List<ItemData> itemDatabase;
  
  [Header("인벤토리")]
  public SmallInventory smallInventory;
  public BigInventory bigInventory;

  private void Awake()
  {
    if(Instance == null)
      Instance = this;
    else Destroy(gameObject);
  }

  public void AddItemById(int itemId)
  {
    var data = itemDatabase.Find(x => x.id == itemId);
    if (data == null)
    {
      Debug.LogWarning($"[InventoryManager] ItemData not found for id={itemId}");
      return;
    }
    
    var emptySlot = smallInventory.Slots.Find(s => s.IsEmpty);
    if (emptySlot == null)
    {
      Debug.LogWarning("[InventoryManager] 빈 슬롯이 없습니다.");
      return;
    }
    var go = Instantiate(data.uiPrefab, emptySlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    itemUI.data = data;
    emptySlot.SetItem(itemUI);
    
  }
  
  public void RemoveItemById(int id)
  {
    // 해당 아이템 UI가 들어 있는 슬롯 찾기
    var slot = smallInventory.Slots
      .Find(s => s.currentItemUI != null && s.currentItemUI.data.id == id);
    if (slot == null) return;

    // UI 오브젝트 파괴하고 슬롯 초기화
    Destroy(slot.currentItemUI.gameObject);
    slot.currentItemUI = null;
    smallInventory.DestroyCurrentItem();
  }
  
  
  
}
