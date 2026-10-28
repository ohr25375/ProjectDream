using System.Collections.Generic;
using UnityEngine;

public class CreateCollisionTestMap : MonoBehaviour
{
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private float _threshold = 0.5f;
    private List<bool> _collisionTiles;
    private List<GameObject> _collisionTileObjects;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private CollisionTestPlayerController _playerController;

    public static List<bool> CollisionTiles;
    public static Vector2Int MapSize;

    private void Awake()
    {
        InitializeMapGrid(_mapSize);
        CreateMap(_mapSize);
        _playerController.SetMapPositionOffset(GetPositionFromXY(0, 0, _mapSize) - new Vector3(0.5f, 0.5f, 0));
    }
    [ContextMenu("Regenerate Map")]
    private void RegenerateMap()
    {
        ResizeMapGrid(_mapSize);
        CreateMap(_mapSize);
    }
    private Vector3 GetPositionFromXY(int x, int y, Vector2Int size)
    {
        return new Vector3(x - size.x / 2 + 0.5f, y - size.y / 2 + 0.5f, 0);
    }

    private void InitializeMapGrid(Vector2Int mapSize)
    {
        _collisionTileObjects = new(new GameObject[mapSize.x * mapSize.y]);
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var obj = Instantiate(_tilePrefab, transform);
                obj.transform.position = GetPositionFromXY(x, y, mapSize);
                obj.SetActive(false);
                var index = y * mapSize.x + x;
                _collisionTileObjects[index] = obj;
            }
        }
    }
    private void ResizeMapGrid(Vector2Int mapSize)
    {
        foreach (var obj in _collisionTileObjects)
        {
            obj.SetActive(false);
        }
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var index = y * mapSize.x + x;
                if (index >= _collisionTileObjects.Count)
                {
                    _collisionTileObjects[index] = Instantiate(_tilePrefab, transform);
                    _collisionTileObjects[index].SetActive(false);
                }
                _collisionTileObjects[index].transform.position = GetPositionFromXY(x, y, mapSize);
            }
        }
    }
    private void CreateMap(Vector2Int mapSize)
    {
        _collisionTiles = new(new bool[mapSize.x * mapSize.y]);
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var index = y * mapSize.x + x;
                bool value = Random.value > _threshold;
                if (y == 0 || y == mapSize.y -1 || x == 0 || x == mapSize.x -1)
                {
                    value = true;
                }
                _collisionTiles[index] = value;
                _collisionTileObjects[index].SetActive(_collisionTiles[index]);
            }
        }
        CollisionTiles = _collisionTiles;
        MapSize = mapSize;
    }
}
