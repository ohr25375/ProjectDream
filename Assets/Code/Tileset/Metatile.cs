[System.Serializable]
public struct MetatileSpriteData
{
    public string name;
    public IndexableMetatileStruct<TileSpriteData> data;
}
[System.Serializable]
public struct MetatileMapData
{
    public int metatileIndex;
    public Palette palette;
    public IndexableMetatileStruct<TileCollisionData> data;
}

public struct MetatileData
{
    public Palette palette;
    public IndexableMetatileStruct<TileData> data;
    public MetatileData(MetatileMapData mapData, MetatileSpriteData spriteData)
    {
        palette = mapData.palette;
        data = new();
        for (var i = 0; i < 4; i++)
        {
            data[i] = new TileData(){ spriteData = spriteData.data[i], collisionData = mapData.data[i] };
        }
    }
}