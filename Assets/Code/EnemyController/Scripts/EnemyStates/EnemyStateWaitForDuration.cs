using UnityEngine;

[System.Serializable]
public class EnemyStateWaitForDuration : EnemyState
{
    [SerializeField] private float duration;
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        duration = variables[0];
    }
    protected override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
