using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyResultsPanel : MonoBehaviour
{
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Button continueButton;
  
    
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
        // 1) 기존 슬롯 초기화
        foreach (var slot in Slots)
            slot.Clear();  // SlotUI 내부에 Clear() 메서드가 있다고 가정

        // 2) 오늘 획득한 아이템 목록
        var today = inventoryManager.GetDailyAcquisitions();
        
        // 3) 슬롯에 아이템 채워 넣기
        for (int i = 0; i < today.Count && i < Slots.Count; i++)
        {
            var (itemId, count) = today[i];
            var data = inventoryManager.GetItemData(itemId);
            if (data == null) continue;

            // 슬롯 위치에 아이템 UI 인스턴트
            var go = Instantiate(itemUIPrefab, Slots[i].transform);
            var ui = go.GetComponent<ItemUI>();
            var icon = inventoryManager.GetIcon(data.iconKey);

            ui.Init(data, icon);
            ui.SetCount(count);

            // SlotUI 쪽에 새로 SetItem 같은 메서드가 있으면 호출
            Slots[i].SetItem(ui);
        }

        // 4) (옵션) 화면 띄우기
        gameObject.SetActive(true);

        // 5) 다음 날을 위해 기록 초기화
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
