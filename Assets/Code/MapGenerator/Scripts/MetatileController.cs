using UnityEngine;
public class MetatileController : MonoBehaviour
{
    [SerializeField] private IndexableMetatileStruct<TileController> _tileControllers;

    public void SetTileDatas(MetatileData metatileData)
    {
        for (var i = 0; i < 4; i++)
        {
            _tileControllers[i].SetTileData(metatileData.data[i]);
        }
    }
    public void SetTilesets(Tileset tileset)
    {
        for (var i = 0; i < 4; i++)
        {
            _tileControllers[i].SetTileset(tileset);
        }
    }
    public void SetPalettes(Palette palette)
    {
        for (var i = 0; i < 4; i++)
        {
            _tileControllers[i].SetPalette(palette);
        }
    }
    public void UpdateTileRenderers()
    {
        for (var i = 0; i < 4; i++)
        {
            _tileControllers[i].UpdateRenderer();
        }
    }
    public void SetTilesetsAndTileDatas(Tileset tileset, MetatileData metatileData)
    {
        SetTileDatas(metatileData);
        SetTilesets(tileset);
        SetPalettes(metatileData.palette);
    }
    public void UpdateTiles(Tileset tileset, MetatileMapData metatileMapData, MetatileSpriteData metatileSpriteData)
    {
        SetTilesetsAndTileDatas(tileset, new(metatileMapData, metatileSpriteData));
        UpdateTileRenderers();
    }
}
