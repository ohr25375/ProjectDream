using UnityEngine;

[System.Serializable]
[AddTypeMenu("Set")]
public class EnemyStateMathSet : EnemyStateMathBase
{
    [SerializeReference, SubclassSelector] private EnemyValueBase value = new EnemyValueUseImmediate();

    public override int DoOperation(EnemyController enemyController)
    {
        return value.GetValue(enemyController);
    }
}
