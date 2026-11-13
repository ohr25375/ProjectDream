using System.Collections.Generic;
using UnityEditor;
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
    [SerializeReference] public List<Texture> EditorTextureCache = new();
    private void OnValidate()
    {
        RefreshCache();
    }
    [ContextMenu("Refresh Cache")]
    public void RefreshCache()
    {
        EditorTextureCache = new(new Texture[e_tileset.Max]);
        for (var i = 0; i < e_tileset.Max; i++)
        {
            EditorTextureCache[i] = AssetPreview.GetAssetPreview(e_tileset.GetSprite(i));
        }
    }
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
        data.name += "1";
        metatiles.Insert(index + 1, data);
    }
    public void EditorDeleteData(int index)
    {
        metatiles.RemoveAt(index);
    }
#endif
}
