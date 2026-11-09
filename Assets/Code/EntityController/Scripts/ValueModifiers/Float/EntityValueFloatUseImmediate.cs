using UnityEngine;

[System.Serializable]
[AddTypeMenu("Immediate")]
public class EntityValueFloatUseImmediate : EntityValueFloatBase
{
    [SerializeField] private float value;
    public override float GetValue(EntityController controller) => value;
}
