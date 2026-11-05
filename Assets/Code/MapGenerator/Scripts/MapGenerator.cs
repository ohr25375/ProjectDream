using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    private Tileset _tileset;
    private List<MetatileController> _metatiles;

    public void SetTileset(Tileset tileset) => _tileset = tileset;

    #region INTERNALS
    private void InstantiateMetatileObjects(int count, GameObject prefab, Transform parentTransform, out List<MetatileController> metatileControllers, out List<GameObject> gameObjects)
    {
        metatileControllers = new(new MetatileController[count]);
        gameObjects = new(new GameObject[count]);
        for (var i = 0; i < count; i++)
        {
            var obj = Object.Instantiate(prefab, parentTransform);
            if (obj.TryGetComponent<MetatileController>(out var metatileController))
            {
                metatileControllers[i] = metatileController;
            }
            gameObjects[i] = obj;
        }
    }
    private void SetObjectPositions(List<GameObject> gameObjects, Vector2Int mapSize)
    {
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                Vector2 position = new Vector2(x, y) - (Vector2)mapSize / 2 + new Vector2(0.5f, 0.5f);
                int index = x + y * mapSize.x;
                gameObjects[index].transform.localPosition = (Vector3)position * 2;
            }
        }
    }
    private void UpdateTileDatas(List<MetatileController> metatiles, List<MetatileMapData> metatileDatas, MetatileMemory metatileMemory)
    {
        for (var i = 0; i < metatiles.Count; i++)
        {
            var metatileMapData = metatileDatas[i];
            var metatileSpriteData = metatileMemory.GetData(metatileMapData.metatileIndex);
            metatiles[i].UpdateTiles(_tileset, metatileMapData, metatileSpriteData);
        }
    }
    #endregion

    public void InitializeObjects(GameObject prefab, Vector2Int mapSize, Transform mapCenter)
    {
        InstantiateMetatileObjects(mapSize.x * mapSize.y, prefab, mapCenter, out _metatiles, out List<GameObject> gameObjects);
        SetObjectPositions(gameObjects, mapSize);
    }

    public void LoadMapData(MapData mapData, MetatileMemory metatileMemory)
    {
        UpdateTileDatas(_metatiles, mapData.MetatileDatas, metatileMemory);
    }
}
