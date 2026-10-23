using UnityEngine;

[System.Serializable]
public class EnemyController
{
    [SerializeField] private EnemyStateTree _enemyStateTree;
    private int _currentStateIndex = 0;
    public int CurrentStateIndex => _currentStateIndex;
    private EnemyState CurrentEnemyState => _enemyStateTree[_currentStateIndex];
    private Transform _transform;
    public bool IsPaused = false;
    public void Initialize(Transform transform) => OnInitialize(transform);
    public void Update() => OnUpdate();
    public void SetEnemyState(int index)
    {
        if (index < 0)
        {
            index = _enemyStateTree.Max - ((-index) % _enemyStateTree.Max);
        }
        _currentStateIndex = index % _enemyStateTree.Max;
        CurrentEnemyState.EnterState(this);
    }
    public void SetEnemyStateRelative(int index)
    {
        if (index < 0)
        {
            index = _enemyStateTree.Max - ((-index) % _enemyStateTree.Max);
        }
        int stateIndex = (_currentStateIndex + index) % _enemyStateTree.Max;
        SetEnemyState(stateIndex);
    }
    private void OnInitialize(Transform transform)
    {
        _currentStateIndex = 0;
        CurrentEnemyState.EnterState(this);
        _transform = transform;
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
