using System;

[Serializable]
public class ItemData
{
    public int id { get; set; }
    public int seedId { get; set; }
    public string itemType { get; set; }
    public string itemName { get; set; }
    public string prefabKey { get; set; }
    public string uiPrefabKey { get; set; }
    public int price { get; set; }

    [NonSerialized] public bool isSoldOut = false;
}