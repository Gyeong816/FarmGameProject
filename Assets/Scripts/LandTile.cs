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
        grassTile.SetActive(true);
        plowedTile.SetActive(false);
        wateredTile.SetActive(false);
    }
    
    public void SetGridPosition(int x, int z)
    {
        gridPos = new Vector2Int(x, z);
        isPlowed = false;
        isWatered = false;
        isPlanted = false;
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