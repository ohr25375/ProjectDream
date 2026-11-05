using UnityEngine;

public class PaletteChecker : MonoBehaviour
{
    [SerializeField] private IndexableMetatileClass<Sprite> _sprites;

    [SerializeField] private InstancedMaterial _foreground;
    [SerializeField] private InstancedMaterial _background;

    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        OnInitialize();
    }
    private void OnInitialize()
    {
        CreatePalettePreview(false);
        CreatePalettePreview(true);
    }

    private void CreatePalettePreview(bool isBackground)
    {
        var material = isBackground ? _background.Material : _foreground.Material;
        var offset = (Vector3)(new Vector2(0.5f, -0.5f) + (isBackground ? Vector2.zero : Vector2.right * 5));
        for (var p = 0; p < 4; p++)
        {
            var origin = 2 * p * Vector3.down;
            for (var i = 0; i < 4; i++)
            {
                var obj = new GameObject($"Palette{p}-{i}");
                obj.transform.parent = transform;
                obj.transform.localPosition = origin + Vector3.right * i + offset;
                obj.transform.localScale = Vector3.one;
                var spriteRenderer = obj.AddComponent<SpriteRenderer>();
                spriteRenderer.material = material;
                spriteRenderer.sprite = _sprites[i];
                spriteRenderer.sortingOrder = 9999;
                MaterialPropertyBlock mpb = new();
                mpb.SetFloat("_Palette", p);
                mpb.SetFloat("_IsBackground", isBackground ? 1 : 0);
                spriteRenderer.SetPropertyBlock(mpb);
            }
        }
    }
}
