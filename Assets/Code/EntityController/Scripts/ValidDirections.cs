using UnityEngine;

public static class ValidDirections
{
    public static Vector2 GetValidDirection(this ValidDirection direction)
    {
        return direction switch
        {
            ValidDirection.Left => Vector2.left,
            ValidDirection.Right => Vector2.right,
            ValidDirection.Up => Vector2.up,
            ValidDirection.Down => Vector2.down,
            _ => Vector2.zero,
        };
    }
}

[System.Serializable]
public enum ValidDirection
{
    None,
    Left,
    Right,
    Up,
    Down,
}
