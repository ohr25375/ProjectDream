using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "Scriptable Objects/Tileset")]
public class Tileset : ScriptableObject
{
    [SerializeField] private List<Sprite> _tileset;

    public int Max => _tileset.Count;
    public Sprite GetSprite(int index)
    {
        if (index < _tileset.Count)
        {
            return _tileset[index];
        }
        else
        {
            return null;
        }
    }

    public Sprite GetRandomSprite() => GetSprite(Random.Range(0, _tileset.Count));
}
