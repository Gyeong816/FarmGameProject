using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    
    
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

        InventoryManager.Instance.AddItemToSmallInventory(seed.cropData.itemId,1);
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
    
    public void WaterAllPlowedTiles()
    {
        // (1) 타일 중 plowed 상태인 것만 Water() 호출
        foreach (var tile in tiles.Values)
        {
            if (tile.isPlowed && !tile.IsWateredThisDay())
                tile.Water();
        }

        // (2) 이미 심어진 작물에도 Water()
        foreach (var crop in plantedCrops.Values)
        {
            if (crop != null)
                crop.Water();
        }

        Debug.Log("[MapManager] Plowed tiles have been watered.");
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
                cropId      = tile.isPlanted ? plantedCrops[kv.Key].cropData.itemId : 0,
                growthStage = tile.isPlanted ? plantedCrops[kv.Key].currentStage : 0,
                fences      = new List<FenceSaveData>()
            };

            // Tag 대신 'Fence' 스크립트 컴포넌트를 찾아서 회전값 저장
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
        // (1) 기존 모든 타일 초기화
        foreach (var kv in tiles.Values)
        {
            kv.ResetTile();  // plowed=false, planted=false, water=false
            // 필요 시 fence 제거 로직도 추가
        }
        plantedCrops.Clear();

        // (2) 저장된 데이터대로 복원
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
                // CropInstance 생성 및 단계 설정
                var prefab = seedPrefabs.First(p => p.GetComponent<CropInstance>().cropData.itemId == entry.cropId);
                var go     = Instantiate(prefab, tile.transform.position + Vector3.up*2f, Quaternion.identity, tile.transform);
                var crop   = go.GetComponent<CropInstance>();
                crop.SetStage(entry.growthStage);
                plantedCrops[key] = crop;
            }

            // 펜스 여러 개 복원
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