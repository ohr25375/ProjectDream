using UnityEngine;

[System.Serializable]
[AddTypeMenu("Move to Random Direction")]
public class EnemyStateMoveToRandomDirection : EnemyState
{
    [SerializeReference, SubclassSelector] private EnemyValueFloatBase duration = new EnemyValueFloatUseImmediate();
    [SerializeField] private float distance;
    private Vector2 direction;
    private readonly Vector2[] randomDirections = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter();
        var random = Random.Range(0, 4);
        direction = randomDirections[random];

    }
    protected override void OnStateExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnStateUpdate()
    {
        var duration = this.duration.GetValue(enemyController);
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
