using UnityEngine;

[System.Serializable]
[AddTypeMenu("Integer Math")]
public class EnemyStateMath : EnemyState
{
    [SerializeReference, SubclassSelector] private EnemyStateMathBase operation = new EnemyStateMathSet();
    [SerializeField] private EnemyController.Register destination;

    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        enemyController.Registers[(int)destination] = operation.DoOperation(enemyController);
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
