using UnityEngine;

[System.Serializable]
[AddTypeMenu("Add")]
public class EnemyStateMathFloatAdd : EnemyStateMathFloatBase
{
    [SerializeReference, SubclassSelector] private EnemyValueFloatBase x = new EnemyValueFloatUseImmediate();
    [SerializeReference, SubclassSelector] private EnemyValueFloatBase y = new EnemyValueFloatUseImmediate();

    public override float DoOperation(EnemyController enemyController)
    {
        return x.GetValue(enemyController) + y.GetValue(enemyController);
    }
}
