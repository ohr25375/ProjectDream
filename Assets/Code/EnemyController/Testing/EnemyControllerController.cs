using UnityEngine;

public class EnemyControllerController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    private void Awake()
    {
        enemyController.Initialize(transform);
    }

    private void Update()
    {
        enemyController.Update();
    }
}
