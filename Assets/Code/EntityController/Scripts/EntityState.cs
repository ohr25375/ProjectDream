using UnityEngine;

[System.Serializable]
public abstract class EntityState : System.ICloneable
{
    protected float timer;
    protected float previousTimer;
    protected float DeltaTime => timer - previousTimer;
    protected EntityController entityController;
    [SerializeReference, SubclassSelector] private ExitToStatesBase exitToState = new ExitToNext();
    public EntityState Clone()
    {
        return MemberwiseClone() as EntityState;
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
    public void EnterState(EntityController enemyController, params int[] variables)
    {
        this.entityController = enemyController;
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
        entityController.SetEntityState(exitToState.GetExitStateIndex(entityController.CurrentStateIndex));
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
