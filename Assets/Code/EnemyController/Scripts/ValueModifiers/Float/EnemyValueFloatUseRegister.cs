using UnityEngine;

[System.Serializable]
[AddTypeMenu("Register")]
public class EnemyValueFloatUseRegister : EnemyValueFloatBase
{
    [SerializeField] private EnemyController.Register register;
    public override float GetValue(EnemyController controller) => controller.FloatingRegisters[(int)register];
}
