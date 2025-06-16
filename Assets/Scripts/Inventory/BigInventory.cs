using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigInventory : MonoBehaviour
{
    public List<SlotUI> Slots;


    private void Awake()
    {
        Slots = new List<SlotUI>(GetComponentsInChildren<SlotUI>());
    }
}
