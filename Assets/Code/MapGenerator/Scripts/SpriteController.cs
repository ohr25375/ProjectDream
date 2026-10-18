using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;
    public void SetFlipX(bool flipX) => _spriteRenderer.flipX = flipX;
    public void SetFlipY(bool flipY) => _spriteRenderer.flipY = flipY;
    public void SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;
    public void SetPropertyBlock(MaterialPropertyBlock mpb) => _spriteRenderer.SetPropertyBlock(mpb);
}
