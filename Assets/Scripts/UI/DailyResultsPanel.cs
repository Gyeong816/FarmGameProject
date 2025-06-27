using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyResultsPanel : MonoBehaviour
{
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private TradeManager tradeManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text coinText;


    
    private List<SlotUI> Slots;
    private void Awake()
    {
        Slots = new List<SlotUI>(GetComponentsInChildren<SlotUI>());
        continueButton.onClick.AddListener(OnContinueButton);
    }

    private void OnEnable()
    {
        Time.timeScale       = 0f;
        Cursor.lockState     = CursorLockMode.Confined;
        Cursor.visible       = true;
        ShowDailyResults();
    }

    private void ShowDailyResults()
    {
        int playerCoin = tradeManager.GetCoinCount();
        int dayCount = timeManager.GetCurrentDay();
        coinText.text = playerCoin.ToString();
        dayText.text = $"Day {dayCount}";
            
        foreach (var slot in Slots)
            slot.Clear();  

        
        var today = inventoryManager.GetDailyAcquisitions();
        
       
        for (int i = 0; i < today.Count && i < Slots.Count; i++)
        {
            var (itemId, count) = today[i];
            var data = inventoryManager.GetItemData(itemId);
            if (data == null) continue;
            
            var go = Instantiate(itemUIPrefab, Slots[i].transform);
            var ui = go.GetComponent<ItemUI>();
            var icon = inventoryManager.GetIcon(data.iconKey);

            ui.Init(data, icon);
            ui.SetCount(count);
            
            Slots[i].SetItem(ui);
        }
        
        gameObject.SetActive(true);
        
        inventoryManager.ResetDailyAcquisitions();
    }
    
    public void OnContinueButton()
    {
        TimeManager.Instance.SkipNight();
        Time.timeScale       = 1f;
        Cursor.lockState     = CursorLockMode.Locked;
        Cursor.visible       = false;
        
        gameObject.SetActive(false);
    }
}
