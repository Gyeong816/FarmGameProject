using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    public List<ShopSlot> ShopSlots;
    public List<ItemData> shopItemDatabase;

    private void Awake()
    {
        ShopSlots = new List<ShopSlot>(GetComponentsInChildren<ShopSlot>());
    }
}
