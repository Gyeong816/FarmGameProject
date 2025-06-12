using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    
    [Header("농작물 프리팹")]
    [SerializeField] private GameObject[] seedPrefabs;
    [Header("펜스 프리팹")]
    [SerializeField] private GameObject fencePrefabs;
    
    private float tileSize = 2f;
    private Dictionary<Vector2Int, LandTile> tiles;
    private Dictionary<Vector2Int, CropInstance> plantedCrops = new();
    private void Awake()
    {
        Instance = this;
        
        tiles = new Dictionary<Vector2Int, LandTile>();
        foreach (var tile in GetComponentsInChildren<LandTile>())
        {
            int x = Mathf.RoundToInt(tile.transform.position.x / tileSize);
            int z = Mathf.RoundToInt(tile.transform.position.z / tileSize);
            var key = new Vector2Int(x, z);

            tiles[key] = tile;
            tile.SetGridPosition(x, z);
            
        }
    }

    public bool WorldToGrid(Vector3 worldPos, out int x, out int z)
    {
        x = Mathf.RoundToInt(worldPos.x / tileSize);
        z = Mathf.RoundToInt(worldPos.z / tileSize);
        return tiles.ContainsKey(new Vector2Int(x, z));
    }

    public LandTile GetTile(int x, int z)
    {
        tiles.TryGetValue(new Vector2Int(x, z), out var tile);
        return tile;
    }

    public LandTile GetTileAtWorldPos(Vector3 worldPos)
    {
        if (WorldToGrid(worldPos, out int x, out int z))
            return GetTile(x, z);
        return null;
    }
    
    public void PlantCropAt(LandTile tile, int seedId, int itemId)
    {
        if (!tile.isPlowed || tile.isPlanted) 
            return;
        
        tile.MarkPlanted();
        var prefab = seedPrefabs[seedId];
        var pos = tile.transform.position + Vector3.up * 2f;
        var seedGO = Instantiate(prefab, pos, Quaternion.identity, tile.transform);
        var seed = seedGO.GetComponent<CropInstance>();
        
        plantedCrops[tile.gridPos] = seed;
        
        InventoryManager.Instance.RemoveItemById(itemId);
    }

    public void BuildFenceAt(LandTile tile)
    {
        if (tile.isPlanted) 
            return;
        
        tile.MarkFenced();
        
        
    }

    public void HarvestCropAt(LandTile tile)
    {
        if (!plantedCrops.TryGetValue(tile.gridPos, out var seed) || seed == null) 
            return;
        if (!seed.canHarvest) 
            return;

        InventoryManager.Instance.AddItemById(seed.cropData.itemId);
        Destroy(seed.gameObject);
        plantedCrops.Remove(tile.gridPos);
        tile.ResetTile();
    }
    
    public void WaterCropAt(LandTile tile)
    {
        if (plantedCrops.TryGetValue(tile.gridPos, out var crop) && crop != null)
        {
            crop.Water();
        }
    }
    
}