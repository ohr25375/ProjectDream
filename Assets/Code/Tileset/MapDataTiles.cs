using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class MapDataTiles
{
    private Vector2Int size = new(16,15);
    public int MaxTileCount => size.x * size.y;
    [SerializeField] private List<MetatileMapData> metatileDatas = new();
    public List<MetatileMapData> MetatileDatas => metatileDatas;
    public ReadOnlyCollection<bool> GetColliders(TileCollisionFlags collisionFlags)
    {
        List<bool> colliders = new(new bool[MaxTileCount * 4]);
        for (var my = 0; my < size.y; my++)
        {
            for (var mx = 0; mx < size.x; mx++)
            {
                var mi = mx + my * size.x;
                for (var i = 0; i < 4; i++)
                {
                    var tx = i & 1;
                    var ty = 1 - (i >> 1 & 1);
                    var index = (mx * 2 + tx) + ((my * size.x * 4) + (ty * size.x * 2));
                    colliders[index] = (metatileDatas[mi].data[i] & collisionFlags) != 0;
                }
            }
        }
        return colliders.AsReadOnly();
    }

    #region UNITY
    public void OnEnable()
    {
        CorrectSize(size);
    }
    #endregion

    #region INTERNAL
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
    public void CorrectSize(Vector2Int size)
    {
        this.size = size;
        CorrectMetatileDataSize();
    }
    #endregion

    #region EDITOR
#if UNITY_EDITOR
    public void ChangeMetatileIndex(int mapIndex, int metatileIndex)
    {
        var mdata = metatileDatas[mapIndex];
        mdata.metatileIndex = metatileIndex;
        metatileDatas[mapIndex] = mdata;
    }
    public void SetTileCollisionDatas(int mapIndex, IndexableMetatileStruct<TileCollisionFlags> tileCollisionDatas)
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
        CorrectSize(size);
    }
#endif
    #endregion
}
