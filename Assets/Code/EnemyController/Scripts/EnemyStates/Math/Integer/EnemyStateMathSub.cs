using UnityEngine;

[System.Serializable]
[AddTypeMenu("Sub")]
public class EnemyStateMathSub : EnemyStateMathBase
{
    [SerializeReference, SubclassSelector] private EnemyValueBase x = new EnemyValueUseImmediate();
    [SerializeReference, SubclassSelector] private EnemyValueBase y = new EnemyValueUseImmediate();

    public override int DoOperation(EnemyController enemyController)
    {
        return x.GetValue(enemyController) - y.GetValue(enemyController);
    }
}
