using UnityEngine;

[CreateAssetMenu(fileName = "InstancedMaterial", menuName = "Scriptable Objects/InstancedMaterial")]
public class InstancedMaterial : ScriptableObject
{
    [SerializeField] private Material material;
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
}
