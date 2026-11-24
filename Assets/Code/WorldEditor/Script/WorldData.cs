using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Scriptable Objects/WorldData")]
public class WorldData : ScriptableObject
{
    [SerializeField] private List<WorldMapData> mapDatas;
    public int Count => mapDatas.Count;
    public WorldMapData GetWorldMapData(int index)
    {
        if (index < 0 || index >= mapDatas.Count) return mapDatas[0];
        return mapDatas[index];
    }
    public bool IsNextMapReachable(int currentIndex, Direction direction)
    {
        var nextIndex = mapDatas[currentIndex].Exits[(int)direction].ExitIndex;
        return nextIndex >= 0 && nextIndex < mapDatas.Count;
    }
    public int GetNextIndex(int currentIndex, Direction direction)
    {
        return mapDatas[currentIndex].Exits[(int)direction].ExitIndex;
    }
}
