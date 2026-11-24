using System;
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
            var obj = UnityEngine.Object.Instantiate(prefab, parentTransform);
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
                Vector3 position = new Vector2(x, y) * 2 + (Vector2.one - mapSize);
                int index = x + y * mapSize.x;
                gameObjects[index].transform.localPosition = position;
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
    private void UpdateSelectedTileData(List<MetatileController> metatiles, List<MetatileMapData> metatileDatas, MetatileMemory metatileMemory, int index, int metatileIndex)
    {
        var metatileMapData = metatileDatas[index];
        var metatileSpriteData = metatileMemory.GetData(metatileIndex);
        metatiles[index].UpdateTiles(_tileset, metatileMapData, metatileSpriteData);
    }
    #endregion

    public void InitializeObjects(GameObject prefab, Vector2Int mapSize, Transform transform)
    {
        GameObject groupParent = new("Metatiles");
        groupParent.transform.parent = transform;
        groupParent.transform.localPosition = Vector3.zero;
        InstantiateMetatileObjects(mapSize.x * mapSize.y, prefab, groupParent.transform, out _metatiles, out List<GameObject> gameObjects);
        SetObjectPositions(gameObjects, mapSize);
    }

    public void LoadMapData(MapData mapData, MetatileMemory metatileMemory)
    {
        UpdateTileDatas(_metatiles, mapData.MetatileDatas, metatileMemory);
    }

    public void OverrideSelectedMetatiles(MapData mapData, MetatileMemory metatileMemory, List<Tuple<int, int>> overrides)
    {
        foreach (var (index, metatileIndex) in overrides)
        {
            UpdateSelectedTileData(_metatiles, mapData.MetatileDatas, metatileMemory, index, metatileIndex);
        }
    }
    public void OverrideSelectedMetatile(MapData mapData, MetatileMemory metatileMemory, int index, int metatileIndex)
    {
        Debug.Log($"{index}:{metatileIndex}");
        UpdateSelectedTileData(_metatiles, mapData.MetatileDatas, metatileMemory, index, metatileIndex);
    }
}
