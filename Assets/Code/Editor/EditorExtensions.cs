using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
    const int SPRITE_SIZE = 8;
    public static void DrawTile(Tileset tileset, Vector2 position, Vector2 offset, TileSpriteData spriteData, float magnify = 1, float size = 1)
    {
        Sprite sprite = tileset.GetSprite(spriteData.spriteID);
        bool flipX = spriteData.isFlippedX;
        bool flipY = spriteData.isFlippedY;
        if (sprite == null)
        {
            sprite = tileset.GetSprite(0);
        }
        var spriteRect = sprite.GetNormalizedSpriteRect();
        Rect rect = new()
        {
            x = SPRITE_SIZE * magnify * (2 * position.x + ((flipX ? 1 : 0) + (1 - size) / 2 * (flipX ? -1 : 1))) + offset.x,
            y = SPRITE_SIZE * magnify * (2 * position.y + ((flipY ? 1 : 0) + (1 - size) / 2 * (flipY ? -1 : 1))) + offset.y,
            width = (flipX ? -1 : 1) * SPRITE_SIZE * magnify * size,
            height = (flipY ? -1 : 1) * SPRITE_SIZE * magnify * size
        };
        GUI.DrawTextureWithTexCoords(rect, sprite.texture, spriteRect);
    }
    public static void DrawTileRaw(Tileset tileset, Vector2 position, Vector2 offset, int index, float magnify = 1, float size = 1)
    {
        Sprite sprite = tileset.GetSprite(index);
        if (sprite == null)
        {
            sprite = tileset.GetSprite(0);
        }
        var spriteRect = sprite.GetNormalizedSpriteRect();
        Rect rect = new()
        {
            x = SPRITE_SIZE * magnify * (position.x + offset.x + (1 - size) / 2),
            y = SPRITE_SIZE * magnify * (position.y + offset.y + (1 - size) / 2),
            width =  SPRITE_SIZE * magnify * size,
            height = SPRITE_SIZE * magnify * size
        };
        GUI.DrawTextureWithTexCoords(rect, sprite.texture, spriteRect);
    }
    public static void DrawMetatile(Tileset tileset, MetatileSpriteData metatileSpriteData, Vector2 position, Vector2 offset, float magnify = 1, float size = 1)
    {
        var tileSpriteDatas = metatileSpriteData.data;
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2;
            DrawTile(tileset, pos, offset, tileSpriteDatas[i], magnify, size);
        }
    }
    public static void DrawMetatileAsOne(Tileset tileset, MetatileSpriteData metatileSpriteData, Vector2 position, Vector2 offset, float magnify = 1, float size = 1)
    {
        var tileSpriteDatas = metatileSpriteData.data;
        for (var i = 0; i < 4; i++)
        {
            var pos = position + new Vector2(i % 2, i / 2) / 2 + new Vector2((((i & 1) == 0) ? 1 : -1), (i >> 1 == 0) ? 1 : -1) * (1 / 32.0f);
            DrawTile(tileset, pos, offset, tileSpriteDatas[i], magnify, size);

        }
    }
    public static GUIStyle ButtonStyle(float spriteSize, float magnify = 1)
    {
        return new(GUI.skin.button)
        {
            fixedHeight = spriteSize * magnify,
            fixedWidth = spriteSize * magnify,
            margin = new(0, 0, 0, 0),
            border = new(0, 0, 0, 0),
            padding = new(0, 0, 0, 0),
            contentOffset = new(0, 0),
            overflow = new(0, 0, 0, 0),
        };
    }
    public static Rect GetNormalizedSpriteRect(this Sprite sprite)
    {
       return new Rect()
        {
            x = sprite.rect.x / sprite.texture.width,
            y = sprite.rect.y / sprite.texture.height,
            width = sprite.rect.width / sprite.texture.width,
            height = sprite.rect.height / sprite.texture.height,
        };
    }
    public static int DrawSelectionGrid(Vector2Int size, float spriteSize, float buttonSize = 1, int margin = 1) => DrawSelectionGrid(size.x, size.y, spriteSize, buttonSize, margin);
    public static int DrawSelectionGrid(int width, int height, float spriteSize, float buttonSize = 1, int margin = 0)
    {
        var style = ButtonStyle(spriteSize, buttonSize);
        style.margin = new(margin, margin, margin, margin);
        var selected = -1;
        EditorGUILayout.BeginVertical();
        for (var y = 0; y < height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var x = 0; x < width; x++)
            {
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
    public static int DrawTileIdentiferSelector(int currentIndex, Tileset tileset, MetatileSpriteData metatileSpriteData, float spriteSize = SPRITE_SIZE)
    {
        float magnify = 4;
        var rect = EditorGUILayout.BeginVertical(GUILayout.Width(magnify * spriteSize * 2));
        Rect tileIdentifierRect = new(rect)
        {
            width = magnify * spriteSize,
            height = magnify * spriteSize,
        };
        tileIdentifierRect.position += spriteSize * magnify * new Vector2(currentIndex % 2, currentIndex >> 1);
        var selectedMetatileSubIndex = DrawSelectionGrid(2, 2, spriteSize, magnify);
        EditorGUI.DrawRect(tileIdentifierRect, Color.red);
        DrawMetatile(tileset, metatileSpriteData, Vector2.zero, rect.position, magnify, (magnify * spriteSize - 2) / (magnify * spriteSize));
        EditorGUILayout.EndVertical();
        return selectedMetatileSubIndex;
    }
    public static int DrawMetatileSelector(MetatileMemory metatileMemory, int selectedMetatileIndex, ref int currentMetatileSubIndex)
    {
        var metatileSpriteData = metatileMemory.GetData(selectedMetatileIndex);
        GUIStyle verticalButtonStyle = new(GUI.skin.button)
        {
            stretchHeight = false,
            fixedHeight = 4 * SPRITE_SIZE * 2 - 2,
            stretchWidth = false
        };
        EditorGUILayout.BeginHorizontal();
        bool isPreviousPressed = GUILayout.Button("Å©", verticalButtonStyle);

        var selectedMetatileSubIndex = DrawTileIdentiferSelector(currentMetatileSubIndex, metatileMemory.EditorTileset, metatileSpriteData);
        if (selectedMetatileSubIndex != -1)
        {
            currentMetatileSubIndex = selectedMetatileSubIndex;
        }

        bool isNextPressed = GUILayout.Button("Å®", verticalButtonStyle);
        int inc = isPreviousPressed ? -1 : isNextPressed ? 1 : 0;
        selectedMetatileIndex = (selectedMetatileIndex + metatileMemory.Max + inc) % metatileMemory.Max;
        while (metatileMemory.GetData(selectedMetatileIndex, true).name == "")
        {
            if (inc == 0) inc = 1;
            selectedMetatileIndex = (selectedMetatileIndex + metatileMemory.Max + inc) % metatileMemory.Max;
        }
        EditorGUILayout.EndHorizontal();
        return selectedMetatileIndex;
    }
    public static int DrawMetatileMemories(MetatileMemory metatileMemory, int selectedMetatileIndex)
    {
        const int ROW_SIZE = 16;
        var rect = EditorGUILayout.BeginHorizontal();
        var count = metatileMemory.Max;
        var drawIndex = 0;
        var validCount = metatileMemory.Max - metatileMemory.GetDisabledCount();
        var selected = GUILayout.SelectionGrid(-1, new string[validCount], ROW_SIZE, ButtonStyle(SPRITE_SIZE * 2));
        for (int i = 0; i < count; i++)
        {
            var data = metatileMemory.GetData(i, true);
            if (data.IsNull()) continue;
            int x = drawIndex % ROW_SIZE;
            int y = drawIndex / ROW_SIZE;
            if (selectedMetatileIndex == i)
            {
                var size = 2;
                Rect spriteIdentifierRect = new(rect)
                {
                    width = SPRITE_SIZE * size + 1,
                    height = SPRITE_SIZE * size + 1,
                };
                spriteIdentifierRect.position += (SPRITE_SIZE * size) * new Vector2(drawIndex % ROW_SIZE, drawIndex / ROW_SIZE) - Vector2.right;
                EditorGUI.DrawRect(spriteIdentifierRect, Color.red);
            }
            DrawMetatileAsOne(metatileMemory.EditorTileset, data, new Vector2(x, y), rect.position);
            drawIndex++;
        }
        EditorGUILayout.EndHorizontal();
        return selected;
    }
}
