using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateTree", menuName = "Scriptable Objects/EnemyStateTree")]
public class EnemyStateTree : ScriptableObject
{
    [SerializeReference, SubclassSelector] private List<EnemyState> enemyStates;
}