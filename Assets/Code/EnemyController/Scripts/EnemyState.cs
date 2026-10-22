using UnityEngine;
using UnityEditor;

[System.Serializable]
public abstract class EnemyState
{
    protected float timer;
    protected EnemyController enemyController;
    [SerializeReference, SubclassSelector] private ExitToStatesBase exitToState;
    public void UpdateState(float deltaTime)
    {
        timer += deltaTime;
        OnStateUpdate();
    }
    public void EnterState(EnemyController enemyController, params int[] variables)
    {
        this.enemyController = enemyController;
        OnStateEnter(variables);
    }
    public void ExitState()
    {
        OnStateExit();
    }
    public void ChangeToNextState()
    {
        ExitState();
        enemyController.SetEnemyState(exitToState.GetExitStateIndex(enemyController.CurrentStateIndex));
    }
    private void InitializeBaseEnemyState()
    {
        timer = 0;
    }
    protected virtual void OnStateEnter(params int[] variables)
    {
        InitializeBaseEnemyState();
    }
    protected abstract void OnStateExit();
    protected abstract void OnStateUpdate();
}
