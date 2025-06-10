using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstance : MonoBehaviour
{
    [Header("Crop Data")]
    public CropData cropData;
    
    private int currentStage = 0;
    private GameObject currentModel;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OndayPassed += OnDayPassed;
        }
        UpdateCropModel();
    }
    
    void OnDayPassed(int currentDay)
    {
        if (currentStage < cropData.stagePrefabs.Length - 1)
        {
            currentStage++;
            UpdateCropModel();
        }
    }

    void UpdateCropModel()
    {
        if(currentModel != null)
            Destroy(currentModel);
        
        currentModel = Instantiate(cropData.stagePrefabs[currentStage], transform);
    }
}
