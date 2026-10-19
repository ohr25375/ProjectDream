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
    private bool _hasValidated = false;
    private enum PreviewMode
    {
        Sprite,
        Collisions
    };
    private PreviewMode _previewMode = PreviewMode.Sprite;
    private List<Texture> GetButtonTextures()
    {
        if (_hasValidated)
        {
            _buttonTextures = new(new Texture[_mapdata.MaxTileCount]);
            for (var i = 0; i < _mapdata.MaxTileCount; i++)
            {
                int metatileIndex = _mapdata.MetatileDatas[i].metatileIndex;
                MetatileSpriteData metatileSpriteData = _mapdata.EditorMemory.GetData(metatileIndex);
                Sprite sprite = _mapdata.EditorTileset.GetSprite(2);
                _buttonTextures[i] = AssetPreview.GetAssetPreview(sprite);
            }
        }
        return _buttonTextures;
    }
    private void OnEnable()
    {
        _mapdata = target as MapData;
        _selectedTile = -1;
        _hasValidated = false;
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
    private void DrawCollision(Vector2 position, Vector2 offset, TileCollisionData collisionData)
    {
        Rect rect = new(position * 16 + offset, Vector2.one * 8);
        Color color = new(collisionData.isCollidable ? 1 : 0, 0, collisionData.isAirCollidable ? 1 : 0);
        EditorGUI.DrawRect(rect, color);
    }
    private void DrawCollisions(Vector2 position, Vector2 offset, int mapIndex)
    {
        MetatileMapData metatileMapData = _mapdata.MetatileDatas[mapIndex];
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawCollision(pos, offset, metatileMapData.data[i]);
        }
    }
    private void DrawButtons()
    {
        //GUILayout.
        GUIStyle style = new(GUI.skin.button);
        style.fixedWidth = 16;
        style.fixedHeight = 16;
        style.margin = new(0, 0, 0, 0);
        //style.border = new(0, 0, 0, 0);
        //style.padding = new(0, 0, 0, 0);
        //style.contentOffset = new(0, 0);
        //style.overflow = new(0, 0, 0, 0);
        _selectedTile = GUILayout.SelectionGrid(_selectedTile, GetButtonTextures().ToArray(), _mapdata.Size.x, style);
        if (_selectedTile != -1)
        {
            Debug.Log($"Selected {_selectedTile}");
            _selectedTile = -1;
        }
    }
    private void DrawPreview()
    {
        if (!_editorFoldout) return;
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
        DrawButtons();
        Rect rt = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
        for (var y = 0; y < _mapdata.Size.y; y++)
        {
            for (var x = 0; x < _mapdata.Size.x; x++)
            {
                var mapIndex = x + y * _mapdata.Size.x;
                var metatileIndex = _mapdata.MetatileDatas[mapIndex].metatileIndex;
                var position = new Vector2(x, _mapdata.Size.y - 1 - y);
                switch (_previewMode)
                {
                    case PreviewMode.Sprite: DrawMetatile(position, rt.position, metatileIndex); break;
                    case PreviewMode.Collisions: DrawCollisions(position, rt.position, mapIndex); break;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
    public void OnValidate()
    {
        _hasValidated = true;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _editorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_editorFoldout, "Preview");
        DrawPreview();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
