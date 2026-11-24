using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSetter : MonoBehaviour
{
    [SerializeField] private InstancedMaterial _material;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _defaultSprite;

    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        _renderer.material = _material.Material;
        if (_defaultSprite != null)
        {
            _renderer.sprite = _defaultSprite;
        }
        Destroy(this);
    }
}
