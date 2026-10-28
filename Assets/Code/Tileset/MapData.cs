using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private Vector2Int size = new(16,15);
    public Vector2Int Size => size;
    public int MaxTileCount => size.x * size.y;
    [SerializeField] private List<MetatileMapData> metatileDatas = new();
    public List<MetatileMapData> MetatileDatas => metatileDatas;

    private void CorrectListSize<T>(List<T> list, int max) where T : struct
    {
        if (list.Count == max) return;
        if (list.Count < max)
        {
            for (int i = list.Count; i < max; i++)
            {
                list.Add(new T());
            }
        }
        else
        {
            for (int i = list.Count - 1; i >= max; i--)
            {
                list.RemoveAt(i);
            }
        }
    }

    private void CorrectMetatileDataSize()
    {
        var max = size.x * size.y;
        CorrectListSize(metatileDatas, max);
    }

    private void CorrectSize()
    {
        CorrectMetatileDataSize();
    }
    public void OnEnable()
    {
        CorrectSize();
    }

#if UNITY_EDITOR
    [SerializeField] private MetatileMemory e_metatileMemory;
    public MetatileMemory EditorMemory => e_metatileMemory;
    [SerializeField] private Tileset e_tileset;
    public Tileset EditorTileset => e_tileset;
    [SerializeField] private int e_changingMapIndex;
    [ContextMenu("Validate")]
    public void OnValidate()
    {
        CorrectSize();
    }
    public void ChangeMetatileIndex(int mapIndex, int metatileIndex)
    {
        var mdata = metatileDatas[mapIndex];
        mdata.metatileIndex = metatileIndex;
        metatileDatas[mapIndex] = mdata;
    }
    public void SetTileCollisionDatas(int mapIndex, IndexableMetatileStruct<TileCollisionData> tileCollisionDatas)
    {
        var mdata = metatileDatas[mapIndex];
        mdata.data = tileCollisionDatas;
        metatileDatas[mapIndex] = mdata;
    }
    public void ChangeMetatilePalette(int mapIndex, Palette palette)
    {
        var mdata = metatileDatas[mapIndex];
        mdata.palette = palette;
        metatileDatas[mapIndex] = mdata;
    }
    public void ClearMap()
    {
        metatileDatas = new();
        CorrectSize();
    }
#endif
}
