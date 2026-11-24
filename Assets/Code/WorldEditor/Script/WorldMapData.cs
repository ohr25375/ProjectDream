using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldMapData
{
    [SerializeField] private MapData mapData;
    public readonly MapData MapData => mapData;
    [SerializeField] private IndexableDirectionalStruct<Exit> exits;
    public readonly IndexableDirectionalStruct<Exit> Exits => exits;
    [SerializeField] private List<MapEventTriggerSet> mapEventTriggers;
    public readonly List<MapEventTriggerSet> MapEventTriggers => mapEventTriggers;
}

[System.Serializable]
public class Exit
{
    public int UnlockTriggerIndex = -1;
    public int ExitIndex;
    [Tooltip("in tiles (if metatile, x2)")]
    public int ExitOffset;
}
[System.Serializable]
public class MapEventTriggerSet
{
    [SerializeField] private Vector2Int position;
    public Vector2Int Position => position;
    [SerializeReference, SubclassSelector] private MapEventTrigger mapEventTrigger = new MapEventTrigger_DebugLogger();
    public MapEventTrigger MapEventTrigger => mapEventTrigger;
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

public static class CompassDirectionsExtensions
{
    public static Direction ToDirection(this CompassDirections compassDirection) => (Direction)compassDirection;
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

    public T this[CompassDirections direction]
    {
        readonly get
        {
            return direction switch
            {
                CompassDirections.North => North,
                CompassDirections.South => South,
                CompassDirections.East => East,
                CompassDirections.West => West,
                _ => throw new System.IndexOutOfRangeException(),
            };
        }
        set
        {
            switch (direction)
            {
                case CompassDirections.North: North = value; break;
                case CompassDirections.South: South = value; break;
                case CompassDirections.East: East = value; break;
                case CompassDirections.West: West = value; break;
                default: throw new System.IndexOutOfRangeException();
            }
        }
    }
}