using UnityEngine;

[System.Serializable]
[AddTypeMenu("Register")]
public class EnemyValueUseRegister : EnemyValueBase
{
    [SerializeField] private EnemyController.Register register;
    public override int GetValue(EnemyController controller) => controller.Registers[(int)register];
}
