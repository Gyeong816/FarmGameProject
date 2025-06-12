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
    private bool isFenceInstalled;
    
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
    }
    public void Water()
    {
        if (!isPlowed || isWatered) return;
        isWatered = true;
        plowedTile.SetActive(false);
        wateredTile.SetActive(true);
        MapManager.Instance.WaterCropAt(this);
    }

    public void ResetTile()
    {
        isPlanted = false;
        isWatered = false;
        grassTile.SetActive(false);
        wateredTile.SetActive(false);
        plowedTile.SetActive(true);
    }
    public void MarkPlanted()
    {
        isPlanted = true;
    }

    public void MarkFenced()
    {
        isFenceInstalled = true;
    }
    
    public Vector2Int GetGridPosition() => gridPos;
}