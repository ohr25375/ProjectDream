using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MetatileController : MonoBehaviour
{
    [SerializeField] private TileController _AA;
    [SerializeField] private TileController _BA;
    [SerializeField] private TileController _AB;
    [SerializeField] private TileController _BB;

    public void SetTileDatas(MetatileData metatileData)
    {
        _AA.SetTileData(metatileData.AA);
        _BA.SetTileData(metatileData.BA);
        _AB.SetTileData(metatileData.AB);
        _BB.SetTileData(metatileData.BB);
    }
    public void SetTilesets(Tileset tileset)
    {
        _AA.SetTileset(tileset);
        _BA.SetTileset(tileset);
        _AB.SetTileset(tileset);
        _BB.SetTileset(tileset);
    }
    public void SetPalettes(Palette palette)
    {
        _AA.SetPalette(palette);
        _BA.SetPalette(palette);
        _AB.SetPalette(palette);
        _BB.SetPalette(palette);
    }
    public void UpdateTileRenderers()
    {
        _AA.UpdateRenderer();
        _BA.UpdateRenderer();
        _AB.UpdateRenderer();
        _BB.UpdateRenderer();
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
