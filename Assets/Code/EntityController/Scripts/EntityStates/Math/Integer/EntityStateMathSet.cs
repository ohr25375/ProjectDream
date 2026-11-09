using UnityEngine;

[System.Serializable]
[AddTypeMenu("Set")]
public class EntityStateMathSet : EnityStateMathBase
{
    [SerializeReference, SubclassSelector] private EntityValueBase value = new EntityValueUseImmediate();

    public override int DoOperation(EntityController enemyController)
    {
        return value.GetValue(enemyController);
    }
}
