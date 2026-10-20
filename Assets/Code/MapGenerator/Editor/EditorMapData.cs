using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MapData))]
public class EditorMapData : Editor
{
    private MapData _mapdata;
    private bool _editorFoldout = true;
    private List<Texture> _buttonTextures = new();
    private int _selectedTile = -1;
    private bool _hasValidated = true;
    private Palette _palette = Palette.A;
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
                    MetatileSpriteData metatileSpriteData = _mapdata.EditorMemory.GetData(metatileIndex);
                    Sprite sprite = _mapdata.EditorTileset.GetSprite(metatileSpriteData.data[0].spriteID);
                    _buttonTextures[textureIndex] = AssetPreview.GetAssetPreview(sprite);
                }
            }
        }
        return _buttonTextures;
    }
    private void OnEnable()
    {
        _mapdata = target as MapData;
        _selectedTile = -1;
        _hasValidated = true;
        _palette = Palette.A;
    }
    private void DrawTile(Vector2 position, Vector2 offset, TileSpriteData spriteData)
    {
        Sprite sprite = _mapdata.EditorTileset.GetSprite(spriteData.spriteID);
        if (sprite == null)
        {
            sprite = _mapdata.EditorTileset.GetSprite(0);
        }
        Texture2D spritePreview = AssetPreview.GetAssetPreview(sprite);
        if (spritePreview == null) return;
        GUI.DrawTexture(new Rect(position.x * 16 + offset.x, position.y * 16 + offset.y, 8, 8), spritePreview);
    }
    private void DrawMetatile(Vector2 position, Vector2 offset, int metatileIndex)
    {
        MetatileSpriteData metatileSpriteDatas = _mapdata.EditorMemory.GetData(metatileIndex);
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawTile(pos, offset, metatileSpriteDatas.data[i]);
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
        //_selectedTile = GUILayout.SelectionGrid(_selectedTile, new Texture[_mapdata.MaxTileCount], _mapdata.Size.x, style);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        bool isPreviousPressed = GUILayout.Button("Previous");
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        bool isNextPressed = GUILayout.Button("Next");
        int inc = isPreviousPressed ? -1 : isNextPressed ? 1 : 0;
        _mapdata.e_metatileIndex = (_mapdata.e_metatileIndex + _mapdata.EditorMemory.Max + inc) % _mapdata.EditorMemory.Max;
        for (int i = 0; i < 4; i++)
        {
            DrawMetatile(Vector2.zero, rt.position, _mapdata.e_metatileIndex);
        }
        if (_selectedTile != -1)
        {
            int selectedX = _selectedTile % _mapdata.Size.x;
            int selectedY = _selectedTile / _mapdata.Size.x;
            int mapY = _mapdata.Size.y - 1 - selectedY;
            int mapIndex = selectedX + mapY * _mapdata.Size.x;
            _mapdata.ChangeMetatileIndex(mapIndex, _mapdata.e_metatileIndex);
            _mapdata.ChangeMetatilePalette(mapIndex, _palette);
            _hasValidated = true;
            _selectedTile = -1;
        }
        GUILayout.EndHorizontal();
        _palette = (Palette)EditorGUILayout.EnumPopup(new GUIContent("Palette", "Metatile palette"), _palette);
        GUILayout.EndVertical();
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
        _editorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_editorFoldout, "Preview");
        DrawPreview();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
