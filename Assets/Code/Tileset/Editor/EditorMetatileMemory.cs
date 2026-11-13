using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetatileMemory))]
public class EditorMetatileMemory : Editor
{
    private MetatileMemory _metatileMemory;
    private const float SPRITE_SIZE = 8;
    private int _selectedMetatileIndex = 0;
    private int _currentMetatileSubIndex = 0;
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
                for (var i = 0; i < 256; i++)
                {
                    int x = i % 16;
                    int y = i / 16;
                    var sprite = _metatileMemory.EditorTileset.GetSprite(i);
                    var spriteRect = sprite.GetNormalizedSpriteRect();
                    Rect textureRect = new()
                    {
                        x = (8 * magnify + 1) * x + rect.x,
                        y = (8 * magnify + 1) * y + rect.y,
                        width  = 8 * magnify,
                        height = 8 * magnify,
                    };
                    GUI.DrawTextureWithTexCoords(textureRect, sprite.texture, spriteRect);
                }
                if (selected != -1)
                {
                    _metatileMemory.EditorModifyTileSpriteID(_selectedMetatileIndex, (TileIdentifier)_currentMetatileSubIndex, selected);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
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
                    if (_selectedMetatileIndex > 0)
                    {
                        _selectedMetatileIndex--;
                    }
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
        var selectedMetatileSubIndex = EditorExtensions.DrawTileIdentiferSelector(_currentMetatileSubIndex, metatileMemory.EditorTileset, metatileSpriteData);
        if (selectedMetatileSubIndex != -1)
        {
            _currentMetatileSubIndex = selectedMetatileSubIndex;
        }
    }
    #endregion
}