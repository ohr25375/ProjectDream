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
    public TileCollisionData collisionData;
}

[System.Serializable]
public struct TileSpriteData
{
    public int spriteID;
    public bool isFlippedX;
    public bool isFlippedY;
}

[System.Serializable]
public struct TileCollisionData
{
    public bool IsCollidable;
    public bool IsInFront;
    public bool IsAirCollidable;
}