using UnityEngine;

public class ExtensionFunctions
{
    public static Vector3 RoundPosition(Vector3 position)
    {
        return new()
        {
            x = Mathf.RoundToInt(position.x * 8) / 8.0f,
            y = Mathf.RoundToInt(position.y * 8) / 8.0f,
            z = 0,
        };
    }
    public static Vector3 GetNewPosition(Vector3 currentPosition, float speed, Direction direction) => GetNewPosition(currentPosition, speed, direction.ToVector2());
    public static Vector3 GetNewPosition(Vector3 currentPosition, float speed, Vector2 direction)
    {
        return currentPosition + speed * (Vector3)direction;
    }
}
