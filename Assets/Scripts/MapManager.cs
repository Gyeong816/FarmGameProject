using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSize = 2f;
    [SerializeField] private GameObject grassTilePrefab;

    private LandTile[,] tiles;

    void Start()
    {
        tiles = new LandTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = new Vector3(x * tileSize, 0f, z * tileSize);
                GameObject go = Instantiate(grassTilePrefab, worldPos, Quaternion.identity, transform);
                LandTile tile = go.GetComponent<LandTile>();
                tile.Initialize(x, z, this);
                tiles[x, z] = tile;
            }
        }
    }

    public bool InBounds(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }

    public LandTile GetTile(int x, int z)
    {
        return InBounds(x, z) ? tiles[x, z] : null;
    }

    public bool WorldToGrid(Vector3 worldPos, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPos.x / tileSize + 0.5f);
        z = Mathf.FloorToInt(worldPos.z / tileSize + 0.5f);
        return InBounds(x, z);
    }
}