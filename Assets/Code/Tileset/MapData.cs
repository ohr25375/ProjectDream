using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private MapDataTiles _mapTileData;
    private MapDataTiles MapTileData
    {
        get
        {
            _mapTileData ??= new();
            return _mapTileData;
        }
    }
    [SerializeField] private Vector2Int size = new(16, 15);
    public Vector2Int Size => size;
    public List<MetatileMapData> MetatileDatas => MapTileData.MetatileDatas;
    public ReadOnlyCollection<bool> GetColliders(TileCollisionFlags collisionFlags) => MapTileData.GetColliders(collisionFlags);

    #region EDITOR
#if UNITY_EDITOR
    [SerializeField] private MetatileMemory e_metatileMemory;
    public MetatileMemory EditorMemory => e_metatileMemory;
    [SerializeField] private Tileset e_tileset;
    public Tileset EditorTileset => e_tileset;
    [SerializeField] private int e_changingMapIndex;
    public MapDataTiles EditorMapTileData => MapTileData;
    [ContextMenu("Validate")]
    public void OnValidate()
    {
        MapTileData.CorrectSize(size);
    }
#endif
    #endregion
}
