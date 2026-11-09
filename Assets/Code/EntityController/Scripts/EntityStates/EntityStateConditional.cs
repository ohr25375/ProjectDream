using UnityEngine;
[System.Serializable]
[AddTypeMenu("Conditional")]
public class EntityStateConditional : EntityState
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
    [SerializeReference, SubclassSelector] private EntityValueBase x = new EntityValueUseImmediate();
    [SerializeReference, SubclassSelector] private EntityValueBase y = new EntityValueUseImmediate();
    [SerializeReference, SubclassSelector] private ExitToStatesBase onCondition = new ExitToIndex();
    private bool DoCondition(Conditions condition, int x, int y)
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
