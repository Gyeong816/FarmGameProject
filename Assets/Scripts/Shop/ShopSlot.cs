using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private ShopItemUI currentItemUI;

    public void SetItem(ShopItemUI itemUI)
    {
        currentItemUI = itemUI;
    }

    public void ClearSlot()
    {
        if (currentItemUI != null)
        {
            Destroy(currentItemUI.gameObject);
            currentItemUI = null;
        }
    }
}
