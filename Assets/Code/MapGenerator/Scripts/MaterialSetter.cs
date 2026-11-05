using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSetter : MonoBehaviour
{
    [SerializeField] private InstancedMaterial _material;
    [SerializeField] private Renderer _renderer;

    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
        _renderer.material = _material.Material;
        Destroy(this);
    }
}
