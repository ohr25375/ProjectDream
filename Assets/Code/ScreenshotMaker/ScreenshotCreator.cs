using UnityEditor;
using UnityEngine;

public class ScreenshotCreator
{
    [MenuItem("Assets/Save RenderTexture as png")]
    private static void TakeScreenshot()
    {
        RenderTexture rt = Selection.activeObject as RenderTexture;
        RenderTexture.active = rt;
        Texture2D texture = new(rt.width, rt.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes;
        bytes = texture.EncodeToPNG();
        string path = AssetDatabase.GetAssetPath(rt) +  ".png";
        for (var i = 0; AssetDatabase.AssetPathExists(path); i++)
        {
            path = AssetDatabase.GetAssetPath(rt) + i.ToString() + ".png";
        }
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("Saved to " + path);
    }

    [MenuItem("Assets/Save RenderTexture as png", true)]
    static bool TakeScreenshotValidation()
    {
        return Selection.activeObject is RenderTexture;
    }
}
