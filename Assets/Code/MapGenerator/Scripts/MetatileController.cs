using UnityEngine;
public class MetatileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer AA;
    [SerializeField] private SpriteRenderer BA;
    [SerializeField] private SpriteRenderer AB;
    [SerializeField] private SpriteRenderer BB;

    private readonly IndexableMetatileClass<TileController> _tileControllers = new();
    private void Awake()
    {
        IndexableMetatileClass<SpriteRenderer> spriteRenderers = new()
        {
            AA = AA,
            BA = BA,
            AB = AB,
            BB = BB,
        };

        Initialize(spriteRenderers);
    }
    public void Initialize(IndexableMetatileClass<SpriteRenderer> spriteRenderers)
    {
        for (var i = 0; i < 4; i++)
        {
            _tileControllers[i] = new();
            _tileControllers[i].Initialize(spriteRenderers[i]);
        }
    }

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
