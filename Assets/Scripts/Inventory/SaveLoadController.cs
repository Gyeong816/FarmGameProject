using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private Button saveButton;

    private InventoryManager       inventoryManager;
    private InventoryDataManager   dataManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        dataManager      = FindObjectOfType<InventoryDataManager>();

        // 1) 씬 시작 시 서버에서 저장된 슬롯 데이터 로드
        dataManager.LoadInventory(OnInventoryLoaded);

        // 2) 저장 버튼 클릭 시 수동 저장
        saveButton.onClick.AddListener(OnSaveClicked);
    }

    // LoadInventory 콜백
    private void OnInventoryLoaded(List<SlotSaveData> slots)
    {
        foreach (var slot in slots)
        {
            inventoryManager.LoadSlot(slot);
        }
    }

    // 저장 버튼 클릭 핸들러
    private void OnSaveClicked()
    {
        var saveData = inventoryManager.GetSaveData();
        string json  = JsonUtility.ToJson(saveData);
        dataManager.SaveRawJson(json);
        Debug.Log("수동 저장 요청 완료");
    }
}