using UnityEngine;

[System.Serializable]
public abstract class EnemyState : System.ICloneable
{
    protected float timer;
    protected float previousTimer;
    protected float DeltaTime => timer - previousTimer;
    protected EnemyController enemyController;
    [SerializeReference, SubclassSelector] private ExitToStatesBase exitToState = new ExitToNext();
    public EnemyState Clone()
    {
        return MemberwiseClone() as EnemyState;
    }
    object System.ICloneable.Clone()
    {
        return Clone();
    }
    public void UpdateState(float deltaTime)
    {
        previousTimer = timer;
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
        ChangeToNextState(exitToState);
    }
    public void ChangeToNextState(ExitToStatesBase exitToState)
    {
        ExitState();
        enemyController.SetEnemyState(exitToState.GetExitStateIndex(enemyController.CurrentStateIndex));
    }
    private void InitializeBaseEnemyState()
    {
        timer = 0;
        previousTimer = 0;
    }
    protected virtual void OnStateEnter(params int[] variables)
    {
        InitializeBaseEnemyState();
    }
    protected abstract void OnStateExit();
    protected abstract void OnStateUpdate();
}
