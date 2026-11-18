using UnityEngine;

[System.Serializable]
[AddTypeMenu("Move to Random Direction")]
public class EntityStateMoveToRandomDirection : EntityState
{
    [SerializeReference, SubclassSelector] private EntityValueFloatBase duration = new EntityValueFloatUseImmediate();
    [SerializeField] private float distance;
    [SerializeField] private bool collision = true;
    private Direction direction;
    private readonly Direction[] randomDirections = {Direction.Up, Direction.Down, Direction.Left, Direction.Right};
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
        var duration = this.duration.GetValue(entityController);
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
                entityController.Move(direction, collision, (distance / duration) * remaining);
            }
            ChangeToNextState();
        }
        else
        {
            entityController.Move(direction, collision, (distance / duration) * DeltaTime);
        }
    }
}
