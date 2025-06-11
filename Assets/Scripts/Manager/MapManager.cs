using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    [Header("타일 크기")]
    [SerializeField] public float tileSize = 2f;
    [Header("농작물")]
    [SerializeField] private GameObject[] cropPrefabs;
    
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
    
    public void PlantCropAt(LandTile tile, int CropNumber)
    {
        var prefab = cropPrefabs[CropNumber];

        var pos = tile.transform.position + Vector3.up * 2f;

        var cropGO = Instantiate(prefab, pos, Quaternion.identity, tile.transform);

        var crop = cropGO.GetComponent<CropInstance>();
        
        tile.MarkPlanted();

        plantedCrops[tile.gridPos] = crop;
    }

    public void WaterCropAt(LandTile tile)
    {
        if (plantedCrops.TryGetValue(tile.gridPos, out var crop) && crop != null)
        {
            crop.Water();
        }
    }
    
}