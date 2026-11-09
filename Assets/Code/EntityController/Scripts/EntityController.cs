using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityController
{
    private Transform _transform;
    public Vector2Int MapSize { get; private set; } = Vector2Int.zero;
    [System.Serializable]
    public enum Register
    {
        A,
        B,
    }
    public int[] Registers = new int[System.Enum.GetNames(typeof(Register)).Length];
    public float[] FloatingRegisters = new float[System.Enum.GetNames(typeof(Register)).Length];
    public int CurrentStateIndex { get; private set; } = 0;
    public bool IsPaused = false;
    private List<EntityState> _entityStates;
    private EntityState CurrentEnemyState => _entityStates[CurrentStateIndex];
    
    public void Initialize(Transform transform, Vector2Int position) => OnInitialize(transform, position);
    public void SetMapSize(Vector2Int mapSize) => MapSize = mapSize;
    public void SetStateTree(List<EntityState> states) => _entityStates = states;
    public void Update() => OnUpdate();
    public void SetEntityState(int index)
    {
        if (index < 0)
        {
            index = _entityStates.Count - ((-index) % _entityStates.Count);
        }
        CurrentStateIndex = index % _entityStates.Count;
        CurrentEnemyState.EnterState(this);
    }
    public void SetEntityStateRelative(int index)
    {
        if (index < 0)
        {
            index = _entityStates.Count - ((-index) % _entityStates.Count);
        }
        int stateIndex = (CurrentStateIndex + index) % _entityStates.Count;
        SetEntityState(stateIndex);
    }
    private void OnInitialize(Transform transform, Vector2Int position)
    {
        if (_entityStates == null)
        {
            Debug.LogError("Entity state not initialized. Check parent code", transform.gameObject);
            return;
        }
        CurrentStateIndex = 0;
        _transform = transform;
        _transform.position = (Vector3)(Vector2)position;
        for (var i = 0; i < Registers.Length; i++)
        {
            Registers[i] = 0;
        }
        CurrentEnemyState.EnterState(this);
    }
    private void OnUpdate()
    {
        if (IsPaused) return;
        CurrentEnemyState.UpdateState(Time.deltaTime);
    }
    public void Move(Vector2 direction, float speed = 1)
    {
        _transform.position += speed * (Vector3)direction;
    }
}
