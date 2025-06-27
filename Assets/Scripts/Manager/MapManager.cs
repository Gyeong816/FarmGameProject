using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    
    [SerializeField] private SmallInventory smallInventory;
    [Header("농작물 프리팹")]
    [SerializeField] private GameObject[] seedPrefabs;
    [Header("펜스 프리팹")]
    [SerializeField] private GameObject fencePrefabs;
    [Header("펜스 프리뷰")]
    private GameObject fencePreviewInstance;
    [SerializeField] private GameObject fencePreviewPrefab;
    private float previewRotationY = 0f;
    public float PreviewRotationY => previewRotationY;
    
    private float tileSize = 2f;
    private Dictionary<Vector2Int, LandTile> tiles;
    private Dictionary<Vector2Int, CropInstance> plantedCrops = new();
    
    private Dictionary<int, GameObject> seedPrefabMap;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        tiles = new Dictionary<Vector2Int, LandTile>();
        foreach (var tile in GetComponentsInChildren<LandTile>())
        {
            int x = Mathf.RoundToInt(tile.transform.position.x / tileSize);
            int z = Mathf.RoundToInt(tile.transform.position.z / tileSize);
            var key = new Vector2Int(x, z);

            tiles[key] = tile;
            tile.SetGridPosition(x, z);
            
        }
        seedPrefabMap = new Dictionary<int, GameObject>();
        foreach (var prefab in seedPrefabs)
        {
            var ci = prefab.GetComponent<CropInstance>();
            int id = ci.cropData.cropId;
            if (!seedPrefabMap.ContainsKey(id))
                seedPrefabMap.Add(id, prefab);
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
        Debug.Log("dfa");
        tile.MarkPlanted();
        var prefab = seedPrefabs[seedId];
        var pos = tile.transform.position + Vector3.up * 2f;
        var seedGO = Instantiate(prefab, pos, Quaternion.identity, tile.transform);
        var seed = seedGO.GetComponent<CropInstance>();
        
        plantedCrops[tile.gridPos] = seed;
        
        if (tile.IsWateredThisDay())
            seed.Water();
        InventoryManager.Instance.SubtractItemFromSmallInventory(itemId, 1);
    }

    public void BuildFenceAt(LandTile tile, float rotationY, int itemId)
    {
        if (tile.isPlanted) return;

        Vector3 pos = tile.transform.position;
        Instantiate(fencePrefabs,
            pos,
            Quaternion.Euler(0, rotationY, 0),
            tile.transform);
        InventoryManager.Instance.SubtractItemFromSmallInventory(itemId, 1);
    }

    public void ShowFencePreview(LandTile tile)
    {
        if (tile == null)
        {
            HideFencePreview();
            return;
        }

        Vector3 pos = tile.transform.position;
        if (fencePreviewInstance == null)
        {
            fencePreviewInstance = Instantiate(
                fencePreviewPrefab,
                pos,
                Quaternion.Euler(0, previewRotationY, 0),
                tile.transform
            );
        }
        else
        {
            fencePreviewInstance.transform.SetParent(tile.transform);
            fencePreviewInstance.transform.position = pos;
            fencePreviewInstance.transform.rotation = Quaternion.Euler(0, previewRotationY, 0);
        }
    }
    
    public void RotateFencePreview()
    {
        previewRotationY = (previewRotationY + 90f) % 360f;
        if (fencePreviewInstance != null)
            fencePreviewInstance.transform.rotation = Quaternion.Euler(0, previewRotationY, 0);
    }

    public void HideFencePreview()
    {
        if (fencePreviewInstance != null)
        {
            Destroy(fencePreviewInstance);
            fencePreviewInstance = null;
        }
    }
    
    
    
    public void HarvestCropAt(LandTile tile)
    {
        if (!plantedCrops.TryGetValue(tile.gridPos, out var seed) || seed == null) 
            return;
        if (!seed.canHarvest) 
            return;

        if (seed.isCropRotten)
        {
            InventoryManager.Instance.AddItemToSmallInventory(26,seed.cropData.amount);
        }
        else
        {
            InventoryManager.Instance.AddItemToSmallInventory(seed.cropData.cropId,seed.cropData.amount);
        }
        
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
    
    
    // 저장 및 로드
    
    public MapSaveData GetSaveData()
    {
        var data = new MapSaveData { tiles = new List<TileSaveData>() };

        foreach (var kv in tiles)
        {
            var tile = kv.Value;
            var entry = new TileSaveData {
                x           = kv.Key.x,
                z           = kv.Key.y,
                isPlowed    = tile.isPlowed,
                isWatered   = tile.IsWateredThisDay(),
                isPlanted   = tile.isPlanted,
                cropId      = tile.isPlanted ? plantedCrops[kv.Key].cropData.cropId : 0,
                growthStage = tile.isPlanted ? plantedCrops[kv.Key].currentStage : 0,
                fences      = new List<FenceSaveData>()
            };
            
            foreach (var fence in tile.GetComponentsInChildren<Fence>())
            {
                entry.fences.Add(new FenceSaveData {
                    rotationY = fence.transform.eulerAngles.y
                });
            }

            data.tiles.Add(entry);
        }

        return data;
    }
    public void LoadFromSave(MapSaveData data)
    {
     
        foreach (var kv in tiles.Values)
        {
            kv.ResetTile();  
        }
        plantedCrops.Clear();

 
        foreach (var entry in data.tiles)
        {
            var key = new Vector2Int(entry.x, entry.z);
            if (!tiles.TryGetValue(key, out var tile)) continue;

            if (entry.isPlowed)
                tile.Hoe();
            if (entry.isWatered)
                tile.Water();
            if (entry.isPlanted)
            {
                tile.MarkPlanted();
                if (seedPrefabMap.TryGetValue(entry.cropId, out var prefab))
                {
                    var go   = Instantiate(prefab, tile.transform.position + Vector3.up * 2f, Quaternion.identity, tile.transform);
                    var crop = go.GetComponent<CropInstance>();
                    crop.SetStage(entry.growthStage);
                    plantedCrops[key] = crop;
                }
            }
            
            foreach (var f in entry.fences)
            {
                Instantiate(fencePrefabs,
                    tile.transform.position,
                    Quaternion.Euler(0, f.rotationY, 0),
                    tile.transform);
            }
        }
    }
    
}