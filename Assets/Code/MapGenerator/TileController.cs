using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private Tileset _tileset;
    private TileData _tileData;
    private Palette _palette;
    [SerializeField] private bool _isBackground;
    public void SetPalette(Palette palette) => _palette = palette;
    public void SetTileData(TileData tileData) => _tileData = tileData;
    public void SetTileset(Tileset tileset) => _tileset = tileset;
    public void UpdateRenderer()
    {
        _renderer.sprite = _tileset.GetSprite(_tileData.spriteData.spriteID);
        _renderer.flipX = _tileData.spriteData.isFlippedX;
        _renderer.flipY = _tileData.spriteData.isFlippedY;
        _renderer.sortingOrder = _tileData.collisionData.isInFront ? 100 : -100;
        MaterialPropertyBlock mpb = new();
        mpb.SetFloat("_Palette", (float)_palette);
        mpb.SetFloat("_IsBackground", _isBackground ? 1 : 0);
        _renderer.SetPropertyBlock(mpb);
    }
    public void SetHorizontalFlip(bool isFlipped)
    {
        _tileData.spriteData.isFlippedX = isFlipped;
        _renderer.flipX = isFlipped;
    }
    public void SetVerticalFlip(bool isFlipped)
    {
        _tileData.spriteData.isFlippedY = isFlipped;
        _renderer.flipY = isFlipped;
    }
    public void UpdateTile(Tileset tileset, TileData tileData)
    {
        SetTileData(tileData);
        SetTileset(tileset);
        UpdateRenderer();
    }
}
