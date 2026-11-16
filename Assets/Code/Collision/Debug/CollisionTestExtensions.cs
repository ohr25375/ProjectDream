using UnityEngine;

public enum Direction
{
    None, Left, Right, Up, Down,
}
public static class CollisionTestExtensions
{
    public static Vector2 ToVector2(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            _ => Vector2.zero,
        };
    }
    public static Vector2Int ToVector2Int(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => Vector2Int.left,
            Direction.Right => Vector2Int.right,
            Direction.Up => Vector2Int.up,
            Direction.Down => Vector2Int.down,
            _ => Vector2Int.zero,
        };
    }
    public static Direction GetRelativeDirection(this Direction direction, int bias)
    {
        if (bias == 0) return direction;
        bool isBiasPositive = bias > 0;
        return direction switch
        {
            Direction.Left => isBiasPositive ? Direction.Down : Direction.Up,
            Direction.Right => isBiasPositive ? Direction.Down : Direction.Up,
            Direction.Up => isBiasPositive ? Direction.Left : Direction.Right,
            Direction.Down => isBiasPositive ? Direction.Left : Direction.Right,
            _ => Direction.None,
        };
    }
}
