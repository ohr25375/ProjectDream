using System;
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
    [ContextMenu("Validate")]
    public void OnValidate()
    {
        CorrectSize();
    }
#endif
}
