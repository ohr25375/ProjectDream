using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    struct SpriteControllerRenderer
    {
        public SpriteRenderer SpriteRenderer;
        public SpriteController SpriteController;
        public void Initialize()
        {
            SpriteController = new();
            SpriteController.Initialize(SpriteRenderer);
        }
    }
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private GameObject _spritePrefab;
    [SerializeField] private EntityData _entityData;
    [SerializeField] private int _maxEntityCount = 1;
    [SerializeField] private int _spriteControllerCount = 16;
    private List<EntityControllerController> _entityControllers;
    private List<SpriteControllerRenderer> _spriteControllerRenderers;

    #region UNITY
    private void Awake()
    {
        Initialize(_maxEntityCount, transform, _entityPrefab, _spriteControllerCount, _spritePrefab);
    }
    private void Update()
    {
        UpdateControllers();
    }
    #endregion

    #region INTERNAL
    /// <summary>
    /// Initializes the Entity Controller pool
    /// </summary>
    /// <param name="maxEntityCount">Max count of entities accessable from the pool</param>
    /// <param name="entityPrefab">The prefab used to initialize the pool with</param>
    private void InitializeEntityControllers(int maxEntityCount, GameObject entityPrefab)
    {
        _entityControllers = new(new EntityControllerController[maxEntityCount]);
        for (var i = 0; i < maxEntityCount; i++)
        {
            var obj = Instantiate(entityPrefab, transform);
            _entityControllers[i] = obj.GetComponent<EntityControllerController>();
            obj.SetActive(false);
        }
    }
    private void InitializeSpriteControllers(int maxCount, GameObject prefab)
    {
        _spriteControllerRenderers = new(new SpriteControllerRenderer[maxCount]);
        for (var i = 0; i < maxCount; i++)
        {
            var obj = Instantiate(prefab, transform);
            SpriteControllerRenderer scr = new()
            {
                SpriteRenderer = obj.GetComponent<SpriteRenderer>()
            };
            scr.Initialize();
            _spriteControllerRenderers[i] = scr;
            obj.SetActive(false);
        }
    }
    private EntityControllerController GetNewEntityControllerController()
    {
        var inactiveObjects = _entityControllers.Where(x => !x.gameObject.activeSelf);
        if (inactiveObjects.Count() == 0)
        {
            Debug.LogWarning("Pool empty", this);
            return null;
        }
        return inactiveObjects.First();
    }
    private SpriteController GetNewSpriteController()
    {
        var inactiveObjects = _spriteControllerRenderers.Where(x => !x.SpriteRenderer.gameObject.activeSelf);
        if (inactiveObjects.Count() == 0)
        {
            Debug.LogWarning("Pool empty", this);
            return null;
        }
        var selected = inactiveObjects.First().SpriteController;
        selected.Enable();
        return selected;
    }
    private List<SpriteController> GetSpriteControllers(int count)
    {
        List<SpriteController> spriteControllers = new(new SpriteController[count]);
        for (var i = 0; i < count; i++)
        {
            var sc = GetNewSpriteController();
            if (sc == null)
            {
                Debug.LogWarning("Not enough SpriteControllers were initialized at start. Adjust count");
            }
            spriteControllers[i] = sc;
        }
        return spriteControllers;
    }
    #endregion

    public void Initialize(int maxEntityCount, Transform transform, GameObject entityPrefab, int spriteControllerCount, GameObject spriteControllerPrefab)
    {
        // check prefab for controller
        if (_entityPrefab.GetComponent<EntityControllerController>() == null)
        {
            Debug.LogError("Prefab does not contain EntityControllerController. Check prefab", this);
        }
        InitializeEntityControllers(maxEntityCount, entityPrefab);
        InitializeSpriteControllers(spriteControllerCount, spriteControllerPrefab);
    }
    public void UpdateControllers()
    {
        var activeObjects = _entityControllers.Where(x => x.gameObject.activeSelf);
        foreach (var obj in activeObjects)
        {
            obj.UpdateEntity();
        }
    }
    public void ActivateNew(EntityData entityData, Vector2Int spawnPosition)
    {
        var controller = GetNewEntityControllerController();
        if (controller == null)
        {
            Debug.LogWarning("Got null from pool. Pool may be empty!", this);
            return;
        }
        controller.gameObject.SetActive(true);
        var mapPositionOffset = ScreenController.GetPositionFromXY(0, 0, ScreenController.MainMapData.Size * 2) - new Vector3(0.5f, 0.5f, 0);
        var spriteControllers = GetSpriteControllers(entityData.ColliderOffsets.Count);
        controller.Initialize(entityData, spriteControllers, spawnPosition, mapPositionOffset);
    }
    public void Deactivate(int index)
    {
        if (index >=  _entityControllers.Count)
        {
            Debug.LogWarning($"{index} is larger than current pool count {_entityControllers.Count}. Check code", this);
            return;
        }
        _entityControllers[index].ReturnToPool();
    }

    #region EDITOR
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private Vector2Int debugSpawnPosition;
    [ContextMenu("Spawn Entity")]
    private void SpawnEntity()
    {
        ActivateNew(_entityData, debugSpawnPosition);
    }
#endif
    #endregion
}
