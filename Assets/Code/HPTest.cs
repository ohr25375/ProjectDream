using UnityEngine;
using UnityEngine.InputSystem;

public class HPTest : MonoBehaviour
{
    [SerializeField] private StaminaControl hp;
    [Header("キー配置")]
    [SerializeField] private Key damageKey = Key.Digit1;
    [SerializeField] private Key healKey = Key.Digit2;
    [SerializeField] private Key addMaxHPKey = Key.Digit3;
    [SerializeField] private Key fullHealKey = Key.Digit4;
    [SerializeField] private Key setHPKey = Key.Digit5;
    private void Update()
    {
        if (Keyboard.current[damageKey].wasPressedThisFrame)
        {
            hp.Damage(1);
            Debug.Log($"ダメージ CurrentHP={hp.CurrentHP}");
        }

        if (Keyboard.current[healKey].wasPressedThisFrame)
        {
            hp.Heal(1);
            Debug.Log($"回復 CurrentHP={hp.CurrentHP}");
        }

        if (Keyboard.current[addMaxHPKey].wasPressedThisFrame)
        {
            hp.AddMaxHP(2);
            Debug.Log($"最大HP={hp.MaxHP} CurrentHP={hp.CurrentHP}");
        }

        if (Keyboard.current[fullHealKey].wasPressedThisFrame)
        {
            hp.FullHeal();
            Debug.Log($"全回復 CurrentHP={hp.CurrentHP}");
        }

        if (Keyboard.current[setHPKey].wasPressedThisFrame)
        {
            hp.SetHP(3);
            Debug.Log($"HP設定 CurrentHP={hp.CurrentHP}");
        }
    }
}