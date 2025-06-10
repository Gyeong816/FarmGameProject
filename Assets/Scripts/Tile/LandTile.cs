using System;
using UnityEngine;

public class LandTile : MonoBehaviour
{
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject plowedTile;
    [SerializeField] private GameObject wateredTile;
    
    public Vector2Int gridPos;
    
    private bool isPlanted;
    private bool isPlowed;
    private bool isWatered;
    
    private void Start()
    {
        TimeManager.Instance.OnDayPassed += OnDayPassed;
        grassTile.SetActive(true);
        plowedTile.SetActive(false);
        wateredTile.SetActive(false);
    }

    void OnDayPassed()
    {
        if (isPlowed)
        {
            wateredTile.SetActive(false);
            plowedTile.SetActive(true);
            isWatered = false;
        }
        else
        {
            grassTile.SetActive(true);
        }
 
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
    }
    public void Water()
    {
        if (!isPlowed || isWatered) return;
        isWatered = true;
        plowedTile.SetActive(false);
        wateredTile.SetActive(true);
        MapManager.Instance.WaterCropAt(this);
    }
    
    public void Plant()
    {
        if (!isPlowed || isPlanted) return;
        MapManager.Instance.PlantCropAt(this);
    }
    public void MarkPlanted()
    {
        isPlanted = true;
    }
    
    public Vector2Int GetGridPosition() => gridPos;
}