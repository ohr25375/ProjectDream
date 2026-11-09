using UnityEngine;

[System.Serializable]
[AddTypeMenu("Register")]
public class EntityValueUseRegister : EntityValueBase
{
    [SerializeField] private EntityController.Register register;
    public override int GetValue(EntityController controller) => controller.Registers[(int)register];
}
