using UnityEngine;

[System.Serializable]
[AddTypeMenu("Immediate")]
public class EnemyValueUseImmediate : EnemyValueBase
{
    [SerializeField] private int value;
    public override int GetValue(EnemyController controller) => value;
}
