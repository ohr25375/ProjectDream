using UnityEngine;

[System.Serializable]
[AddTypeMenu("Immediate")]
public class EnemyValueFloatUseImmediate : EnemyValueFloatBase
{
    [SerializeField] private float value;
    public override float GetValue(EnemyController controller) => value;
}
