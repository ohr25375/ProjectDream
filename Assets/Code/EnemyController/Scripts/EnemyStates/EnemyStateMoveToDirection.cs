using UnityEngine;

[System.Serializable]
[AddTypeMenu("Move to Direction")]
public class EnemyStateMoveToDirection : EnemyState
{
    [SerializeField] private float duration;
    [SerializeField] private ValidDirection moveDirection;
    [SerializeField] private float distance;
    private Vector2 direction;
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter();
        direction = moveDirection.GetValidDirection();
    }
    protected override void OnStateExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnStateUpdate()
    {
        if (duration <= 0)
        {
            ChangeToNextState();
            return;
        }
        if (timer > duration)
        {
            if (previousTimer < duration)
            {
                var remaining = duration - previousTimer;
                enemyController.Move(direction, (distance / duration) * remaining);
            }
            ChangeToNextState();
        }
        else
        {
            enemyController.Move(direction, (distance / duration) * DeltaTime);
        }
    }
}
