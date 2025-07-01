using System;
using UnityEngine;

public class LandTile : MonoBehaviour
{
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject plowedTile;
    [SerializeField] private GameObject wateredTile;
    [SerializeField] private GameObject SelectionPointer;
    
    public Vector2Int gridPos;
    
    public bool isPlanted;
    public bool isPlowed;
    private bool isWatered;
    private bool isRaining;
  
    
    private void Start()
    {
        WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
        grassTile.SetActive(true);
        plowedTile.SetActive(false);
        wateredTile.SetActive(false);
    }
    

    private void OnDisable()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= HandleWeatherChanged;
    }
    
    private void HandleWeatherChanged(bool isNowRaining)
    {
        if (!isPlowed) return;
        isRaining = isNowRaining;
        if (isRaining)
        {
            Water();
        }
        else
        {
            Dry(); 
        }
    }
    
    public void ShowSelection()
    {
        SelectionPointer.SetActive(true);
    }
    public void HideSelection()
    {
        SelectionPointer.SetActive(false);
    }
    public void SetGridPosition(int x, int z)
    {
        gridPos = new Vector2Int(x, z);
    }

    public void Hoe()
    {
        if (isPlowed) return;
        
        
        isPlowed = true;
        grassTile.SetActive(false);
        plowedTile.SetActive(true);
        
        if (isRaining)
            MapManager.Instance.WaterCropAt(this);
    }
    public void Water()
    {
        if (!isPlowed) return;
        isWatered = true;
        plowedTile.SetActive(false);
        wateredTile.SetActive(true);
    }

    public void Dry()
    {
        if (!isPlowed) return;
        isWatered = false;
        plowedTile.SetActive(true);
        wateredTile.SetActive(false);
    }
   

    public void ResetTile()
    {
        isPlowed = false;
        isPlanted = false;
        isWatered = false;
        grassTile.SetActive(true);
        wateredTile.SetActive(false);
        plowedTile.SetActive(false);
    }
    public void MarkPlanted()
    {
        isPlanted = true;
    }
    
    public bool IsWateredThisDay()
    {
        return isWatered;
    }
    public Vector2Int GetGridPosition() => gridPos;
}