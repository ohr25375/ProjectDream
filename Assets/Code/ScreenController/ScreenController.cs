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

    private Coroutine _coroutine;
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private MapData _startingMapData;
    [SerializeField] private MapData _nextMapData;
    [System.Serializable]
    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    [SerializeField] private Direction _direction;

    private void Awake()
    {
        _mainMapGenerator.InitializeObjects(_metatilePrefab, _startingMapData.Size, _mainScreen.transform);
        _subMapGenerator.InitializeObjects(_metatilePrefab, _startingMapData.Size, _subScreen.transform);
        _subScreen.SetActive(false);
        _mainMapGenerator.SetTileset(_tileset);
        _subMapGenerator.SetTileset(_tileset);
        _mainMapGenerator.LoadMapData(_startingMapData, _metatileMemory);
    }
#endif

    private IEnumerator MoveCamera(Vector2 target, MapData nextMapData)
    {
        _subScreen.SetActive(true);
        _subMapGenerator.LoadMapData(nextMapData, _metatileMemory);
        _subScreen.transform.localPosition = (Vector3)target;
        float currentTime = 0;
        Vector3 initialPosition = _camera.transform.position;
        Vector3 targetPosition = new Vector3(target.x, target.y, initialPosition.z);
        var waitFrame = new WaitForFixedUpdate();
        yield return new WaitForSeconds(_delay);
        while (currentTime < _lerpTime)
        {
            _camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime / _lerpTime);
            yield return waitFrame;
            currentTime += Time.deltaTime;
        }
        _camera.transform.position = initialPosition;
        _mainMapGenerator.LoadMapData(nextMapData, _metatileMemory);
        _subScreen.SetActive(false);
        _coroutine = null;
    }
    public void Initialize(MapData initialMapData)
    {
        _mainMapGenerator.LoadMapData(initialMapData, _metatileMemory);
    }
    public void MoveScreenLeft(MapData nextMapData)
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(MoveCamera(2 * nextMapData.Size.x * Vector2.left, nextMapData));
    }
    public void MoveScreenRight(MapData nextMapData)
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(MoveCamera(2 * nextMapData.Size.x * Vector2.right, nextMapData));
    }
    public void MoveScreenUp(MapData nextMapData)
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(MoveCamera(2 * nextMapData.Size.y * Vector2.up, nextMapData));
    }
    public void MoveScreenDown(MapData nextMapData)
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(MoveCamera(2 * nextMapData.Size.y * Vector2.down, nextMapData));
    }

    [ContextMenu("Move Map")]
    private void MoveMap()
    {
        switch (_direction)
        {
            case Direction.Left: MoveScreenLeft(_nextMapData); break;
            case Direction.Right: MoveScreenRight(_nextMapData); break;
            case Direction.Up: MoveScreenUp(_nextMapData); break;
            case Direction.Down: MoveScreenDown(_nextMapData); break;
        }
    }
}
