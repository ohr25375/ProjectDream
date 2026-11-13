using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[CustomEditor(typeof(MetatileMemory))]
public class EditorMetatileMemory : Editor
{
    private MetatileMemory _metatileMemory;
    private const float SPRITE_SIZE = 8;
    private int _selectedMetatileIndex = 0;
    private int _currentMetatileSubIndex = 0;
    private bool _hasValidated = true;
    private List<Texture> _editorTilesetTextureCache = new();
    #region UNITY
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawInspector();
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    private void DrawInspector()
    {
        _metatileMemory = target as MetatileMemory;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_tileset"));
        if (!IsRequirementsSet(_metatileMemory))
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        {
            float magnify = 2;
            DrawMetatileSelector(_metatileMemory);
            var rect = EditorGUILayout.BeginVertical();
            {
                DrawSelectedSpriteIndicator(_metatileMemory, rect, magnify);
                var buttonStyle = EditorExtensions.ButtonStyle(SPRITE_SIZE, magnify);
                buttonStyle.margin = new(1, 1, 1, 1);
                var selected = GUILayout.SelectionGrid(-1, new string[256], 16, buttonStyle);
                var textures = _metatileMemory.EditorTextureCache;
                for (var i = 0; i < 256; i++)
                {
                    int x = i % 16;
                    int y = i / 16;
                    Rect textureRect = new()
                    {
                        x = 8 * magnify * x + rect.x,// + (1 - size) / 2 * 8 * magnify,
                        y = 8 * magnify * y + rect.y,// + (1 - size) / 2 * 8 * magnify,
                        width  = 8 * magnify,// * size,
                        height = 8 * magnify,// * size
                    };
                    GUI.DrawTexture(textureRect, textures[i]);
                }
                if (selected != -1)
                {
                    _metatileMemory.EditorModifyTileSpriteID(_selectedMetatileIndex, (TileIdentifier)_currentMetatileSubIndex, selected);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
        _hasValidated = false;
    }

    private void DrawSelectedSpriteIndicator(MetatileMemory metatileMemory, Rect rect, float size = 1)
    {
        var spriteIndex = metatileMemory.GetData(_selectedMetatileIndex).data[_currentMetatileSubIndex].spriteID;
        Rect spriteIdentifierRect = new(rect)
        {
            width = SPRITE_SIZE * size + 2,
            height = SPRITE_SIZE * size + 2,
        };
        spriteIdentifierRect.position += (SPRITE_SIZE * size + 1) * new Vector2(spriteIndex % 16, spriteIndex / 16) - Vector2.one;
        EditorGUI.DrawRect(spriteIdentifierRect, Color.red);
    }

    #region SUB_FUNCTIONS
    private bool IsRequirementsSet(MetatileMemory metatileMemory)
    {
        GUIStyle errorStyle = new(GUI.skin.label) { normal = new() { textColor = Color.red } };
        if (metatileMemory.EditorTileset == null)
        {
            EditorGUILayout.LabelField("ERROR: missing tileset.", errorStyle);
            return false;
        }
        return true;
    }
    private void DrawMetatileSelector(MetatileMemory metatileMemory)
    {
        var metatileSpriteData = metatileMemory.GetData(_selectedMetatileIndex);
        GUIStyle verticalButtonStyle = new(GUI.skin.button)
        {
            stretchHeight = true,
            stretchWidth = false
        };
        float magnify = 4;
        EditorGUILayout.BeginVertical(GUILayout.Height(magnify * SPRITE_SIZE * 2));
        {
            EditorGUILayout.BeginHorizontal();
            bool isPreviousPressed = GUILayout.Button("©", verticalButtonStyle);
            DrawMetatile(metatileMemory, metatileSpriteData);
            bool isNextPressed = GUILayout.Button("¨", verticalButtonStyle);
            int inc = isPreviousPressed ? -1 : isNextPressed ? 1 : 0;
            _selectedMetatileIndex = (_selectedMetatileIndex + metatileMemory.Max + inc) % metatileMemory.Max;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+"))
                {
                    metatileMemory.EditorInsertData(_selectedMetatileIndex);
                    _selectedMetatileIndex++;
                }
                if (GUILayout.Button("-"))
                {
                    metatileMemory.EditorDeleteData(_selectedMetatileIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField($"Name: {metatileSpriteData.name}");
            var newName = EditorGUILayout.TextField(metatileSpriteData.name);
            if (newName != metatileSpriteData.name)
            {
                metatileMemory.EditorModifyMetatileName(_selectedMetatileIndex, newName);
            }
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawMetatile(MetatileMemory metatileMemory, MetatileSpriteData metatileSpriteData)
    {
        float magnify = 4;
        var rect = EditorGUILayout.BeginVertical(GUILayout.Width(magnify * SPRITE_SIZE * 2));
        {
            Rect tileIdentifierRect = new(rect)
            {
                width = magnify * SPRITE_SIZE,
                height = magnify * SPRITE_SIZE,
            };
            tileIdentifierRect.position += SPRITE_SIZE * magnify * new Vector2(_currentMetatileSubIndex % 2, _currentMetatileSubIndex >> 1);
            var selectedMetatileSubIndex = DrawSelectionGrid(2, 2, SPRITE_SIZE, magnify);
            EditorGUI.DrawRect(tileIdentifierRect, Color.red);
            if (selectedMetatileSubIndex != -1)
            {
                _currentMetatileSubIndex = selectedMetatileSubIndex;
            }
            EditorExtensions.DrawMetatile(metatileMemory.EditorTileset, metatileSpriteData, Vector2.zero, rect.position, magnify, (magnify * SPRITE_SIZE - 2) / (magnify * SPRITE_SIZE));
        }
        EditorGUILayout.EndVertical();
    }
    private int DrawSelectionGrid(Vector2Int size, float spriteSize, float buttonSize = 1, int margin = 1) => DrawSelectionGrid(size.x, size.y, spriteSize, buttonSize, margin);
    private int DrawSelectionGrid(int width, int height, float spriteSize, float buttonSize = 1, int margin = 0)
    {
        var style = EditorExtensions.ButtonStyle(spriteSize, buttonSize);
        style.margin = new(margin, margin, margin, margin);
        var selected = -1;
        EditorGUILayout.BeginVertical();
        for (var y = 0; y < height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var x = 0; x < width; x++)
            {
                var index = x + y * width;
                if (GUILayout.Button("", style))
                {
                    selected = x + y * width;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        return selected;
    }
    #endregion
}