using System.Collections.Generic;
using UnityEngine;

public class EntityControllerController : MonoBehaviour
{
    private readonly EntityController enemyController = new();

    public void Initialize(List<EntityState> entityStates, Vector2Int position)
    {
        OnInitialize(entityStates, position);
    }
    public void UpdateEntity()
    {
        enemyController.Update();
    }
    private void OnInitialize(List<EntityState> entityStates, Vector2Int position)
    {
        enemyController.SetStateTree(entityStates);
        enemyController.Initialize(transform, position);
    }
}
