using UnityEngine;

public class EntityData : MonoBehaviour
{
    [SerializeField] private bool isPlayerTeam;//true:味方 false:敵

    [SerializeField] private int damage = 1;

    [SerializeField] private bool isDamageContact; //触れたらダメージを与えるか
    [SerializeField] private bool isInvincible;    //無敵かどうか

    [SerializeField] private StaminaControl staminaControl;

    //[SerializeField] private GameObject entity;
    //
    //private void SummonEntity()
    //{
    //
    //}
    public void DoDamage(int amount)
    {
        staminaControl.Damage(amount);
        Debug.Log(amount);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit");
        if (!isDamageContact) return;
        var target = collision.gameObject.GetComponent<EntityData>();
        if (target.isPlayerTeam != isPlayerTeam)
        {
            target.DoDamage(damage);
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Vector2 pos = transform.position + transform.localScale;


    }

    public void Move() //オーバーライド
    {

    }

    public void Invincible() //オーバーライド
    {
        if (isInvincible) return;


    }
}
