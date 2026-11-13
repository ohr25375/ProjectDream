using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MetatileMemory", menuName = "Scriptable Objects/MetatileMemory")]
public class MetatileMemory : ScriptableObject
{
    [SerializeField] private List<MetatileSpriteData> metatiles;
    public int Max => metatiles.Count;

    public MetatileSpriteData GetData(int index, bool forceNull = false)
    {
        if (index < 0 || index >= metatiles.Count) return metatiles[0];
        var msd = metatiles[index];
        if (forceNull)
        {
            return msd;
        }
        else
        {
            return msd.name == "" ? metatiles[0] : msd;
        }
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
        if (metatileName == "")
        {
            metatileName = " ";
        }
        largeData.name = metatileName;
        metatiles[index] = largeData;
    }
    public int EditorInsertData(int index)
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
        if (metatiles.Exists(x => x.name == ""))
        {
            var dataSlot = metatiles.FindIndex(x => x.name == "");
            metatiles[dataSlot] = data;
            return dataSlot;
        }
        metatiles.Add(data);
        return metatiles.Count - 1;
    }
    public void EditorDeleteData(int index)
    {
        if (index == 0) return;
        var data = metatiles[index];
        data.name = "";
        metatiles[index] = data;
    }
    private void Reset()
    {
        metatiles = new();
        MetatileSpriteData msd = new()
        {
            name = "NULL",
            data = new(),
        };
        metatiles.Add(msd);
    }
    public int GetDisabledCount()
    {
        return metatiles.Where(x => x.name == "").Count();
    }
#endif
}
