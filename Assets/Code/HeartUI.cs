using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    private void Start()
    {
        playerHP.OnHPChanged.AddListener(UpdateHeart);

        UpdateHeart(playerHP.CurrentHP, playerHP.MaxHP);
    }

    void UpdateHeart(int currentHP, int maxHP)
    {

        for (int i = 0; i < hearts.Length; i++)
        {
            int hp = currentHP - i * 2;

            if (hp >= 2)
                hearts[i].sprite = fullHeart;
            else if (hp == 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;

            hearts[i].enabled = i < Mathf.CeilToInt(maxHP / 2f);
        }
    }
}