using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("타일 크기")]
    [SerializeField] public float tileSize = 2f;
    [Header("농작물")]
    [SerializeField] private GameObject[] cropPrefabs;
    
    private readonly Dictionary<Vector2Int, LandTile> tiles = new();
    private Dictionary<Vector2Int, GameObject> plantedCrops = new();
    private void Awake()
    {
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
    
    private void HandlePlantRequest(LandTile tile)
    {
        var prefab = cropPrefabs[0];
        
        var pos = tile.transform.position + Vector3.up * 2f;
        
        var crop = Instantiate(prefab, pos, Quaternion.identity, tile.transform);
        
        tile.MarkPlanted();
        
        plantedCrops[tile.gridPos] = crop;
        
    }
}