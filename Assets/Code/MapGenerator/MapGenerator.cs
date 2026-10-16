using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapData _mapData;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Tileset _tileset;
    [SerializeField] private MetatileMemory _metatileMemory;

    private List<MetatileController> _metatiles;

    //private TileData CreateRandomTileData()
    //{
    //    return new TileData{
    //        spriteID = Random.Range(0, _tileset.Max)
    //    };
    //}
    private MetatileController InstantiateTile(Vector2 position, MetatileMapData metatileMapData, MetatileSpriteData metatileSpriteData)
    {
        var obj = Instantiate(_prefab, transform);
        obj.transform.localPosition = new Vector3(position.x, position.y, 0) * 2;
        var metatileController = obj.GetComponent<MetatileController>();

        if (metatileController != null)
        {
            metatileController.UpdateTiles(_tileset, metatileMapData, metatileSpriteData);
        }
        return metatileController;
    }
    private void InstantiateTiles()
    {
        var mapSize = _mapData.Size;
        _metatiles = new(new MetatileController[_mapData.MaxTileCount]);
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var index = x + y * mapSize.x;
                Vector2 position = new Vector2(x, y) - (Vector2)mapSize / 2 + new Vector2(0.5f, 0.5f);
                var metatileMapData = _mapData.MetatileDatas[index];
                _metatiles[index] = InstantiateTile(position, metatileMapData, _metatileMemory.GetData(metatileMapData.metatileIndex));
            }
        }
    }
    private void SetMetatileDatas()
    {
        for (var i = 0; i < _mapData.MaxTileCount; i++)
        {
            _metatiles[i].SetTileDatas(new(_mapData.MetatileDatas[i],_metatileMemory.GetData(_mapData.MetatileDatas[i].metatileIndex)));
            _metatiles[i].SetPalettes(_mapData.MetatileDatas[i].palette);
            _metatiles[i].UpdateTileRenderers();
        }
    }
    public void LoadMapData(MapData mapData)
    {
        _mapData = mapData;
        if (_metatiles == null)
        {
            InstantiateTiles();
        }
        SetMetatileDatas();
    }
    [ContextMenu("Load MapData")]
    private void EditorLoadMapData()
    {
        LoadMapData(_mapData);
    }
}
