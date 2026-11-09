using UnityEngine;

[System.Serializable]
[AddTypeMenu("Floating Math")]
public class EntityStateMathFloat : EntityState
{
    [SerializeReference, SubclassSelector] private EntityStateMathFloatBase operation = new EntityStateMathFloatSet();
    [SerializeField] private EntityController.Register destination;

    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        entityController.FloatingRegisters[(int)destination] = operation.DoOperation(entityController);
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
