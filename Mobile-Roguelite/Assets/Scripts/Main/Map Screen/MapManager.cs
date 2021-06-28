using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Genearates the world and maps that we move around in
// Also handles party movement
public class MapManager : Singleton<MapManager>
{
    [Header("Map Settings")]

    public Map[] registeredMaps;

    public int currentMapIndex = 0;
    Vector2Int mapSize = Vector2Int.zero;

    [Header("Map Loading")]

    Tile[,] tiles;

    [SerializeField] Vector2 tileDimensions;

    [SerializeField] ObjectPooler pooler;

    [System.Serializable]
    public struct ColorMapping
    {
        public Tile.TileType type;
        public Color textureColor;
    }

    public List<ColorMapping> mappedColors;

    [Header("Map Panning")]

    [SerializeField] RectTransform mapRect;
    [SerializeField] float mapPanSmoothTime;

    [HideInInspector]
    public Vector2Int playerCoords = Vector2Int.zero;
    Vector2 vel = Vector2.zero;
    

    // Loads a map from a Texture2D
    public void LoadMap(int mapIndex)
    {
        CleanMap();

        currentMapIndex = mapIndex;
        Map map = registeredMaps[mapIndex];

        mapSize = new Vector2Int(map.mapTexture.width, map.mapTexture.height);
        tiles = new Tile[mapSize.x, mapSize.y];

        // Spawn tile objects from texture
        for(int y = 0; y < mapSize.y; y++)
        {
            for(int x = 0; x < mapSize.x; x++)
            {
                Color color = map.mapTexture.GetPixel(x, y);
                if(!color.Equals(Color.black))
                {
                    foreach (ColorMapping cm in mappedColors)
                    {
                        if (cm.textureColor.r == color.r && cm.textureColor.g == color.g && cm.textureColor.b == color.b)
                        {
                            SpawnTile(x, y, cm.type);

                            // Also check where the start tile is
                            if(cm.type == Tile.TileType.Start)
                            {
                                playerCoords = new Vector2Int(x, y);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    void SpawnTile(int x, int y, Tile.TileType type)
    {
        GameObject g = pooler.Retrieve(0);
        Tile t = g.GetComponent<Tile>();

        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * tileDimensions.x, y * tileDimensions.y);

        t.tileType = type;
        tiles[x, y] = t;
    }

    void CleanMap()
    {
        if(tiles != null)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    if(tiles[x, y] != null)
                    {
                        pooler.Return(tiles[x, y].gameObject);
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Pan map to player position
        Vector2 targetPos = -playerCoords * tileDimensions;
        mapRect.anchoredPosition = Vector2.SmoothDamp(mapRect.anchoredPosition, targetPos, ref vel, mapPanSmoothTime);
    }

    public void MoveParty(int dir)
    {
        if(GameManager.Instance.canMove)
        {
            switch (dir)
            {
                case 0: // Up
                    if (Valid(playerCoords.x, playerCoords.y + 1))
                    {
                        playerCoords.y += 1;
                        GameManager.Instance.TriggerTile(tiles[playerCoords.x, playerCoords.y]);
                    }
                    break;
                case 1: // Left
                    if (Valid(playerCoords.x - 1, playerCoords.y))
                    {
                        playerCoords.x -= 1;
                        GameManager.Instance.TriggerTile(tiles[playerCoords.x, playerCoords.y]);
                    }
                    break;
                case 2: // Down
                    if (Valid(playerCoords.x, playerCoords.y - 1))
                    {
                        playerCoords.y -= 1;
                        GameManager.Instance.TriggerTile(tiles[playerCoords.x, playerCoords.y]);
                    }
                    break;
                case 3: // Right
                    if (Valid(playerCoords.x + 1, playerCoords.y))
                    {
                        playerCoords.x += 1;
                        GameManager.Instance.TriggerTile(tiles[playerCoords.x, playerCoords.y]);
                    }
                    break;
            }
        }
    }

    bool Valid(int x, int y)
    {
        return x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y
            && tiles[x, y] != null;
    }
}