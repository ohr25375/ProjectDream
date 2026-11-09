using UnityEngine;

[System.Serializable]
[AddTypeMenu("Immediate")]
public class EntityValueUseImmediate : EntityValueBase
{
    [SerializeField] private int value;
    public override int GetValue(EntityController controller) => value;
}
