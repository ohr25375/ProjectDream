using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isUI = false;
    private bool _isInFront = false;
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
    public void SetInFront(bool isInFront) => UpdateSortingOrder(isInFront, _isUI);
    public void SetUI(bool isUI) => UpdateSortingOrder(_isInFront, isUI);
    public void SetPropertyBlock(MaterialPropertyBlock mpb) => _spriteRenderer.SetPropertyBlock(mpb);
    public void UpdateSortingOrder(bool isInFront, bool isUI)
    {
        _isInFront = isInFront;
        _isUI = isUI;
        _spriteRenderer.sortingOrder = (_isInFront ? 100 : -100) + (_isUI ? 1000 : 0); 
    }
}
