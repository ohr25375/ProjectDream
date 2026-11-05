using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[AddTypeMenu("Parabola")]
public class EnemyStateParabola : EnemyState
{
    [SerializeField] private float duration;
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private float height;
    private Vector2 previousPosition;
    protected override void OnStateEnter(params int[] variables)
    {
        base.OnStateEnter(variables);
        previousPosition = Vector2.zero;
    }
    protected override void OnStateExit()
    {
        //
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
                var moveVector = targetOffset - previousPosition;
                enemyController.Move(moveVector);
            }
            ChangeToNextState();
        }
        else
        {
            var lerpValue = timer / duration;
            var position = Vector2.Lerp(Vector2.zero, targetOffset, lerpValue) + Mathf.Sin(Mathf.PI * lerpValue) * height * Vector2.up;
            var moveVector = position - previousPosition;
            enemyController.Move(moveVector);
            previousPosition += moveVector;
        }
    }
}
