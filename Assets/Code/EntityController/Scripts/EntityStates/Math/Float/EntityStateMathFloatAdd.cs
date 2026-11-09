using UnityEngine;

[System.Serializable]
[AddTypeMenu("Add")]
public class EntityStateMathFloatAdd : EntityStateMathFloatBase
{
    [SerializeReference, SubclassSelector] private EntityValueFloatBase x = new EntityValueFloatUseImmediate();
    [SerializeReference, SubclassSelector] private EntityValueFloatBase y = new EntityValueFloatUseImmediate();

    public override float DoOperation(EntityController enemyController)
    {
        return x.GetValue(enemyController) + y.GetValue(enemyController);
    }
}
