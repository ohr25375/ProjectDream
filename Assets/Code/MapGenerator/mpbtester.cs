using UnityEngine;

public class mpbtester : MonoBehaviour
{
    [SerializeField] private string propertyName;
    [ContextMenu("Test")]
    private void GetMPB()
    {
        var renderer = GetComponent<SpriteRenderer>();
        MaterialPropertyBlock mpb = new();
        mpb.SetFloat("_Palette", 1);
        renderer.SetPropertyBlock(mpb);
    }
}
