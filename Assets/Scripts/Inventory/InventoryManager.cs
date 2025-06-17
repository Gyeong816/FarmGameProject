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
    if(Instance == null)
      Instance = this;
    else Destroy(gameObject);
    
    foreach (var prefab in prefabList)
    {
      if (prefab != null)
        prefabDic[prefab.name] = prefab;
    }
  }

  private async void Start()
  {
    
    var itemDataList = await TsvLoader.LoadTableAsync<ItemData>("ItemTable");

    itemDatabase = itemDataList;
    shopInventory.shopItemDatabase = itemDataList;  
    shopInventory.ShowAllItems();
    
    int index = 0;
    foreach (var data in itemDataList)
    {
      if (index >= 5) break;
      
      GameObject go = Instantiate(itemUIPrefab,  bigInventory.Slots[index].transform);
      ItemUI itemUI = go.GetComponent<ItemUI>();
      itemUI.data = data;
      bigInventory.Slots[index].SetItem(itemUI);
      index++;
    }
    
  }

  
  public GameObject GetPrefab(string key)
  {
    if (prefabDic.TryGetValue(key, out var result))
      return result;
    Debug.LogWarning($"프리팹 키 {key}를 찾을 수 없음");
    return null;
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
    var go = Instantiate(itemUIPrefab, emptySlot.transform);
    var itemUI = go.GetComponent<ItemUI>();
    itemUI.data = data;
    emptySlot.SetItem(itemUI);
  }
  
  public void RemoveItemById(int id)
  {
  
    var slot = smallInventory.Slots.Find(s => s.currentItemUI != null && s.currentItemUI.data.id == id);
    if (slot == null) return;
    
    Destroy(slot.currentItemUI.gameObject);
    slot.currentItemUI = null;
    smallInventory.DestroyCurrentItem();
  }
  
  
  
}
