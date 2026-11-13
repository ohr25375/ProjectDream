using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
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
            x = 8 * magnify * (2 * position.x + ((flipX ? 1 : 0) + (1 - size) / 2 * (flipX ? -1 : 1))) + offset.x,
            y = 8 * magnify * (2 * position.y + ((flipY ? 1 : 0) + (1 - size) / 2 * (flipY ? -1 : 1))) + offset.y,
            width = (flipX ? -1 : 1) * 8 * magnify * size,
            height = (flipY ? -1 : 1) * 8 * magnify * size
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
            x = 8 * magnify * (position.x + offset.x + (1 - size) / 2),
            y = 8 * magnify * (position.y + offset.y + (1 - size) / 2),
            width =  8 * magnify * size,
            height = 8 * magnify * size
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
    public static int DrawTileIdentiferSelector(int currentIndex, Tileset tileset, MetatileSpriteData metatileSpriteData, float spriteSize = 8)
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
}
