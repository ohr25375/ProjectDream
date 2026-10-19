using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MetatileMemory", menuName = "Scriptable Objects/MetatileMemory")]
public class MetatileMemory : ScriptableObject
{
    [SerializeField] private List<MetatileSpriteData> metatiles;
    public int Max => metatiles.Count;

    public MetatileSpriteData GetData(int index)
    {
        if (index < 0 || index >= metatiles.Count)
        {
            return metatiles[0];
        }
        return metatiles[index];
    }
}
