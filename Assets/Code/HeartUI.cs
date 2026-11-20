using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private UIController uiController;
    [SerializeField] private Tileset tileset;

    [SerializeField] private int fullHeartIndex;
    [SerializeField] private int halfHeartIndex;
    [SerializeField] private int emptyHeartIndex;

    [SerializeField] private Vector2Int heartStartPosition = new Vector2Int(0, 0);
    private void Start()
    {
        playerHP.OnHPChanged.AddListener(UpdateHeart);

        UpdateHeart(playerHP.CurrentHP, playerHP.MaxHP);
    }

    void UpdateHeart(int currentHP, int maxHP)
    {
        int maxHeart = Mathf.CeilToInt(maxHP / 2f);

        for (int i = 0; i < maxHeart; i++)
        {
            int hp = currentHP - i * 2;

            Sprite sprite;

            if (hp >= 2)
                sprite = tileset.GetSprite(fullHeartIndex);
            else if (hp == 1)
                sprite = tileset.GetSprite(halfHeartIndex);
            else
                sprite = tileset.GetSprite(emptyHeartIndex);

            int x=i%10;
            int y=i/10;

            uiController.GetSprite(heartStartPosition.x + x,heartStartPosition.y-y)?.SetSprite(sprite);
        }
    }
}