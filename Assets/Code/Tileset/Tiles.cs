using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tiles", menuName = "Scriptable Objects/Tiles")]
public class Tiles : ScriptableObject
{
    [SerializeField] private List<TileController> _tileControllers;
}
