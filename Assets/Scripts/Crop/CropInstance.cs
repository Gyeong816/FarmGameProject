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
    public bool isWateredToday;
    private bool isRaining;

    private void Start()
    {
        WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
        TimeManager.Instance.OnDayPassed += OnDayPassed;
        UpdateCropModel();
    }
    
    private void HandleWeatherChanged(bool isNowRaining)
    {
        
        if (isNowRaining)
        {
            isRaining = true;
        }
        else
        {
            isRaining = false;
        }
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
                
                if (currentStage >= 4)
                {
                    isCropRotten = true;
                }
            }        
            
        }
        
        isWateredToday = isRaining;
    }
    
    public void Water()
    {
        isWateredToday = true;
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
