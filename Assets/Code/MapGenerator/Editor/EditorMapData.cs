using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapData))]
public class EditorMapData : Editor
{
    [System.Serializable]
    private enum EditorOverlay
    {
        None,
        Palette,
        Collider,
    };

    private MapData _mapData;
    private MapDataTiles _mapTileData;
    private bool _editorFoldout = true;
    private Palette _palette = Palette.A;
    private int _tileIdentifier;
    private IndexableMetatileStruct<TileCollisionFlags> _tileCollisionDatas;
    private int _metatileIndex = 0;
    private bool _clearMapDataSafety = false;
    private int _lastSelectedMapTile = -1;
    private EditorOverlay _editorOverlayMode;
    private float _overlayOpacity = 0.5f;
    private float _editorMagnification = 1.0f;
    public enum PreviewMode
    {
        Editor,
        Sprite,
        Collisions
    };
    public static PreviewMode GlobalDefaultPreviewMode = PreviewMode.Sprite;
    private PreviewMode _previewMode = GlobalDefaultPreviewMode;
    private void OnEnable()
    {
        _palette = Palette.A;
        _tileCollisionDatas = new();
        _metatileIndex = 0;
    }
    private void DrawCollisions(Rect rect, float alpha = 1, float magnification = 1)
    {
        for (var y = 0; y < _mapData.Size.y; y++)
        {
            for (var x = 0; x < _mapData.Size.x; x++)
            {
                var mapIndex = x + y * _mapData.Size.x;
                var position = new Vector2Int(x, _mapData.Size.y - 1 - y);
                DrawCollisionSet(position, rect.position, mapIndex, alpha, magnification);
            }
        }
    }
    private void DrawMetatiles(Vector2Int mapSize, List<MetatileMapData> metatileDatas, float magnification)
    {
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var mapIndex = x + y * mapSize.x;
                var metatileIndex = metatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, mapSize.y - 1 - y);
                EditorExtensions.DrawMetatile(_mapData.EditorTileset, _mapData.EditorMemory.GetData(metatileIndex), position, rt.position, magnification);
            }
        }
    }
    private void DrawMetatilesAsOne(Rect rect, Vector2Int mapSize, List<MetatileMapData> metatileDatas, float magnification)
    {
        if (_lastSelectedMapTile != -1)
        {
            var sx = _lastSelectedMapTile % mapSize.x;
            var sy = _lastSelectedMapTile / mapSize.x;
            Rect selectedRect = new()
            {
                x = rect.x + sx * 16 * magnification - 1,
                y = rect.y + sy * 16 * magnification,
                width = 16 * magnification + 1,
                height = 16 * magnification + 1,
            };
            EditorGUI.DrawRect(selectedRect, Color.green);
        }
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var mapIndex = x + y * mapSize.x;
                var metatileIndex = metatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, mapSize.y - 1 - y);
                EditorExtensions.DrawMetatileAsOne(_mapData.EditorTileset, _mapData.EditorMemory.GetData(metatileIndex), position, rect.position, magnification);
            }
        }
    }
    private void DrawPaletteMetatile(Palette palette, Rect rect, Vector2Int mapSize, int x, int y, float magnification)
    {
        Rect selectedRect = new()
        {
            x = rect.x + x * 16 * magnification,
            y = rect.y + (mapSize.y - 1 - y) * 16 * magnification + 1,
            width = 15 * magnification,
            height = 15 * magnification,
        };
        var color = Color.black;
        switch (palette)
        {
            case Palette.A: color = Color.red; break;
            case Palette.B: color = Color.green; break;
            case Palette.C: color = Color.cyan; break;
            case Palette.D: color = Color.magenta; break;
        }
        color.a = _overlayOpacity;
        EditorGUI.DrawRect(selectedRect, color);
    }
    private void DrawBasicPaletteOverlay(Rect rect, Vector2Int mapSize, List<MetatileMapData> metatileDatas, float magnification)
    {
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var mapIndex = x + y * mapSize.x;
                var paletteIndex = metatileDatas[mapIndex].palette;
                DrawPaletteMetatile(paletteIndex, rect, mapSize, x, y, magnification);
            }
        }
    }
    private void DrawCollision(Vector2 position, Vector2 offset, TileCollisionFlags collisionData, float alpha, float magnification)
    {
        Rect rect = new(offset + 8 * magnification * position, 8 * magnification * Vector2.one);
        Color color = new()
        {
            r = collisionData.HasFlag(TileCollisionFlags.IsCollidable) ? 1 : 0,
            g = 0,
            b = collisionData.HasFlag(TileCollisionFlags.IsAirCollidable) ? 1 : 0,
        };
        color.a = alpha * Mathf.Max(color.r, color.b);
        EditorGUI.DrawRect(rect, color);
    }
    private void DrawCollisionSet(Vector2Int position, Vector2 offset, int mapIndex, float alpha, float magnification)
    {
        MetatileMapData metatileMapData = _mapTileData.MetatileDatas[mapIndex];
        for (var i = 0; i < 4; i++)
        {
            var pos = position * 2 + new Vector2(i % 2, i / 2);
            DrawCollision(pos, offset, metatileMapData.data[i], alpha, magnification);
        }
    }
    private void DrawMapEditor()
    {
        //Debug.Log(GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none));
        var rect = EditorGUILayout.BeginVertical();
        GUIStyle style = new(GUI.skin.button)
        {
            fixedWidth = 16 * _editorMagnification,
            fixedHeight = 16 * _editorMagnification,
            margin = new(0, 0, 0, 0),
            border = new(0, 0, 0, 0),
            padding = new(0, 0, 0, 0),
            contentOffset = new(0, 0),
            overflow = new(0, 0, 0, 0)
        };
        var mapTileCount = _mapData.Size.x * _mapData.Size.y;
        var selectedTile = GUILayout.SelectionGrid(-1, new string[mapTileCount], _mapData.Size.x, style);
        DrawMetatilesAsOne(rect, _mapData.Size, _mapData.MetatileDatas, _editorMagnification);
        switch (_editorOverlayMode)
        {
            default: break;
            case EditorOverlay.Palette: DrawBasicPaletteOverlay(rect, _mapData.Size, _mapData.MetatileDatas, _editorMagnification); break;
            case EditorOverlay.Collider: DrawCollisions(rect, _overlayOpacity, _editorMagnification); break;
        }
        GUILayout.BeginHorizontal();
        _clearMapDataSafety = GUILayout.Toggle(_clearMapDataSafety, "", GUILayout.ExpandWidth(false));
        if (!_clearMapDataSafety) GUI.enabled = false;
        if (GUILayout.Button($"Clear Map{(_clearMapDataSafety ? "" : " (Disable safety first)")}")) _mapTileData.ClearMap();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUIStyle saveStyle = new(GUI.skin.button);
        bool isDirty = EditorUtility.IsDirty(target);
        GUI.enabled = isDirty;
        if (GUILayout.Button(isDirty ? "### SAVE ASSET ###" : "Asset is saved", saveStyle)) SaveAsset();
        GUI.enabled = true;
        //_selectedTile = GUILayout.SelectionGrid(_selectedTile, new Texture[_mapdata.MaxTileCount], _mapdata.Size.x, style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        _metatileIndex = EditorExtensions.DrawMetatileSelector(_mapData.EditorMemory, _metatileIndex, ref _tileIdentifier, out Rect selectorRect);
        switch (_editorOverlayMode)
        {
            default: break;
            case EditorOverlay.Palette:
                {
                    selectorRect.position += Vector2.right;
                    DrawPaletteMetatile(_palette, selectorRect, _mapData.Size, 0, _mapData.Size.y - 1, 2);
                    DrawPaletteMetatile(_palette, selectorRect, _mapData.Size, 1, _mapData.Size.y - 1, 2);
                    DrawPaletteMetatile(_palette, selectorRect, _mapData.Size, 0, _mapData.Size.y - 2, 2);
                    DrawPaletteMetatile(_palette, selectorRect, _mapData.Size, 1, _mapData.Size.y - 2, 2);
                    break;
                }
            case EditorOverlay.Collider:
                {
                    DrawCollision(new(0, 0), selectorRect.position, _tileCollisionDatas[0], _overlayOpacity, 4);
                    DrawCollision(new(1, 0), selectorRect.position, _tileCollisionDatas[1], _overlayOpacity, 4);
                    DrawCollision(new(0, 1), selectorRect.position, _tileCollisionDatas[2], _overlayOpacity, 4);
                    DrawCollision(new(1, 1), selectorRect.position, _tileCollisionDatas[3], _overlayOpacity, 4);
                    break;
                }
        }
        GUILayout.BeginVertical();
        _editorOverlayMode = (EditorOverlay)EditorGUILayout.EnumPopup(_editorOverlayMode);
        if (_editorOverlayMode == EditorOverlay.None)
        {
            GUI.enabled = false;
        }
        _overlayOpacity = EditorGUILayout.Slider(_overlayOpacity, 0, 1);
        GUI.enabled = true;
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField($"Name: {_mapData.EditorMemory.GetData(_metatileIndex).name}");
        if (selectedTile != -1)
        {
            UpdateMapData(selectedTile);
            _lastSelectedMapTile = selectedTile;
        }
        string[] palettePopupTexts = { "A", "B", "C", "D" };
        if (_editorOverlayMode == EditorOverlay.Palette)
        {
            palettePopupTexts = new string[]
            {
                "A (Red)",
                "B (Green)",
                "C (Cyan)",
                "D (Magenta)"
            };
        }
        _palette = (Palette)EditorGUILayout.Popup(new GUIContent("Palette: "), (int)_palette, palettePopupTexts);
        _tileCollisionDatas[_tileIdentifier] = (TileCollisionFlags)EditorGUILayout.EnumFlagsField(_tileCollisionDatas[_tileIdentifier]);
        var applyToAll = GUILayout.Button("Copy collision to all subtiles");
        if (applyToAll)
        {
            for (var i = 0; i < 4; i++)
            {
                _tileCollisionDatas[i] = _tileCollisionDatas[_tileIdentifier];
            }
        }
        var selectedMetatile = EditorExtensions.DrawMetatileMemories(_mapData.EditorMemory, _metatileIndex, _editorMagnification);
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
        EditorUtility.SetDirty(target);
    }
    private void SaveAsset()
    {
        AssetDatabase.SaveAssetIfDirty(target);
    }

    private void DrawPreview()
    {
        if (!_editorFoldout) return;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_metatileMemory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_tileset"));
        GUIStyle errorStyle = new(GUI.skin.label) { normal = new() { textColor = Color.red } };
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
        var selectedPreviewMode = (PreviewMode)EditorGUILayout.EnumPopup(new GUIContent("Preview Mode", "Change display mode of map preview"), _previewMode);
        if (selectedPreviewMode != _previewMode)
        {
            _previewMode = selectedPreviewMode;
            GlobalDefaultPreviewMode = selectedPreviewMode;
        }
        bool isEditorZoomed = EditorGUILayout.Toggle("x2", _editorMagnification > 1.0f);
        _editorMagnification = isEditorZoomed ? 2.0f : 1.0f;
        Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(_mapData.Size.y * 16));
        switch (_previewMode)
        {
            case PreviewMode.Editor: DrawMapEditor(); break;
            case PreviewMode.Sprite: DrawMetatiles(_mapData.Size, _mapData.MetatileDatas, _editorMagnification); break;
            case PreviewMode.Collisions: DrawCollisions(rect, 1, _editorMagnification); break;
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
