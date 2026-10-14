using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Tileset _tileset;

    private List<MetatileController> _metatiles;

    private TileData CreateRandomTileData()
    {
        return new TileData{
            spriteID = Random.Range(0, _tileset.Max)
        };
    }

    private MetatileController InstantiateTile(Vector2 position)
    {
        var obj = Instantiate(_prefab, transform);
        obj.transform.position = new Vector3(position.x, position.y, 0) * 2;
        var metatileController = obj.GetComponent<MetatileController>();

        if (metatileController != null)
        {
            metatileController.SetTilesetsAndTileDatas(_tileset, CreateRandomTileData(), CreateRandomTileData(), CreateRandomTileData(), CreateRandomTileData());
        }
        return metatileController;
    }

    private void InstantiateTiles()
    {
        _metatiles = new(new MetatileController[_mapSize.x * _mapSize.y]);
        Vector2 mapSize = _mapSize;
        for (var y = 0; y < _mapSize.y; y++)
        {
            for (var x = 0; x < _mapSize.x; x++)
            {
                var index = x + y * _mapSize.x;
                Vector2 position = new Vector2(x, y) - mapSize / 2 + new Vector2(0.5f, 0.5f);
                _metatiles[index] = InstantiateTile(position);
            }
        }
    }

    private void SwapTiles()
    {
        foreach (var tile in _metatiles)
        {
            tile.SetTilesets(_tileset);
        }
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (_metatiles == null) {
            InstantiateTiles();
        } 
        else
        {
            SwapTiles();
        }
    }
}
