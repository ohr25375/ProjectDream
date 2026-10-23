using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MapData))]
public class EditorMapData : Editor
{
    private MapData _mapdata;
    private bool _editorFoldout = true;
    private List<Texture> _buttonTextures = new();
    private int _selectedTile = -1;
    private bool _hasValidated = true;
    private Palette _palette = Palette.A;
    private TileIdentifier _tileIdentifier;
    private IndexableMetatileStruct<TileCollisionData> _tileCollisionDatas;
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
            _buttonTextures = new(new Texture[_mapdata.MaxTileCount]);
            for (var y = 0; y < _mapdata.Size.y; y++)
            {
                for (var x = 0; x < _mapdata.Size.x; x++)
                {
                    int index = x + y * _mapdata.Size.x;
                    int textureIndex = x + (_mapdata.Size.y - 1 - y) * _mapdata.Size.x;
                    int metatileIndex = _mapdata.MetatileDatas[index].metatileIndex;
                    var tileSpriteDatas = _mapdata.EditorMemory.GetData(metatileIndex).data;
                    Sprite sprite = _mapdata.EditorTileset.GetSprite(tileSpriteDatas[0].spriteID);
                    _buttonTextures[textureIndex] = AssetPreview.GetAssetPreview(sprite);
                }
            }
        }
        return _buttonTextures;
    }
    private void OnEnable()
    {
        _selectedTile = -1;
        _hasValidated = true;
        _palette = Palette.A;
        _tileCollisionDatas = new();
        _metatileIndex = 0;
    }
    private void DrawTile(Vector2 position, Vector2 offset, TileSpriteData spriteData, float magnify = 1, float size = 1)
    {
        Sprite sprite = _mapdata.EditorTileset.GetSprite(spriteData.spriteID);
        bool flipX = spriteData.isFlippedX;
        bool flipY = spriteData.isFlippedY;
        if (sprite == null)
        {
            sprite = _mapdata.EditorTileset.GetSprite(0);
        }
        Texture2D spritePreview = AssetPreview.GetAssetPreview(sprite);
        if (spritePreview == null) return;
        Rect rect = new()
        {
            x = 16 * magnify * position.x + 8 * magnify * (flipX ? 1 : 0) + offset.x + (1 - size) / 2 * 8 * magnify * (flipX ? -1 : 1),
            y = 16 * magnify * position.y + 8 * magnify * (flipY ? 1 : 0) + offset.y + (1 - size) / 2 * 8 * magnify * (flipY ? -1 : 1),
            width = (flipX ? -1 : 1) * 8 * magnify * size,
            height = (flipY ? -1 : 1) * 8 * magnify * size
        };
        GUI.DrawTexture(rect, spritePreview);
    }
    private void DrawMetatile(Vector2 position, Vector2 offset, int metatileIndex, float magnify = 1, float size = 1)
    {
        var tileSpriteDatas = _mapdata.EditorMemory.GetData(metatileIndex).data;
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawTile(pos, offset, tileSpriteDatas[i], magnify, size);
        }
    }
    private void DrawCollisions()
    {
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < _mapdata.Size.y; y++)
        {
            for (var x = 0; x < _mapdata.Size.x; x++)
            {
                var mapIndex = x + y * _mapdata.Size.x;
                var position = new Vector2(x, _mapdata.Size.y - 1 - y);
                DrawCollisionSet(position, rt.position, mapIndex);
            }
        }
    }
    private void DrawMetatiles()
    {
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < _mapdata.Size.y; y++)
        {
            for (var x = 0; x < _mapdata.Size.x; x++)
            {
                var mapIndex = x + y * _mapdata.Size.x;
                var metatileIndex = _mapdata.MetatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, _mapdata.Size.y - 1 - y);
                DrawMetatile(position, rt.position, metatileIndex);
            }
        }
    }
    private void DrawCollision(Vector2 position, Vector2 offset, TileCollisionData collisionData)
    {
        Rect rect = new(position * 16 + offset, Vector2.one * 8);
        Color color = new(collisionData.isCollidable ? 1 : 0, 0, collisionData.isAirCollidable ? 1 : 0);
        EditorGUI.DrawRect(rect, color);
    }
    private void DrawCollisionSet(Vector2 position, Vector2 offset, int mapIndex)
    {
        MetatileMapData metatileMapData = _mapdata.MetatileDatas[mapIndex];
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawCollision(pos, offset, metatileMapData.data[i]);
        }
    }
    private void DrawMapEditor()
    {
        //GUILayout.BeginHorizontal();
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
        _selectedTile = GUILayout.SelectionGrid(_selectedTile, GetButtonTextures().ToArray(), _mapdata.Size.x, style);
        if (GUILayout.Button("Clear Map")) _mapdata.ClearMap();
        if (GUILayout.Button("Save Asset")) SaveAsset();

        //_selectedTile = GUILayout.SelectionGrid(_selectedTile, new Texture[_mapdata.MaxTileCount], _mapdata.Size.x, style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        DrawMetatileSelector();
        GUILayout.EndHorizontal();
        float magnify = 4;
        Rect metatilePreviewRect = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none, GUILayout.MinWidth(magnify * 8 * 2), GUILayout.MinHeight(magnify * 8 * 2));
        metatilePreviewRect.x += 20;
        Rect tileIdentifierRect = new(metatilePreviewRect)
        {
            width = magnify * 8,
            height = magnify * 8,
        };
        tileIdentifierRect.position += new Vector2((int)_tileIdentifier % 2, (int)_tileIdentifier / 2) * 8 * magnify;
        EditorGUI.DrawRect(tileIdentifierRect, Color.red);
        DrawMetatile(Vector2.zero, metatilePreviewRect.position, _metatileIndex, magnify, (magnify * 8 - 2) / (magnify * 8));
        if (_selectedTile != -1)
        {
            UpdateMapData();
        }
        _palette = (Palette)EditorGUILayout.EnumPopup(new GUIContent("Palette", "Metatile palette"), _palette);
        _tileIdentifier = (TileIdentifier)EditorGUILayout.EnumPopup(new GUIContent("Tile", "Which tile to edit"), _tileIdentifier);
        TileCollisionData tileCollisionData = new()
        {
            isCollidable = EditorGUILayout.Toggle("Is Collidable", _tileCollisionDatas[(int)_tileIdentifier].isCollidable),
            isInFront = EditorGUILayout.Toggle("Is in front", _tileCollisionDatas[(int)_tileIdentifier].isInFront),
            isAirCollidable = EditorGUILayout.Toggle("Is air collidable", _tileCollisionDatas[(int)_tileIdentifier].isAirCollidable)
        };
        _tileCollisionDatas[(int)_tileIdentifier] = tileCollisionData;
        GUILayout.EndVertical();
    }

    private void DrawMetatileSelector()
    {
        bool isPreviousPressed = GUILayout.Button("Previous");
        bool isNextPressed = GUILayout.Button("Next");
        int inc = isPreviousPressed ? -1 : isNextPressed ? 1 : 0;
        _metatileIndex = (_metatileIndex + _mapdata.EditorMemory.Max + inc) % _mapdata.EditorMemory.Max;
    }

    private void UpdateMapData()
    {
        int selectedX = _selectedTile % _mapdata.Size.x;
        int selectedY = _selectedTile / _mapdata.Size.x;
        int mapY = _mapdata.Size.y - 1 - selectedY;
        int mapIndex = selectedX + mapY * _mapdata.Size.x;
        _mapdata.ChangeMetatileIndex(mapIndex, _metatileIndex);
        _mapdata.SetTileCollisionDatas(mapIndex, _tileCollisionDatas);
        _mapdata.ChangeMetatilePalette(mapIndex, _palette);
        _hasValidated = true;
        _selectedTile = -1;
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
        if (_mapdata.EditorMemory == null)
        {
            EditorGUILayout.LabelField("missing Metatile Memory", errorStyle);  
            return;
        }
        if (_mapdata.EditorTileset == null)
        {
            EditorGUILayout.LabelField("missing Tileset", errorStyle);
            return;
        }
        _previewMode = (PreviewMode)EditorGUILayout.EnumPopup(new GUIContent("Preview Mode", "Change display mode of map preview"), _previewMode);
        GUILayout.BeginHorizontal(GUILayout.Height(_mapdata.Size.y * 16));
        switch (_previewMode)
        {
            case PreviewMode.Editor: DrawMapEditor(); break;
            case PreviewMode.Sprite: DrawMetatiles(); break;
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
        _mapdata = target as MapData;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
        _editorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_editorFoldout, "Preview");
        DrawPreview();
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
