using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapData))]
public class EditorMapData : Editor
{
    private MapData _mapData;
    private MapDataTiles _mapTileData;
    private bool _editorFoldout = true;
    private List<Texture> _buttonTextures = new();
    private bool _hasValidated = true;
    private Palette _palette = Palette.A;
    private TileIdentifier _tileIdentifier;
    private IndexableMetatileStruct<TileCollisionFlags> _tileCollisionDatas;
    private int _metatileIndex = 0;
    private enum PreviewMode
    {
        Editor,
        Sprite,
        Collisions
    };
    private PreviewMode _previewMode = PreviewMode.Sprite;
    private List<Texture> GetButtonTextures()
    {
        if (_hasValidated)
        {
            _buttonTextures = new(new Texture[_mapTileData.MaxTileCount]);
            for (var y = 0; y < _mapData.Size.y; y++)
            {
                for (var x = 0; x < _mapData.Size.x; x++)
                {
                    int index = x + y * _mapData.Size.x;
                    int textureIndex = x + (_mapData.Size.y - 1 - y) * _mapData.Size.x;
                    int metatileIndex = _mapTileData.MetatileDatas[index].metatileIndex;
                    var tileSpriteDatas = _mapData.EditorMemory.GetData(metatileIndex).data;
                    Sprite sprite = _mapData.EditorTileset.GetSprite(tileSpriteDatas[0].spriteID);
                    _buttonTextures[textureIndex] = AssetPreview.GetAssetPreview(sprite);
                }
            }
        }
        return _buttonTextures;
    }
    private void OnEnable()
    {
        _hasValidated = true;
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
        GUILayout.BeginVertical();
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
        var selectedTile = GUILayout.SelectionGrid(-1, GetButtonTextures().ToArray(), _mapData.Size.x, style);
        if (GUILayout.Button("Clear Map")) _mapTileData.ClearMap();
        if (GUILayout.Button("Save Asset")) SaveAsset();

        //_selectedTile = GUILayout.SelectionGrid(_selectedTile, new Texture[_mapdata.MaxTileCount], _mapdata.Size.x, style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        DrawMetatileSelector();
        EditorGUILayout.LabelField($"Name: {_mapData.EditorMemory.GetData(_metatileIndex).name}");
        GUILayout.EndHorizontal();
        float magnify = 4;
        Rect metatilePreviewRect = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none, GUILayout.MinWidth(magnify * 8 * 2), GUILayout.MinHeight(magnify * 8 * 2));
        metatilePreviewRect.x += 20;
        Rect tileIdentifierRect = new(metatilePreviewRect)
        {
            width = magnify * 8,
            height = magnify * 8,
        };
        tileIdentifierRect.position += 8 * magnify * new Vector2((int)_tileIdentifier % 2, (int)_tileIdentifier / 2);
        EditorGUI.DrawRect(tileIdentifierRect, Color.red);
        EditorExtensions.DrawMetatile(_mapData.EditorTileset, _mapData.EditorMemory.GetData(_metatileIndex), Vector2.zero, metatilePreviewRect.position, magnify, (magnify * 8 - 2) / (magnify * 8));
        if (selectedTile != -1)
        {
            UpdateMapData(selectedTile);
        }
        _palette = (Palette)EditorGUILayout.EnumPopup(new GUIContent("Palette", "Metatile palette"), _palette);
        _tileIdentifier = (TileIdentifier)EditorGUILayout.EnumPopup(new GUIContent("Tile", "Which tile to edit"), _tileIdentifier);
        _tileCollisionDatas[(int)_tileIdentifier] = (TileCollisionFlags)EditorGUILayout.EnumFlagsField(_tileCollisionDatas[(int)_tileIdentifier]);
        var applyToAll = GUILayout.Button("Apply to all");
        if (applyToAll)
        {
            for (var i = 0; i < 4; i++)
            {
                _tileCollisionDatas[i] = _tileCollisionDatas[(int)_tileIdentifier];
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawMetatileSelector()
    {
        bool isPreviousPressed = GUILayout.Button("Previous");
        bool isNextPressed = GUILayout.Button("Next");
        int inc = isPreviousPressed ? -1 : isNextPressed ? 1 : 0;
        _metatileIndex = (_metatileIndex + _mapData.EditorMemory.Max + inc) % _mapData.EditorMemory.Max;
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
        _hasValidated = true;
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
    public void OnValidate()
    {
        _hasValidated = true;
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
