using UnityEngine;

[System.Serializable]
public class EnemyStateWaitForDuration : EnemyState
{
    [SerializeField] private float duration;
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
    }
    protected override void OnStateExit()
    {
        //
    }

    protected override void OnStateUpdate()
    {
        if (timer <= duration) return;
        ChangeToNextState();
    }
}
