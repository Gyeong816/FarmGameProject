using UnityEngine;

public enum ItemType { Hoe, WateringPot, Sickle, Seed, Crop, None }

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public int      id;           // 씨앗·작물 구분 번호
    public ItemType itemType;     // Seed or Crop
    public string   itemName;     // 에디터용 식별명
    public Sprite   icon;         // 인벤토리 UI 아이콘
    public GameObject worldPrefab; // 손에 들거나 드랍할 프리팹
}