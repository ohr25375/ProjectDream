using System;
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

#if UNITY_EDITOR
    [SerializeField] private Tileset e_tileset;
    public Tileset EditorTileset => e_tileset;
    public void EditorModifyTileSpriteID(int index, TileIdentifier subIndex, int spriteIndex)
    {
        var largeData = metatiles[index];
        var data = largeData.data[(int)subIndex];
        data.spriteID = spriteIndex;
        largeData.data[(int)subIndex] = data;
        metatiles[index] = largeData;
    }
    public void EditorModifyMetatileName(int index, string metatileName)
    {
        var largeData = metatiles[index];
        largeData.name = metatileName;
        metatiles[index] = largeData;
    }
    public void EditorInsertData(int index)
    {
        var data = GetData(index);
        var suffixStartPosition = data.name.Length;
        while (suffixStartPosition > 0 && char.IsDigit(data.name[suffixStartPosition - 1]))
        {
            suffixStartPosition--;
        }
        var prefix = data.name[..suffixStartPosition];
        var suffix = " 0";
        if (data.name.Length - suffixStartPosition > 0)
        {
            suffix = $"{int.Parse(data.name[suffixStartPosition..]) + 1}";
        }
        data.name = prefix + suffix;
        metatiles.Insert(index + 1, data);
    }
    public void EditorDeleteData(int index)
    {
        metatiles.RemoveAt(index);
    }
#endif
}
