[System.Serializable]
public enum Palette
{
    A,
    B,
    C,
    D
}

[System.Serializable]
public struct TileData
{
    public TileSpriteData spriteData;
    public TileCollisionFlags collisionData;
}

[System.Serializable]
public struct TileSpriteData
{
    public int spriteID;
    public bool isFlippedX;
    public bool isFlippedY;
}

[System.Serializable]
[System.Flags]
public enum TileCollisionFlags
{
    None = 0,
    IsCollidable = 1,
    IsInFront = 2,
    IsAirCollidable = 4,
}