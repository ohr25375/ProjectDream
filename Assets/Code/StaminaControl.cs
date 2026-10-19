using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class StaminaControl : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected int maxHP = 6;

    public int CurrentHP { get; protected set; }
    public int MaxHP => maxHP;

    [Header("無敵時間")]
    [SerializeField] protected float invincibleTime = 1f;

    protected bool isInvincible;
    public bool IsGodMode {  get; protected set; }

    public UnityEvent<int, int> OnHPChanged;
    public UnityEvent OnDead;
    protected virtual void Awake()
    {
        CurrentHP = maxHP;

        OnHPChanged ??= new UnityEvent<int, int>();
        OnDead ??= new UnityEvent();
    }

    public virtual void Damage(int damage)
    {
        if (IsGodMode||isInvincible) return;

        CurrentHP -= damage;

        if (CurrentHP < 0) CurrentHP = 0;


        OnHPChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP == 0)
        {
            Die();
            return;
        }

        if (invincibleTime > 0)
            StartCoroutine(Invincible());
    }

    public virtual void Heal(int amount)
    {
        CurrentHP += amount;

        if (CurrentHP > maxHP) CurrentHP = maxHP;

        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    public virtual void SetHP(int hp)
    {
        CurrentHP = Mathf.Clamp(hp, 0, maxHP);

        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    public virtual void FullHeal()
    {
        CurrentHP = maxHP;
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    public virtual void AddMaxHP(int amount)
    {
        maxHP += amount;
        CurrentHP += amount;

        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    protected virtual void Die()
    {
        Debug.Log("死亡");
        OnDead?.Invoke();
    }

    IEnumerator Invincible()
    {
        isInvincible = true;
        Debug.Log("無敵開始");
        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        Debug.Log("無敵終了");
    }
    public void SetGodMode(bool value)
    {
        IsGodMode = value;

        if (value)
            Debug.Log("無敵ON");
        else
            Debug.Log("無敵OFF");
    }
}