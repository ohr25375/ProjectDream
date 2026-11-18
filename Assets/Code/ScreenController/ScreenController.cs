using System.Collections;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [Header("Metatile Controllers")]
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _subScreen;
    private readonly MapGenerator _mainMapGenerator = new();
    private readonly MapGenerator _subMapGenerator = new();
    [SerializeField] private MetatileMemory _metatileMemory;
    [SerializeField] private Tileset _tileset;
    [SerializeField] private GameObject _metatilePrefab;

    [Header("Camera Controls")]
    [SerializeField] private GameObject _camera;
    [SerializeField, Min(0)] private float _lerpTime;
    [SerializeField, Min(0)] private float _delay;

    [Header("Player")]
    [SerializeField] private CollisionTestPlayerController _player;
    [SerializeField] private Vector2Int _playerStartingPosition;

    private Coroutine _coroutine;

    public static ScreenController Instance = null;
    public static MapData MainMapData = null;
    public int _currentMapIndex = 0;
    private WorldMapData CurrentWorldMapData => _worldData.GetWorldMapData(_currentMapIndex);
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private WorldData _worldData;
    [SerializeField] private Direction _direction;

    private void Awake()
    {
        InitializeScreen();
    }
    [ContextMenu("Move Map")]
    private void MoveMap()
    {
        DoMoveMap(_direction);
    }
#endif
    public void InitializeScreen()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found duplicate Screen Controller. Destroying...");
            Destroy(this);
            return;
        }
        Instance = this;
        var startingMapData = _worldData.GetWorldMapData(0).MapData;
        _mainMapGenerator.InitializeObjects(_metatilePrefab, startingMapData.Size, _mainScreen.transform);
        _subMapGenerator.InitializeObjects(_metatilePrefab, startingMapData.Size, _subScreen.transform);
        _subScreen.SetActive(false);
        _mainMapGenerator.SetTileset(_tileset);
        _subMapGenerator.SetTileset(_tileset);
        _player.Initialize(_playerStartingPosition);
        _player.SetMapPositionOffset(GetPositionFromXY(0, 0, startingMapData.Size * 2) - new Vector3(0.5f, 0.5f, 0));
        SetStaticMapData(startingMapData);
    }
    public static Vector3 GetPositionFromXY(int x, int y, Vector2Int size)
    {
        return new Vector3(x - size.x / 2 + 0.5f, y - size.y / 2 + 0.5f, 0);
    }

    private void SetStaticMapData(MapData mapData)
    {
        MainMapData = mapData;
        _mainMapGenerator.LoadMapData(mapData, _metatileMemory);
    }

    private IEnumerator MoveCamera(Vector2 direction, int nextMapIndex)
    {
        _player.SetCanMove(false);
        var nextMap = _worldData.GetWorldMapData(nextMapIndex);
        var nextMapData = nextMap.MapData;
        var baseTarget = new Vector2(nextMapData.Size.x * direction.x, nextMapData.Size.y * direction.y);
        var target = 2 * baseTarget;
        _subScreen.SetActive(true);
        _subMapGenerator.LoadMapData(nextMapData, _metatileMemory);
        _subScreen.transform.localPosition = (Vector3)target;
        float currentTime = 0;
        Vector3 initialPosition = _camera.transform.position;
        Vector3 targetPosition = new(target.x, target.y, initialPosition.z);
        var waitFrame = new WaitForFixedUpdate();
        var playerCurrentPosition = _player.CurrentPosition;
        yield return new WaitForSeconds(_delay);
        while (currentTime < _lerpTime)
        {
            _camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime / _lerpTime);
            _player.SetCurrentPosition(Vector3.Lerp(playerCurrentPosition, playerCurrentPosition + (Vector3)direction * 2, currentTime / _lerpTime));
            yield return waitFrame;
            currentTime += Time.deltaTime;
        }
        _camera.transform.position = initialPosition;
        _player.AddCurrentPosition((new Vector2(nextMapData.Size.x * -direction.x, nextMapData.Size.y * -direction.y)) * 2);
        _player.SetCanMove(true);
        _subScreen.SetActive(false);
        SetStaticMapData(nextMapData);
        _currentMapIndex = nextMapIndex;
        _coroutine = null;
    }
    public void Initialize(MapData initialMapData)
    {
        _mainMapGenerator.LoadMapData(initialMapData, _metatileMemory);
    }
    public static void MoveMap(Direction direction)
    {
        Instance.DoMoveMap(direction);
    }
    private void DoMoveMap(Direction direction)
    {
        if (!_worldData.IsNextMapReachable(_currentMapIndex, direction)) return;
        if (_coroutine != null) return;
        var nextIndex = _worldData.GetNextIndex(_currentMapIndex, direction);
        _coroutine = StartCoroutine(MoveCamera(direction.ToVector2(), nextIndex));
    }
}
