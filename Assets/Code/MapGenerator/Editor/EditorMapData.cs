using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapData))]
public class EditorMapData : Editor
{
    private MapData _mapData;
    private MapDataTiles _mapTileData;
    private bool _editorFoldout = true;
    private Palette _palette = Palette.A;
    private int _tileIdentifier;
    private IndexableMetatileStruct<TileCollisionFlags> _tileCollisionDatas;
    private int _metatileIndex = 0;
    private bool _clearMapDataSafety = false;
    private enum PreviewMode
    {
        Editor,
        Sprite,
        Collisions
    };
    private PreviewMode _previewMode = PreviewMode.Sprite;
    private void OnEnable()
    {
        _palette = Palette.A;
        _tileCollisionDatas = new();
        _metatileIndex = 0;
    }
    private void DrawCollisions()
    {
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < _mapData.Size.y; y++)
        {
            for (var x = 0; x < _mapData.Size.x; x++)
            {
                var mapIndex = x + y * _mapData.Size.x;
                var position = new Vector2(x, _mapData.Size.y - 1 - y);
                DrawCollisionSet(position, rt.position, mapIndex);
            }
        }
    }
    private void DrawMetatiles(Vector2Int mapSize, List<MetatileMapData> metatileDatas)
    {
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var mapIndex = x + y * mapSize.x;
                var metatileIndex = metatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, mapSize.y - 1 - y);
                EditorExtensions.DrawMetatile(_mapData.EditorTileset, _mapData.EditorMemory.GetData(metatileIndex), position, rt.position);
            }
        }
    }
    private void DrawMetatilesAsOne(Rect rect, Vector2Int mapSize, List<MetatileMapData> metatileDatas)
    {
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var mapIndex = x + y * mapSize.x;
                var metatileIndex = metatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, mapSize.y - 1 - y);
                EditorExtensions.DrawMetatileAsOne(_mapData.EditorTileset, _mapData.EditorMemory.GetData(metatileIndex), position, rect.position);
            }
        }
    }
    private void DrawCollision(Vector2 position, Vector2 offset, TileCollisionFlags collisionData)
    {
        Rect rect = new(position * 16 + offset, Vector2.one * 8);
        Color color = new(collisionData.HasFlag(TileCollisionFlags.IsCollidable) ? 1 : 0, 0, collisionData.HasFlag(TileCollisionFlags.IsAirCollidable) ? 1 : 0);
        EditorGUI.DrawRect(rect, color);
    }
    private void DrawCollisionSet(Vector2 position, Vector2 offset, int mapIndex)
    {
        MetatileMapData metatileMapData = _mapTileData.MetatileDatas[mapIndex];
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawCollision(pos, offset, metatileMapData.data[i]);
        }
    }
    private void DrawMapEditor()
    {
        //Debug.Log(GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none));
        var rect = EditorGUILayout.BeginVertical();
        GUIStyle style = new(GUI.skin.button)
        {
            fixedWidth = 16,
            fixedHeight = 16,
            margin = new(0, 0, 0, 0),
            border = new(0, 0, 0, 0),
            padding = new(0, 0, 0, 0),
            contentOffset = new(0, 0),
            overflow = new(0, 0, 0, 0)
        };
        var mapTileCount = _mapData.Size.x * _mapData.Size.y;
        var selectedTile = GUILayout.SelectionGrid(-1, new string[mapTileCount], _mapData.Size.x, style);
        DrawMetatilesAsOne(rect, _mapData.Size, _mapData.MetatileDatas);
        GUILayout.BeginHorizontal();
        _clearMapDataSafety = GUILayout.Toggle(_clearMapDataSafety, "", GUILayout.ExpandWidth(false));
        if (!_clearMapDataSafety) GUI.enabled = false;
        if (GUILayout.Button($"Clear Map{(_clearMapDataSafety ? "": " (Disable safety first)")}")) _mapTileData.ClearMap();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Save Asset")) SaveAsset();

        //_selectedTile = GUILayout.SelectionGrid(_selectedTile, new Texture[_mapdata.MaxTileCount], _mapdata.Size.x, style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        _metatileIndex = EditorExtensions.DrawMetatileSelector(_mapData.EditorMemory, _metatileIndex, ref _tileIdentifier);
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField($"Name: {_mapData.EditorMemory.GetData(_metatileIndex).name}");
        if (selectedTile != -1)
        {
            UpdateMapData(selectedTile);
        }
        _palette = (Palette)EditorGUILayout.EnumPopup(new GUIContent("Palette", "Metatile palette"), _palette);
        _tileCollisionDatas[_tileIdentifier] = (TileCollisionFlags)EditorGUILayout.EnumFlagsField(_tileCollisionDatas[_tileIdentifier]);
        var applyToAll = GUILayout.Button("Copy collision to all subtiles");
        if (applyToAll)
        {
            for (var i = 0; i < 4; i++)
            {
                _tileCollisionDatas[i] = _tileCollisionDatas[_tileIdentifier];
            }
        }
        var selectedMetatile = EditorExtensions.DrawMetatileMemories(_mapData.EditorMemory, _metatileIndex);
        if (selectedMetatile != -1)
        {
            _metatileIndex = selectedMetatile;
        }
        EditorGUILayout.EndVertical();
    }

    private void UpdateMapData(int selectedTile)
    {
        int selectedX = selectedTile % _mapData.Size.x;
        int selectedY = selectedTile / _mapData.Size.x;
        int mapY = _mapData.Size.y - 1 - selectedY;
        int mapIndex = selectedX + mapY * _mapData.Size.x;
        _mapTileData.ChangeMetatileIndex(mapIndex, _metatileIndex);
        _mapTileData.SetTileCollisionDatas(mapIndex, _tileCollisionDatas);
        _mapTileData.ChangeMetatilePalette(mapIndex, _palette);
        selectedTile = -1;
    }
    private void SaveAsset()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssetIfDirty(target);
    }

    private void DrawPreview()
    {
        if (!_editorFoldout) return;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_metatileMemory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_tileset"));
        GUIStyle errorStyle = new(GUI.skin.label){ normal = new(){ textColor = Color.red } };
        if (_mapData.EditorMemory == null)
        {
            EditorGUILayout.LabelField("missing Metatile Memory", errorStyle);  
            return;
        }
        if (_mapData.EditorTileset == null)
        {
            EditorGUILayout.LabelField("missing Tileset", errorStyle);
            return;
        }
        _previewMode = (PreviewMode)EditorGUILayout.EnumPopup(new GUIContent("Preview Mode", "Change display mode of map preview"), _previewMode);
        GUILayout.BeginHorizontal(GUILayout.Height(_mapData.Size.y * 16));
        switch (_previewMode)
        {
            case PreviewMode.Editor: DrawMapEditor(); break;
            case PreviewMode.Sprite: DrawMetatiles(_mapData.Size, _mapData.MetatileDatas); break;
            case PreviewMode.Collisions: DrawCollisions(); break;
        }

        GUILayout.EndHorizontal();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _mapData = target as MapData;
        _mapTileData = _mapData.EditorMapTileData;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
        _editorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_editorFoldout, "Preview");
        DrawPreview();
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
