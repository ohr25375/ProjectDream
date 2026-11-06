using UnityEngine;

[System.Serializable]
[AddTypeMenu("Set")]
public class EnemyStateMathFloatSet : EnemyStateMathFloatBase
{
    [SerializeReference, SubclassSelector] private EnemyValueFloatBase value = new EnemyValueFloatUseImmediate();

    public override float DoOperation(EnemyController enemyController)
    {
        return value.GetValue(enemyController);
    }
}
