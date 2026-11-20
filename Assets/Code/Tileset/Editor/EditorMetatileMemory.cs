using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetatileMemory))]
public class EditorMetatileMemory : Editor
{
    private MetatileMemory _metatileMemory;
    private const float SPRITE_SIZE = 8;
    private int _selectedMetatileIndex = 0;
    private int _currentMetatileSubIndex = 0;
    public static float EditorMagnification;

    #region UNITY
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawInspector();
        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssetIfDirty(target);
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
        bool isEditorZoomed = EditorGUILayout.Toggle("x2", EditorMagnification > 1.0f);
        EditorMagnification = isEditorZoomed ? 2.0f : 1.0f;
        EditorGUILayout.BeginHorizontal();
        {
            float magnify = 2;
            DrawMetatileSelectorMenu(_metatileMemory);
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
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
    }

    #region SUB_FUNCTIONS
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
    private void DrawMetatileSelectorMenu(MetatileMemory metatileMemory)
    {
        var metatileSpriteData = metatileMemory.GetData(_selectedMetatileIndex);
        float magnify = 4;
        EditorGUILayout.BeginVertical(GUILayout.Height(magnify * SPRITE_SIZE * 2));
        {
            EditorGUILayout.BeginHorizontal();
            _selectedMetatileIndex = EditorExtensions.DrawMetatileSelector(metatileMemory, _selectedMetatileIndex, ref _currentMetatileSubIndex, out Rect rect);
            var spriteData = metatileSpriteData.data[_currentMetatileSubIndex];
            EditorGUILayout.BeginVertical();
            var stateX = GUILayout.Toggle(spriteData.isFlippedX, $"{(TileIdentifier)_currentMetatileSubIndex}: FlipX");
            var stateY = GUILayout.Toggle(spriteData.isFlippedY, $"{(TileIdentifier)_currentMetatileSubIndex}: FlipY");
            if (stateX != spriteData.isFlippedX || stateY != spriteData.isFlippedY)
            {
                spriteData.isFlippedX = stateX;
                spriteData.isFlippedY = stateY;
                metatileMemory.EditorModifyTileSpriteData(_selectedMetatileIndex, (TileIdentifier)_currentMetatileSubIndex, spriteData);
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+"))
                {
                    _selectedMetatileIndex = metatileMemory.EditorInsertData(_selectedMetatileIndex);
                    EditorUtility.SetDirty(target);
                }
                if (_selectedMetatileIndex == 0) GUI.enabled = false;
                if (GUILayout.Button("-"))
                {
                    metatileMemory.EditorDeleteData(_selectedMetatileIndex);
                    if (_selectedMetatileIndex > 0)
                    {
                        _selectedMetatileIndex--;
                    }
                    EditorUtility.SetDirty(target);
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
            var disabledCount = metatileMemory.GetDisabledCount();
            EditorGUILayout.LabelField($"[{_selectedMetatileIndex + 1}/{metatileMemory.Max - disabledCount}{(disabledCount > 0 ? $"({disabledCount})" : "")}]Name: {metatileSpriteData.name}");
            var newName = EditorGUILayout.DelayedTextField(metatileSpriteData.name);
            if (newName != metatileSpriteData.name)
            {
                metatileMemory.EditorModifyMetatileName(_selectedMetatileIndex, newName);
                EditorUtility.SetDirty(target);
            }
            var selectedMetatilePalette = EditorExtensions.DrawMetatileMemories(metatileMemory, _selectedMetatileIndex, EditorMagnification);
            if (selectedMetatilePalette != -1)
            {
                _selectedMetatileIndex = selectedMetatilePalette;
            }
        }
        EditorGUILayout.EndVertical();
    }
    #endregion
}