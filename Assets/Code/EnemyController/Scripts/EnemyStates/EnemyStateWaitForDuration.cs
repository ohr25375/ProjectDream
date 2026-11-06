using UnityEngine;

[System.Serializable]
[AddTypeMenu("Wait for Duration")]
public class EnemyStateWaitForDuration : EnemyState
{
    [SerializeReference, SubclassSelector] private EnemyValueFloatBase duration = new EnemyValueFloatUseImmediate();
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
        if (timer <= duration.GetValue(enemyController)) return;
        ChangeToNextState();
    }
}
