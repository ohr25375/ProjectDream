using UnityEngine;

public class TileController
{
    private readonly SpriteController _spriteController = new();
    private Tileset _tileset;
    private TileData _tileData;
    private Palette _palette;
    public void Initialize(SpriteRenderer spriteRenderer)
    {
        _spriteController.Initialize(spriteRenderer);
    }
    public void SetPalette(Palette palette) => _palette = palette;
    public void SetTileData(TileData tileData) => _tileData = tileData;
    public void SetTileset(Tileset tileset) => _tileset = tileset;
    public void UpdateRenderer()
    {
        _spriteController.SetSprite(_tileset.GetSprite(_tileData.spriteData.spriteID));
        _spriteController.SetFlipX(_tileData.spriteData.isFlippedX);
        _spriteController.SetFlipY(_tileData.spriteData.isFlippedY);
        _spriteController.UpdateSortingOrder(_tileData.collisionData.HasFlag(TileCollisionFlags.IsInFront), false);
        _spriteController.SetPalette(_palette);
        _spriteController.SetIsBackground(_tileData.collisionData.HasFlag(TileCollisionFlags.IsInFront));
        _spriteController.UpdateRenderer();

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
