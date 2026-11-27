using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    [SerializeField] private EntitySpawner _entitySpawner;

    private Coroutine _coroutine;

    public static ScreenController Instance = null;
    public static MapData MainMapData = null;
    private int _currentMapIndex = 0;
    private int _currentWorldIndex = 0;
    public int CurrentMapIndex => _currentMapIndex;
    private WorldMapData CurrentWorldMapData => CurrentWorldData.GetWorldMapData(_currentMapIndex);
    [Header("Experimental")]
    private List<Dictionary<Vector2Int, MapEventTrigger>> WorldMapEvents => _worldMapEventsList[_currentWorldIndex];
    private List<List<Dictionary<Vector2Int, MapEventTrigger>>> _worldMapEventsList;
    private List<IndexableDirectionalStruct<bool>> ExitLockStates => _exitLockStateList[_currentWorldIndex];
    private List<List<IndexableDirectionalStruct<bool>>> _exitLockStateList;
    private List<int> _keysInInventoryPerWorld;
    public bool HasKey => _keysInInventoryPerWorld[_currentWorldIndex] > 1;
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private List<WorldData> _worldDatas;
    public WorldData CurrentWorldData => _worldDatas[_currentWorldIndex];
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
    [ContextMenu("Give Key")]
    private void GiveKey()
    {
        _keysInInventoryPerWorld[_currentWorldIndex]++;
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
        var startingMapData = CurrentWorldMapData.MapData;
        _worldMapEventsList = new(new List<Dictionary<Vector2Int, MapEventTrigger>>[_worldDatas.Count]);
        for (var w = 0; w < _worldDatas.Count; w++)
        {
            _worldMapEventsList[w] = new(new Dictionary<Vector2Int, MapEventTrigger>[_worldDatas[w].Count]);
            for (var i = 0; i < _worldDatas[w].Count; i++)
            {
                WorldMapEvents[i] = new();
                var mapEventTriggers = _worldDatas[w].GetWorldMapData(i).MapEventTriggers;
                foreach (var trigger in mapEventTriggers)
                {
                    WorldMapEvents[i].Add(trigger.Position, trigger.MapEventTrigger.Clone());
                }
            }
        }
        _keysInInventoryPerWorld = new(new int[_worldDatas.Count]);

        _exitLockStateList = new(new List<IndexableDirectionalStruct<bool>>[_worldDatas.Count]);
        for (var w = 0; w < _worldDatas.Count; w++)
        {
            _exitLockStateList[w] = new(new IndexableDirectionalStruct<bool>[_worldDatas[w].Count]);
            for (var i = 0; i < _worldDatas[w].Count; i++)
            {
                var worldMapData = _worldDatas[w].GetWorldMapData(i);
                IndexableDirectionalStruct<bool> exits = new();
                for (var d = 1; d < 5; d++)
                {
                    exits[d] = worldMapData.Exits[d].UnlockTriggerIndex >= 0;
                }
                _exitLockStateList[w][i] = exits;
            }
        }

        _mainMapGenerator.InitializeObjects(_metatilePrefab, startingMapData.Size, _mainScreen.transform);
        _subMapGenerator.InitializeObjects(_metatilePrefab, startingMapData.Size, _subScreen.transform);
        _subScreen.SetActive(false);
        _mainMapGenerator.SetTileset(_tileset);
        _subMapGenerator.SetTileset(_tileset);
        _player.Initialize(_playerStartingPosition);
        _player.SetMapPositionOffset(GetPositionFromXY(0, 0, startingMapData.Size * 2) - new Vector3(0.5f, 0.5f, 0));
        SetStaticMapData(0);
    }
    public static Vector3 GetPositionFromXY(int x, int y, Vector2Int size)
    {
        return new Vector3(x - size.x / 2 + 0.5f, y - size.y / 2 + 0.5f, 0);
    }
    public static ReadOnlyCollection<bool> GetColliders(TileCollisionFlags tileCollisionFlags)
    {
        return Instance.DoGetColliders(tileCollisionFlags);
    }
    private ReadOnlyCollection<bool> DoGetColliders(TileCollisionFlags tileCollisionFlags)
    {
        List<bool> mainColliders = MainMapData.GetColliders(tileCollisionFlags).ToList();
        var triggerColliders = WorldMapEvents[_currentMapIndex].Where(x => x.Value.IsSolid).Select(x => x.Key);
        var size = MainMapData.Size;
        foreach (var location in triggerColliders)
        {
            var mx = location.x;
            var my = location.y;
            for (var i = 0; i < 4; i++)
            {
                var tx = i & 1;
                var ty = 1 - (i >> 1 & 1);
                var index = (mx * 2 + tx) + (my * size.x * 4) + (ty * size.x * 2);
                mainColliders[index] = true;
            }
        }
        return mainColliders.AsReadOnly();
    }

    private void SetStaticMapData(int mapIndex)
    {
        var mapData = CurrentWorldData.GetWorldMapData(mapIndex).MapData;
        MainMapData = mapData;
        _mainMapGenerator.LoadMapData(mapData, _metatileMemory);
        List<System.Tuple<int, int>> overrides = new();
        foreach (var (position, triggerEvent) in WorldMapEvents[mapIndex])
        {
            if (!triggerEvent.IsVisible) continue;
            var index = position.x + position.y * mapData.Size.x;
            overrides.Add(new(index, triggerEvent.TileIndex));
        }
        _mainMapGenerator.OverrideSelectedMetatiles(mapData, _metatileMemory, overrides);
    }

    private IEnumerator MoveCamera(Direction direction, int nextMapIndex)
    {
        _player.SetCanMove(false);
        var directionVector = direction.ToVector2();

        var nextMap = CurrentWorldData.GetWorldMapData(nextMapIndex);
        var nextMapData = nextMap.MapData;

        var baseTarget = new Vector2(nextMapData.Size.x * directionVector.x, nextMapData.Size.y * directionVector.y);
        var target = 2 * baseTarget;

        _subScreen.SetActive(true);
        _subMapGenerator.LoadMapData(nextMapData, _metatileMemory);
        
        List<System.Tuple<int, int>> overrides = new();
        foreach (var (position, triggerEvent) in WorldMapEvents[nextMapIndex])
        {
            if (!triggerEvent.IsVisible) continue;
            overrides.Add(new(position.x + position.y * nextMap.MapData.Size.x, triggerEvent.TileIndex));
        }
        _subMapGenerator.OverrideSelectedMetatiles(nextMapData, _metatileMemory, overrides);
        _subScreen.transform.localPosition = (Vector3)target;
        float currentTime = 0;

        Vector3 initialPosition = _camera.transform.position;
        Vector3 targetPosition = new(target.x, target.y, initialPosition.z);

        var waitFrame = new WaitForFixedUpdate();

        var playerCurrentPosition = _player.CurrentPosition;
        var playerTargetPosition = playerCurrentPosition + (Vector3)directionVector * (2 + nextMap.Exits[(CompassDirections)direction.Inverse()].ExitOffset);

        yield return new WaitForSeconds(_delay);
        while (currentTime < _lerpTime)
        {
            _camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime / _lerpTime);
            _player.SetCurrentPosition(Vector3.Lerp(playerCurrentPosition, playerTargetPosition, currentTime / _lerpTime));
            yield return waitFrame;
            currentTime += Time.deltaTime;
        }
        _camera.transform.position = initialPosition;
        _player.AddCurrentPosition((new Vector2(nextMapData.Size.x * -directionVector.x, nextMapData.Size.y * -directionVector.y)) * 2);
        _player.SetCanMove(true);
        _subScreen.SetActive(false);
        SetStaticMapData(nextMapIndex);
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
        if (!CurrentWorldData.IsNextMapReachable(_currentMapIndex, direction)) return;
        if (_coroutine != null) return;
        var nextIndex = CurrentWorldData.GetNextIndex(_currentMapIndex, direction);

        var exits = ExitLockStates[nextIndex];
        exits[(CompassDirections)direction.Inverse()] = false;
        ExitLockStates[nextIndex] = exits;

        var nextWorldData = CurrentWorldData.GetWorldMapData(nextIndex);
        var opposingLockTriggerIndex = nextWorldData.Exits[(CompassDirections)direction.Inverse()].UnlockTriggerIndex;
        if (opposingLockTriggerIndex >= 0)
        {
            var opposingLockTriggerPosition = nextWorldData.MapEventTriggers[opposingLockTriggerIndex].Position;
            if (WorldMapEvents[nextIndex].ContainsKey(opposingLockTriggerPosition))
            {
                WorldMapEvents[nextIndex][opposingLockTriggerPosition].DisableAll();
            }
        }

        _entitySpawner.DeactivateAll();
        _coroutine = StartCoroutine(MoveCamera(direction, nextIndex));
    }
    public static bool UnlockExit(CompassDirections exitDirection)
    {
        return Instance.DoUnlockExit(exitDirection);
    }
    private bool DoUnlockExit(CompassDirections exitDirection)
    {
        if (_keysInInventoryPerWorld[_currentWorldIndex] == 0) { Debug.Log("No Key");  return false; }
        _keysInInventoryPerWorld[_currentWorldIndex]--;
        Debug.Log("Unlocking");
        var exits = ExitLockStates[_currentMapIndex];
        exits[exitDirection] = false;
        ExitLockStates[_currentMapIndex] = exits;

        var lockTriggerIndex = CurrentWorldMapData.Exits[exitDirection].UnlockTriggerIndex;
        var lockTriggerPosition = CurrentWorldMapData.MapEventTriggers[lockTriggerIndex].Position;

        if (WorldMapEvents[_currentMapIndex].ContainsKey(lockTriggerPosition))
        {
            var eventTrigger = WorldMapEvents[_currentMapIndex][lockTriggerPosition];
            eventTrigger.DisableAll();
            var index = lockTriggerPosition.x + lockTriggerPosition.y * MainMapData.Size.x;
            var tileIndex = MainMapData.MetatileDatas[index].metatileIndex;
            _mainMapGenerator.OverrideSelectedMetatile(MainMapData, _metatileMemory, index, tileIndex);
        }

        return true;
    }
    public static void TriggerEvent(Vector2Int position)
    {
        Instance.DoTriggerEvent(position);
    }
    private void DoTriggerEvent(Vector2Int position)
    {
        if (!WorldMapEvents[_currentMapIndex].ContainsKey(position)) { return; }
        WorldMapEvents[_currentMapIndex][position].TriggerEvent();
    }
}
