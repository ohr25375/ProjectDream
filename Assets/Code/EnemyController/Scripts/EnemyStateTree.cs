using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateTree", menuName = "Scriptable Objects/EnemyStateTree")]
public class EnemyStateTree : ScriptableObject
{
    [SerializeReference, SubclassSelector] private List<EnemyState> enemyStates;
    public List<EnemyState> GetClone()
    {
        var clone = new List<EnemyState>(new EnemyState[enemyStates.Count]);
        for (var i = 0; i < enemyStates.Count; i++)
        {
            clone[i] = enemyStates[i].Clone();
        }
        return clone;
    }
}