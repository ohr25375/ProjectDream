using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyController
{
    [SerializeField] private EnemyStateTree _enemyStateTree;
    private List<EnemyState> _enemyStates;
    private int _currentStateIndex = 0;
    private EnemyState CurrentEnemyState => _enemyStates[_currentStateIndex];
    private Transform _transform;
    [System.Serializable]
    public enum Register
    {
        A,
        B,
    }
    public int[] Registers = new int[System.Enum.GetNames(typeof(Register)).Length];
    public float[] FloatingRegisters = new float[System.Enum.GetNames(typeof(Register)).Length];
    public int CurrentStateIndex => _currentStateIndex;
    public bool IsPaused = false;
    public void Initialize(Transform transform) => OnInitialize(transform);
    public void Update() => OnUpdate();
    public void SetEnemyState(int index)
    {
        if (index < 0)
        {
            index = _enemyStates.Count - ((-index) % _enemyStates.Count);
        }
        _currentStateIndex = index % _enemyStates.Count;
        CurrentEnemyState.EnterState(this);
    }
    public void SetEnemyStateRelative(int index)
    {
        if (index < 0)
        {
            index = _enemyStates.Count - ((-index) % _enemyStates.Count);
        }
        int stateIndex = (_currentStateIndex + index) % _enemyStates.Count;
        SetEnemyState(stateIndex);
    }
    private void OnInitialize(Transform transform)
    {
        _enemyStates = _enemyStateTree.GetClone();
        _currentStateIndex = 0;
        _transform = transform;
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
