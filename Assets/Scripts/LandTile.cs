// LandTile.cs

using System;
using UnityEngine;

public class LandTile : MonoBehaviour
{
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject plowedTile;
    [SerializeField] private GameObject wateredTile;

    public event Action<LandTile> onPlantRequested;
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

    // Awake 대신 MapManager에서 호출
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
        onPlantRequested?.Invoke(this);
    }
    public void MarkPlanted()
    {
        isPlanted = true;
    }
    
    public Vector2Int GetGridPosition() => gridPos;
}