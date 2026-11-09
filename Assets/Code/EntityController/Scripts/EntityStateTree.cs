using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStateTree", menuName = "Scriptable Objects/EntityStateTree")]
public class EntityStateTree : ScriptableObject
{
    [SerializeReference, SubclassSelector] private List<EntityState> entityStates;
    public List<EntityState> GetClone()
    {
        var clone = new List<EntityState>(new EntityState[entityStates.Count]);
        for (var i = 0; i < entityStates.Count; i++)
        {
            clone[i] = entityStates[i].Clone();
        }
        return clone;
    }
}