using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstancedMaterial))]
public class EditorInstancedMaterial : Editor
{
    private InstancedMaterial _instancedMaterial;
    private MaterialEditor _materialEditor;

    void OnEnable()
    {
        _instancedMaterial = target as InstancedMaterial;

        if (_instancedMaterial.Material != null)
        {
            _materialEditor = (MaterialEditor)CreateEditor(_instancedMaterial.Material);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            if (_materialEditor != null)
            {
                DestroyImmediate(_materialEditor);
            }
            if (_instancedMaterial.Material != null)
            {
                _materialEditor = (MaterialEditor)CreateEditor(_instancedMaterial.Material);
            }
        }
        if (_materialEditor != null)
        {
            _materialEditor.DrawHeader();
            _materialEditor.OnInspectorGUI();
        }
    }
    void OnDisable()
    {
        if (_materialEditor != null)
        {
            DestroyImmediate(_materialEditor);
        }
    }
}
