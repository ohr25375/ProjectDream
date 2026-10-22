using UnityEngine;

[System.Serializable]
public class EnemyStateMoveToRandomDirection : EnemyState
{
    [SerializeField] private int duration;
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
        if (timer <= duration)
        {
            enemyController.Move(direction);
        }
        else
        {
            ChangeToNextState();
        }
    }
}
