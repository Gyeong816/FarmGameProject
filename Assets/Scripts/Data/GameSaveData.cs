using System;
using System.Collections.Generic;


public enum InventoryType
{
    Small,
    Big
}

[Serializable]
public class GameSaveData
{
    public InventorySaveData inventory;  // 인벤토리 슬롯 정보
    public TimeSaveData      time;       // 날짜·시간 정보
    public SkySaveData       sky;        // 하늘 상태 정보
}


[Serializable]
public class SlotSaveData
{
    public InventoryType inventoryType;  
    public int           slotIndex;     
    public int           itemId;        
    public int           count;      
}


[Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> savedInvenDatas;
}


[Serializable]
public class TimeSaveData
{
    public int   day;       
    public float timeOfDay;  
}

[Serializable]
public class SkySaveData
{
    public string phase;  
}