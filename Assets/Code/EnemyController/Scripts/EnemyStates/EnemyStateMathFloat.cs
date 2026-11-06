using UnityEngine;

[System.Serializable]
[AddTypeMenu("Floating Math")]
public class EnemyStateMathFloat : EnemyState
{
    [SerializeReference, SubclassSelector] private EnemyStateMathFloatBase operation = new EnemyStateMathFloatSet();
    [SerializeField] private EnemyController.Register destination;

    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        enemyController.FloatingRegisters[(int)destination] = operation.DoOperation(enemyController);
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
