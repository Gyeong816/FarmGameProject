using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstance : MonoBehaviour
{
    [Header("Crop Data")]
    public CropData cropData;

    public int cropNumber;

    public bool canHarvest;
    public int currentStage = 0;
    private GameObject currentModel;
    private bool isWateredToday;

    private void Start()
    {
        
        TimeManager.Instance.OnDayPassed += OnDayPassed;
        UpdateCropModel();
    }
    
    public void Water()
    {
        isWateredToday = true;
        Debug.Log($"{name} 물 주기 완료");
    }
    
    void OnDayPassed()
    {
        if (isWateredToday)
        {
            if (currentStage < cropData.stagePrefabs.Length - 1)
            {
                currentStage++;
                UpdateCropModel();
                
                if (currentStage >= 3)
                {
                    canHarvest = true;
                }
            }    
        }
        else
        {
            Debug.Log($"{name} 물을 안 줘서 성장 불가 ");
        }

        isWateredToday = false;
    }
    
    void UpdateCropModel()
    {
        if(currentModel != null)
            Destroy(currentModel);
        
        currentModel = Instantiate(cropData.stagePrefabs[currentStage], transform);
    }
    
    public void SetStage(int stage)
    {
        currentStage = stage;
        UpdateCropModel();

        canHarvest = (currentStage >= cropData.stagePrefabs.Length - 1);
    }
}
