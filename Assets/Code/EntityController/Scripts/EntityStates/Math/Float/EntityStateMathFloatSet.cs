using UnityEngine;

[System.Serializable]
[AddTypeMenu("Set")]
public class EntityStateMathFloatSet : EntityStateMathFloatBase
{
    [SerializeReference, SubclassSelector] private EntityValueFloatBase value = new EntityValueFloatUseImmediate();

    public override float DoOperation(EntityController enemyController)
    {
        return value.GetValue(enemyController);
    }
}
