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
    public MapSaveData       map;        // 타일·크롭·펜스 상태
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

[Serializable]
public class FenceSaveData
{
    public float rotationY;      // 펜스 Y축 회전값
}

[Serializable]
public class TileSaveData
{
    public int x, z;                         // 그리드 좌표
    public bool isPlowed;                    // 밭갈기 여부
    public bool isWatered;                   // 오늘 물 준 여부
    public bool isPlanted;                   // 씨앗 심긴 여부

    public int  cropId;                      // 심은 작물 ID
    public int  growthStage;                 // 현재 성장 단계

    public List<FenceSaveData> fences = new List<FenceSaveData>();  
}

[Serializable]
public class MapSaveData
{
    public List<TileSaveData> tiles;
}