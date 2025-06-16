using UnityEngine;

public enum ItemType { Hoe, WateringPot, Sickle, Seed, Crop, Fence, Hammer, None }

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public int      id;           
    public int seedId;
    public ItemType itemType;     
    public string   itemName;     
    public Sprite   icon;         
    public GameObject itemPrefab; 
    public GameObject uiPrefab; 
    public int      price;
}