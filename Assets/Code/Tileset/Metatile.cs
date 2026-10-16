[System.Serializable]
public struct MetatileSpriteData
{
    public string name;
    public TileSpriteData AA;
    public TileSpriteData BA;
    public TileSpriteData AB;
    public TileSpriteData BB;
}
[System.Serializable]
public struct MetatileMapData
{
    public int metatileIndex;
    public Palette palette;
    public TileCollisionData AA;
    public TileCollisionData BA;
    public TileCollisionData AB;
    public TileCollisionData BB;
}

public struct MetatileData
{
    public Palette palette;
    public TileData AA;
    public TileData BA;
    public TileData AB;
    public TileData BB;
    public MetatileData(MetatileMapData mapData, MetatileSpriteData spriteData)
    {
        palette = mapData.palette;
        AA = new TileData(){ spriteData = spriteData.AA, collisionData = mapData.AA };
        BA = new TileData(){ spriteData = spriteData.BA, collisionData = mapData.BA };
        AB = new TileData(){ spriteData = spriteData.AB, collisionData = mapData.AB };
        BB = new TileData(){ spriteData = spriteData.BB, collisionData = mapData.BB };
    }
}