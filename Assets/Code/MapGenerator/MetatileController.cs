using UnityEngine;

public class MetatileController : MonoBehaviour
{
    [SerializeField] private TileController _AA;
    [SerializeField] private TileController _BA;
    [SerializeField] private TileController _AB;
    [SerializeField] private TileController _BB;

    public void SetTileDatas(TileData AA, TileData BA, TileData AB, TileData BB)
    {
        _AA.SetTileData(AA);
        _BA.SetTileData(BA);
        _AB.SetTileData(AB);
        _BB.SetTileData(BB);
    }
    public void SetTilesets(Tileset tileset)
    {
        _AA.SetTileset(tileset);
        _BA.SetTileset(tileset);
        _AB.SetTileset(tileset);
        _BB.SetTileset(tileset);
    }
    public void SetTilesetsAndTileDatas(Tileset tileset, TileData AA, TileData BA, TileData AB, TileData BB)
    {
        SetTileDatas(AA, BA, AB, BB);
        SetTilesets(tileset);
    }
}
