using UnityEngine;
[System.Serializable]
[AddTypeMenu("Float Conditional")]
public class EntityStateConditionalFloat : EntityState
{
    [System.Serializable]
    enum Conditions
    {
        NOP,
        Jump,
        Equals,
        NotEquals,
        LessThan,
        GreaterThanEquals,
        LessThanEquals,
        GreaterThan,
    }

    [SerializeField] private Conditions condition = Conditions.Jump;
    [SerializeReference, SubclassSelector] private EntityValueFloatBase x = new EntityValueFloatUseImmediate();
    [SerializeReference, SubclassSelector] private EntityValueFloatBase y = new EntityValueFloatUseImmediate();
    [SerializeReference, SubclassSelector] private ExitToStatesBase onCondition = new ExitToIndex();
    private bool DoCondition(Conditions condition, float x, float y)
    {
        bool initial = condition.HasFlag(Conditions.Jump);
        bool equals = condition.HasFlag(Conditions.Equals) && x == y;
        bool lessThan = condition.HasFlag(Conditions.LessThan) && x < y;
        return (equals || lessThan) != initial;
    }
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        if (DoCondition(condition, x.GetValue(entityController), y.GetValue(entityController)))
        {
            ChangeToNextState(onCondition);
        }
        else
        {
            ChangeToNextState();
        }
    }
    protected override void OnStateExit()
    {
        //
    }

    protected override void OnStateUpdate()
    {
        //
    }
}
