using UnityEngine;

[System.Serializable]
[AddTypeMenu("Integer Math")]
public class EntityStateMath : EntityState
{
    [SerializeReference, SubclassSelector] private EnityStateMathBase operation = new EntityStateMathSet();
    [SerializeField] private EntityController.Register destination;

    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        entityController.Registers[(int)destination] = operation.DoOperation(entityController);
        ChangeToNextState();
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
