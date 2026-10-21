using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyState _currentEnemyState;

    public void SetEnemyState(EnemyState enemyState)
    {
        _currentEnemyState?.ExitState();
        _currentEnemyState = enemyState;
        _currentEnemyState.EnterState(this);
    }
    private void OnInitialize(EnemyState startingEnemyState)
    {
        _currentEnemyState = startingEnemyState;
        _currentEnemyState.EnterState(this);
    }
    private void OnUpdate()
    {
        _currentEnemyState.UpdateState(Time.deltaTime);
    }
    public void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction * Time.deltaTime;
    }
}
