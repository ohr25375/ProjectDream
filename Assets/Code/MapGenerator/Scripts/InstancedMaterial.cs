using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "InstancedMaterial", menuName = "Scriptable Objects/InstancedMaterial")]
public class InstancedMaterial : ScriptableObject
{
    [System.Serializable]
    public struct Palette
    {
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;
        [SerializeField] private Color color3;
        public readonly Color Color1 => color1;
        public readonly Color Color2 => color2;
        public readonly Color Color3 => color3;
        public Palette(Color color1, Color color2, Color color3)
        {
            this.color1 = color1;
            this.color2 = color2;
            this.color3 = color3;
        }
    }
    [SerializeField] private Material material;
    [Header("Default Palette")]
    [SerializeField] private bool isBackground;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Palette paletteA = new(Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteB = new(Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteC = new(Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteD = new(Color.gray3, Color.gray6, Color.white);
    private Material instantiatedMaterial;
    public Material Material => GetMaterial();
    private void InitializeMaterial()
    {
        if (instantiatedMaterial == null)
        {
            instantiatedMaterial = Instantiate(material);
            SetPalettes();
        }
    }

    private Material GetMaterial()
    {
        InitializeMaterial();
        return instantiatedMaterial;
    }

    private void ChangeMaterialColors(Material material, string prefix, Palette palette)
    {
        material.SetColor(prefix + "_Color1", palette.Color1);
        material.SetColor(prefix + "_Color2", palette.Color2);
        material.SetColor(prefix + "_Color3", palette.Color3);
    }

    [ContextMenu("Update Material")]
    private void SetPalettes()
    {
        GetMaterial();
        GetMaterial().SetFloat("_IsBackground", isBackground ? 1f : 0f);
        GetMaterial().SetColor("_Background", backgroundColor);
        ChangeMaterialColors(GetMaterial(), "_PaletteA", paletteA);
        ChangeMaterialColors(GetMaterial(), "_PaletteB", paletteB);
        ChangeMaterialColors(GetMaterial(), "_PaletteC", paletteC);
        ChangeMaterialColors(GetMaterial(), "_PaletteD", paletteD);
    }

    private void OnEnable()
    {
        InitializeMaterial();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        InitializeMaterial();
        if (!EditorApplication.isPlaying)
        {
            SetPalettes();
        }
    }
#endif
}