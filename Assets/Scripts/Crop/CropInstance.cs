using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstance : MonoBehaviour
{
    [Header("Crop Data")]
    public CropData cropData;
    
    public bool canHarvest;
    public bool isCropRotten;
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
    }
    
    void OnDayPassed()
    {
        if (isWateredToday || canHarvest)
        {
            if (currentStage < cropData.stagePrefabs.Length - 1)
            {
                currentStage++;
                UpdateCropModel();
                
                if (currentStage >= 3)
                {
                    canHarvest = true;
                }
                
                if (currentStage >= 4)
                {
                    isCropRotten = true;
                }
            }    
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
