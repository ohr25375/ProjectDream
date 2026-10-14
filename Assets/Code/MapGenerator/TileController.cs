using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private Tileset _tileset;
    private TileData _tileData;
    public void SetTileData(TileData tileData) => _tileData = tileData;
    public void SetTileset(Tileset tileset)
    {
        _tileset = tileset;
        _renderer.sprite = _tileset.GetSprite(_tileData.spriteID);
    }
    public void SetHorizontalFlip(bool isFlipped)
    {
        _tileData.isFlippedX = isFlipped;
        _renderer.flipX = isFlipped;
    }

    public void SetVerticalFlip(bool isFlipped)
    {
        _tileData.isFlippedY = isFlipped;
        _renderer.flipY = isFlipped;
    }
    public void SetTilesetATileData(Tileset tileset, TileData tileData)
    {
        SetTileData(tileData);
        SetTileset(tileset);
    }
}
