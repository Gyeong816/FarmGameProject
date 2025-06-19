using System;
using System.Collections.Generic;


public enum InventoryType
{
    Small,
    Big
}

// 슬롯 단위로 저장할 데이터
[Serializable]
public class SlotSaveData
{
    public InventoryType inventoryType;  // Small or Big
    public int           slotIndex;      // 슬롯 번호 (0부터)
    public int           itemId;         // 아이템 고유 ID
    public int           count;          // 수량
}

[Serializable]
public class InventorySaveData
{
    // 저장된 슬롯 정보 리스트
    public List<SlotSaveData> savedInvenDatas;
}
