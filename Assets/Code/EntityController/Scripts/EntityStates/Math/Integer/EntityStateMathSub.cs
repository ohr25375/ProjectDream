using UnityEngine;

[System.Serializable]
[AddTypeMenu("Sub")]
public class EntityStateMathSub : EnityStateMathBase
{
    [SerializeReference, SubclassSelector] private EntityValueBase x = new EntityValueUseImmediate();
    [SerializeReference, SubclassSelector] private EntityValueBase y = new EntityValueUseImmediate();

    public override int DoOperation(EntityController enemyController)
    {
        return x.GetValue(enemyController) - y.GetValue(enemyController);
    }
}
