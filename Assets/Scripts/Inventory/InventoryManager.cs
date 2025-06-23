using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  public static InventoryManager Instance;

  [Header("아이템 데이터베이스")] 
  public List<ItemData> itemDatabase;
  private Dictionary<int, ItemData> dataDict;
  
  [Header("인벤토리")]
  public SmallInventory smallInventory;
  public BigInventory bigInventory;
  public ShopInventory shopInventory;
  
  [Header("프리팹")]
  public GameObject itemUIPrefab;
  public List<GameObject> prefabList;
  private Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
    
    foreach (var prefab in prefabList)
    {
      if (prefab != null)
        prefabDic[prefab.name] = prefab;
    }
    
    dataDict = new Dictionary<int, ItemData>();
  }

  private async void Start()
  {
    
    var itemDataList = await TsvLoader.LoadTableAsync<ItemData>("ItemTable");

    itemDatabase = itemDataList;
    
    dataDict.Clear();
    foreach (var data in itemDatabase)
    {
      dataDict[data.id] = data;
    }
    
    shopInventory.shopItemDatabase = itemDataList;  
    shopInventory.ShowAllItems();
    
  }


  public InventorySaveData GetSaveData()
  {
    var save = new InventorySaveData {
      savedInvenDatas = new List<SlotSaveData>()};

    for (int i = 0; i < smallInventory.Slots.Count; i++)
    {
      var ui = smallInventory.Slots[i].currentItemUI;
      if (ui != null)
      {
        save.savedInvenDatas.Add(new SlotSaveData {
          inventoryType = InventoryType.Small,
          slotIndex     = i,
          itemId        = ui.data.id,
          count         = ui.itemCount
        });
      }
    }
    for (int i = 0; i < bigInventory.Slots.Count; i++)
    {
      var ui = bigInventory.Slots[i].currentItemUI;
      if (ui != null)
      {
        save.savedInvenDatas.Add(new SlotSaveData {
          inventoryType = InventoryType.Big,
          slotIndex     = i,
          itemId        = ui.data.id,
          count         = ui.itemCount
        });
      }
    }
    
    return save;
  }
  
  

  public void AddItemToSmallInventory(int itemId, int amount)
  {
    var data = itemDatabase.Find(x => x.id == itemId);
    if (data == null)
    {
      Debug.LogWarning($"[InventoryManager] ItemData not found for id={itemId}");
      return;
    }
    
    foreach (var slot in smallInventory.Slots)
    {
      var existing = slot.currentItemUI;
      if (existing != null && existing.data.id == itemId)
      {
        existing.AddItemCount(amount);
        return;
      }
    }
    
    var emptySlot = smallInventory.Slots.Find(s => s.IsEmpty);
    if (emptySlot == null)
    {
      AddItemToBigInventory(itemId, amount);
      return;
    }
    
    var go = Instantiate(itemUIPrefab, emptySlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    emptySlot.SetItem(itemUI);
    itemUI.Init(data);
    itemUI.AddItemCount(amount);
  }
  
  public void AddItemToBigInventory(int itemId, int amount)
  {
    var data = itemDatabase.Find(x => x.id == itemId);
    if (data == null)
    {
      Debug.LogWarning($"[InventoryManager] ItemData not found for id={itemId}");
      return;
    }
    foreach (var slot in bigInventory.Slots)
    {
      var existing = slot.currentItemUI;
      if (existing != null && existing.data.id == itemId)
      {
        existing.AddItemCount(amount);
        return;
      }
    }
    
    var emptySlot = bigInventory.Slots.Find(s => s.IsEmpty);
    if (emptySlot == null)
    {
      Debug.LogWarning("[InventoryManager] 빈 슬롯이 없습니다.");
      return;
    }
    var go = Instantiate(itemUIPrefab, emptySlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    emptySlot.SetItem(itemUI);
    itemUI.Init(data);
    itemUI.AddItemCount(amount);
  }

  public void SubtractItemToBigInventory(int itemId, int amount)
  {
    var slot = bigInventory.Slots.Find(s => s.currentItemUI != null && s.currentItemUI.data.id == itemId);
    if (slot == null) return;
    
    var itemUI = slot.currentItemUI;
    itemUI.SubtractItemCount(amount);
    
    if(itemUI.itemCount <= 0)
    {
      Destroy(slot.currentItemUI.gameObject);
      slot.currentItemUI = null;
    }
  }
  
  

  public void SubtractItemFromSmallInventory(int itemId, int amount)
  {
    var slot = smallInventory.Slots.Find(s => s.currentItemUI != null && s.currentItemUI.data.id == itemId);
    if (slot == null) return;
    
    var itemUI = slot.currentItemUI;
    
    itemUI.SubtractItemCount(amount);
    
    if (itemUI.itemCount <= 0)
    {
      Destroy(slot.currentItemUI.gameObject);
      slot.currentItemUI = null;
      smallInventory.DestroyCurrentItem();
    }
  }
  public GameObject GetPrefab(string key)
  {
    if (prefabDic.TryGetValue(key, out var result))
      return result;
    Debug.LogWarning($"프리팹 키 {key}를 찾을 수 없음");
    return null;
  }
  
  public ItemData GetItemData(int id)
  {
    if (dataDict.TryGetValue(id, out var data))
      return data;
    return null;
  }
  
  
  
  public void ClearAllSlots()
  {
    void Clear(List<SlotUI> slots)
    {
      foreach (var slot in slots)
      {
        if (slot.currentItemUI != null)
        {
          Destroy(slot.currentItemUI.gameObject);
          slot.currentItemUI = null;
        }
      }
    }

    Clear(smallInventory.Slots);
    Clear(bigInventory.Slots);
  }


  public void LoadFromSave(InventorySaveData saveData)
  {
    // 1) 이전에 표시된 모든 아이템 UI를 지우고
    ClearAllSlots();

    // 2) 저장된 슬롯 정보만큼 LoadSlot 호출
    foreach (var slot in saveData.savedInvenDatas)
      LoadSlot(slot);
  }
  
  public void LoadSlot(SlotSaveData data)
  {
    var slots = (data.inventoryType == InventoryType.Small) 
      ? smallInventory.Slots : bigInventory.Slots;
    
    var targetSlot = slots[data.slotIndex];
    
    if (targetSlot.currentItemUI != null)
      Destroy(targetSlot.currentItemUI.gameObject);
    
    var go     = Instantiate(itemUIPrefab, targetSlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    var itemdata = itemDatabase.Find(x => x.id == data.itemId);
    
    itemUI.Init(itemdata);
    itemUI.SetCount(data.count);
    targetSlot.SetItem(itemUI);
  }
}
