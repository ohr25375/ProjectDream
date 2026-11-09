using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private EntityStateTree _entityStateTree;
    [SerializeField] private int _maxEntityCount = 1;
    private List<EntityControllerController> _entityControllers;


    private void Awake()
    {
        Initialize(_maxEntityCount, transform, _entityPrefab);
    }
    private void Update()
    {
        UpdateControllers();
    }
    public void Initialize(int maxEntityCount, Transform transform, GameObject entityPrefab)
    {
        // check prefab for controller
        if (_entityPrefab.GetComponent<EntityControllerController>() == null)
        {
            Debug.LogError("Prefab does not contain EntityControllerController. Check prefab");
        }
        // create instances of controllers
        _entityControllers = new List<EntityControllerController>(new EntityControllerController[maxEntityCount]);
        for (var i = 0; i < maxEntityCount; i++)
        {
            var obj = Instantiate(entityPrefab, transform);
            _entityControllers[i] = obj.GetComponent<EntityControllerController>();
            obj.SetActive(false);
        }
    }
    public void UpdateControllers()
    {
        var activeObjects = _entityControllers.Where(x => x.gameObject.activeSelf);
        foreach (var obj in activeObjects)
        {
            obj.UpdateEntity();
        }
    }
    public void ActivateNew(EntityStateTree entityStateTree, Vector2Int spawnPosition)
    {
        var inactiveObjects = _entityControllers.Where(x => !x.gameObject.activeSelf);
        if (inactiveObjects.Count() == 0)
        {
            Debug.LogWarning("Pool empty");
            return;
        }
        var controller = inactiveObjects.First();
        controller.gameObject.SetActive(true);
        controller.Initialize(entityStateTree.GetClone(), spawnPosition);
    }
    public void Deactivate(int index)
    {
        if (index >=  _entityControllers.Count)
        {
            Debug.LogWarning($"{index} is larger than current pool count {_entityControllers.Count}. Check code", this);
            return;
        }
        _entityControllers[index].gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private Vector2Int debugSpawnPosition;
    [ContextMenu("Spawn Entity")]
    private void SpawnEntity()
    {
        ActivateNew(_entityStateTree, debugSpawnPosition);
    }
#endif
}
