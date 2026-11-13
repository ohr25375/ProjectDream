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
    public static void DrawTileRaw(Tileset tileset, Vector2 position, Vector2 offset, int index, float magnify = 1, float size = 1)
    {
        Sprite sprite = tileset.GetSprite(index);
        if (sprite == null)
        {
            sprite = tileset.GetSprite(0);
        }
        Texture2D spritePreview = AssetPreview.GetAssetPreview(sprite);
        if (spritePreview == null) return;
        Rect rect = new()
        {
            x = 8 * magnify * position.x + offset.x + (1 - size) / 2 * 8 * magnify,
            y = 8 * magnify * position.y + offset.y + (1 - size) / 2 * 8 * magnify,
            width =  8 * magnify * size,
            height = 8 * magnify * size
        };
        GUI.DrawTexture(rect, spritePreview);
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
}
