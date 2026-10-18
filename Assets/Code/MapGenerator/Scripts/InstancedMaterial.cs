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
        [SerializeField] private Color color4;
        public readonly Color Color1 => color1;
        public readonly Color Color2 => color2;
        public readonly Color Color3 => color3;
        public readonly Color Color4 => color4;
        public Palette(Color color1, Color color2, Color color3, Color color4)
        {
            this.color1 = color1;
            this.color2 = color2;
            this.color3 = color3;
            this.color4 = color4;
        }
    }
    [SerializeField] private Material material;
    [SerializeField] private Palette paletteA = new(Color.black, Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteB = new(Color.black, Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteC = new(Color.black, Color.gray3, Color.gray6, Color.white);
    [SerializeField] private Palette paletteD = new(Color.black, Color.gray3, Color.gray6, Color.white);
    private Material instantiatedMaterial;
    public Material Material => GetMaterial();

    private Material GetMaterial()
    {
        if (instantiatedMaterial == null)
        {
            instantiatedMaterial = Instantiate(material);
        }
        return instantiatedMaterial;
    }

    private void ChangeMaterialColors(Material material, string prefix, Palette palette)
    {
        material.SetColor(prefix + "_Color1", palette.Color1);
        material.SetColor(prefix + "_Color2", palette.Color2);
        material.SetColor(prefix + "_Color3", palette.Color3);
        material.SetColor(prefix + "_Color4", palette.Color4);
    }

    [ContextMenu("Update Material")]
    private void SetDefaultPalettes()
    {
        ChangeMaterialColors(GetMaterial(), "_PaletteA", paletteA);
        ChangeMaterialColors(GetMaterial(), "_PaletteB", paletteB);
        ChangeMaterialColors(GetMaterial(), "_PaletteC", paletteC);
        ChangeMaterialColors(GetMaterial(), "_PaletteD", paletteD);
    }

    private void OnEnable()
    {
        SetDefaultPalettes();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetDefaultPalettes();
    }
#endif
}