using UnityEngine;

[System.Serializable]
[AddTypeMenu("Wait for Duration")]
public class EntityStateWaitForDuration : EntityState
{
    [SerializeReference, SubclassSelector] private EntityValueFloatBase duration = new EntityValueFloatUseImmediate();
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
        if (timer <= duration.GetValue(entityController)) return;
        ChangeToNextState();
    }
}
