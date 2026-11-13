using UnityEngine;

[System.Serializable]
public struct WorldMapData
{
    [SerializeField] private MapData mapData;
    public readonly MapData MapData => mapData;
    [SerializeField] private IndexableDirectionalStruct<int> exits;
    public readonly IndexableDirectionalStruct<int> Exits => exits;
}

[System.Serializable]
public enum CompassDirections
{
    None,
    West,
    East,
    North,
    South,
}

[System.Serializable]
public struct IndexableDirectionalStruct<T>
{
    public T North, South, East, West;
    public T this[int i]
    {
        readonly get
        {
            return i switch
            {
                (int)CompassDirections.North => North,
                (int)CompassDirections.South => South,
                (int)CompassDirections.East => East,
                (int)CompassDirections.West => West,
                _ => throw new System.IndexOutOfRangeException(),
            };
        }
        set
        {
            switch (i)
            {
                case (int)CompassDirections.North : North = value; break;
                case (int)CompassDirections.South : South = value; break;
                case (int)CompassDirections.East : East = value; break;
                case (int)CompassDirections.West: West = value; break;
                default: throw new System.IndexOutOfRangeException();
            }
        }
    }
}