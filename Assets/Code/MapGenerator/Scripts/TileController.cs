using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private SpriteController _spriteController;

    private Tileset _tileset;
    private TileData _tileData;
    private Palette _palette;
    [SerializeField] private bool _isBackground;
    bool _isUI = false;
    public void SetPalette(Palette palette) => _palette = palette;
    public void SetTileData(TileData tileData) => _tileData = tileData;
    public void SetTileset(Tileset tileset) => _tileset = tileset;
    public void SetTileOrder(bool isUI) => _isUI = isUI;
    public void UpdateRenderer()
    {
        _spriteController.SetSprite(_tileset.GetSprite(_tileData.spriteData.spriteID));
        _spriteController.SetFlipX(_tileData.spriteData.isFlippedX);
        _spriteController.SetFlipY(_tileData.spriteData.isFlippedY);
        _spriteController.UpdateSortingOrder(_tileData.collisionData.isInFront, _isUI);
        MaterialPropertyBlock mpb = new();
        mpb.SetFloat("_Palette", (float)_palette);
        mpb.SetFloat("_IsBackground", _isBackground ? 1 : 0);
        _spriteController.SetPropertyBlock(mpb);
    }
    public void SetHorizontalFlip(bool isFlipped)
    {
        _tileData.spriteData.isFlippedX = isFlipped;
        _spriteController.SetFlipX(isFlipped);
    }
    public void SetVerticalFlip(bool isFlipped)
    {
        _tileData.spriteData.isFlippedY = isFlipped;
        _spriteController.SetFlipY(isFlipped);
    }
    public void UpdateTile(Tileset tileset, TileData tileData)
    {
        SetTileData(tileData);
        SetTileset(tileset);
        UpdateRenderer();
    }
}
