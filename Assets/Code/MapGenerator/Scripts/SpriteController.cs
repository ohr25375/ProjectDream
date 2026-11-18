using UnityEngine;

public class SpriteController
{
    private SpriteRenderer _spriteRenderer;
    private bool _isUI = false;
    private bool _isInFront = false;
    public void Initialize(SpriteRenderer spriteRenderer) => _spriteRenderer = spriteRenderer;
    public void Enable() => _spriteRenderer.gameObject.SetActive(true);
    public void ReturnToPool() => _spriteRenderer.gameObject.SetActive(false);
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
    public void SetGlobalPosition(Vector3 position) => _spriteRenderer.gameObject.transform.position = position;
}
