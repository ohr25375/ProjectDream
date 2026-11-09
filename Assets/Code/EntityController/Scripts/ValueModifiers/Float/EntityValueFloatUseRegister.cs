using UnityEngine;

[System.Serializable]
[AddTypeMenu("Register")]
public class EntityValueFloatUseRegister : EntityValueFloatBase
{
    [SerializeField] private EntityController.Register register;
    public override float GetValue(EntityController controller) => controller.FloatingRegisters[(int)register];
}
