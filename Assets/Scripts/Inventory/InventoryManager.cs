using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  public static InventoryManager Instance;

  [Header("아이템 데이터베이스")] 
  public List<ItemData> itemDatabase;
  private Dictionary<int, ItemData> dataDict;
  private Dictionary<int, int> dailyAcquisitions;
  
  [Header("인벤토리")]
  public SmallInventory smallInventory;
  public BigInventory bigInventory;
  public ShopInventory shopInventory;
  
  [Header("프리팹")]
  public GameObject itemUIPrefab;
  public List<GameObject> prefabList;
  private Dictionary<string, GameObject> prefabDic;
  public List<Sprite> iconList;                
  private Dictionary<string, Sprite> iconDic;
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
    
    iconDic = new Dictionary<string, Sprite>();
    prefabDic = new Dictionary<string, GameObject>();
    dataDict = new Dictionary<int, ItemData>();
    dailyAcquisitions = new Dictionary<int,int>();
    
    foreach (var prefab in prefabList)
    {
      if (prefab != null)
        prefabDic[prefab.name] = prefab;
    }
    foreach (var sp in iconList)
    {
      if (sp != null )
        iconDic[sp.name] = sp;
    }
    
  }
  
  

  public async Task LoadDatabaseAsync()
  {
   
    var itemDataList = await TsvLoader.LoadTableAsync<ItemData>("ItemTable");
    itemDatabase = itemDataList;
    
    dataDict.Clear();
    foreach (var data in itemDatabase)
    {
      dataDict[data.id] = data;
    }

    // 3) 상점 UI 셋업 (필요하다면)
    shopInventory.shopItemDatabase = itemDataList;  
    shopInventory.ShowAllItems();

    // 4) 디버그
    var allIds = itemDatabase.Select(x => x.id).ToArray();
    Debug.Log($"[InventoryManager] DB 아이템 ID 목록: {string.Join(",", allIds)}");
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
        RecordAcquisition(itemId, amount);
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
    var iconSprite = GetIcon(data.iconKey);
    itemUI.Init(data,iconSprite);
    itemUI.AddItemCount(amount);
    
    RecordAcquisition(itemId, amount);
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
        RecordAcquisition(itemId, amount);
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
    var iconSprite = GetIcon(data.iconKey);
    itemUI.Init(data,iconSprite);
    itemUI.AddItemCount(amount);
    
    RecordAcquisition(itemId, amount);
  }
  public void RecordAcquisition(int itemId, int amount)
  {
    if (dailyAcquisitions.ContainsKey(itemId))
      dailyAcquisitions[itemId] += amount;
    else
      dailyAcquisitions.Add(itemId, amount);
  }
  
  
  
  public void SubtractItemFromBigInventory(int itemId, int amount)
  {
    
    foreach (var slot in bigInventory.Slots)
    {
      var existing = slot.currentItemUI;
      if (existing != null && existing.data.id == itemId)
      {   
        existing.SubtractItemCount(amount);
        if (existing.itemCount <= 0)
        {
          Destroy(slot.currentItemUI.gameObject);
          slot.currentItemUI = null;
        }
        return;
      }
    }
    
  }
  
  

  public void SubtractItemFromSmallInventory(int itemId, int amount)
  {
    
    foreach (var slot in smallInventory.Slots)
    {
      var existing = slot.currentItemUI;
      if (existing != null && existing.data.id == itemId)
      {   
        existing.SubtractItemCount(amount);
        if (existing.itemCount <= 0)
        {
          smallInventory.DestroyCurrentItem();
          Destroy(slot.currentItemUI.gameObject);
          slot.currentItemUI = null;
        }
        return;
      }
    }
    
    
  }
  
  
  
  public bool HasItem(int itemId, int amount)
  {
    int total = 0;

    // Small 인벤토리에서 개수 합산
    foreach (var slot in smallInventory.Slots)
      if (slot.currentItemUI != null && slot.currentItemUI.data.id == itemId)
        total += slot.currentItemUI.itemCount;

    // Big 인벤토리도 필요하다면 합산
    foreach (var slot in bigInventory.Slots)
      if (slot.currentItemUI != null && slot.currentItemUI.data.id == itemId)
        total += slot.currentItemUI.itemCount;

    return total >= amount;
  }
  
  
  public GameObject GetPrefab(string key)
  {
    if (prefabDic.TryGetValue(key, out var result))
      return result;
    Debug.LogWarning($"프리팹 키 {key}를 찾을 수 없음");
    return null;
  }
  
  public Sprite GetIcon(string iconKey)
  {
    if (iconDic.TryGetValue(iconKey, out var sprite))
      return sprite;

    Debug.LogWarning($"[InventoryManager] 아이콘 키 '{iconKey}'를 찾을 수 없습니다.");
    return null;
  }

  public string GetItemName(int itemId)
  {
    if (dataDict.TryGetValue(itemId, out var data))
      return data.itemName;

    Debug.LogWarning($"[InventoryManager] 아이템 이름을 찾을 수 없습니다. id={itemId}");
    return string.Empty;
  }
  
  public ItemData GetItemData(int id)
  {
    if (dataDict.TryGetValue(id, out var data))
      return data;
    return null;
  }
  public List<(int itemId, int count)> GetDailyAcquisitions()
  {
    return dailyAcquisitions
      .Where(kv => kv.Value > 0)
      .Select(kv => (kv.Key, kv.Value))
      .ToList();
  }
  public void ResetDailyAcquisitions()
  {
    dailyAcquisitions.Clear();
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

    foreach (var slotData in saveData.savedInvenDatas)
    {
      LoadSlot(slotData);
    }
  }
  
  public void LoadSlot(SlotSaveData data)
  {
    var slots = (data.inventoryType == InventoryType.Small)
      ? smallInventory.Slots
      : bigInventory.Slots;

    // 2) 인덱스 검사 (범위 벗어나면 스킵)
    if (data.slotIndex < 0 || data.slotIndex >= slots.Count)
    {
      Debug.LogWarning($"[InventoryManager] LoadSlot: 잘못된 slotIndex={data.slotIndex}");
      return;
    }

    var targetSlot = slots[data.slotIndex];

    // 3) 기존 아이템 제거
    targetSlot.Clear();

    // 4) 빈 슬롯이면 리턴
    if (data.itemId <= 0 || data.count <= 0)
      return;

    // 5) 아이템 데이터 조회
    var itemData = itemDatabase.Find(x => x.id == data.itemId);
    if (itemData == null)
    {
      Debug.LogWarning($"[InventoryManager] LoadSlot: 잘못된 itemId={data.itemId}");
      return;
    }

    // 6) UI 생성 및 초기화
    var go     = Instantiate(itemUIPrefab, targetSlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    var iconSprite = GetIcon(itemData.iconKey);    
    itemUI.Init(itemData, iconSprite);  
    itemUI.SetCount(data.count);
    targetSlot.SetItem(itemUI);
  }
}
