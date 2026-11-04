using System.Collections.Generic;
using UnityEngine;

public class GridBasedCollider
{
    private Vector2 _offset;

    #region INTERNAL
    private bool IsOutOfBounds(Vector2 position, Vector2 size) => position.x < 0 || position.x >= size.x || position.y < 0 || position.y >= size.y;
    #endregion

    public void SetOffset(Vector2 offset) => _offset = offset;
    public bool IsColliding(List<bool> colliders, Vector2Int size, Direction direction, Vector2 rootPosition)
    {
        var currentPosition = rootPosition + _offset;
        Vector2Int checkingPosition1, checkingPosition2;
        switch (direction)
        {
            case Direction.Left:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
                    checkingPosition2 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));
                    break;
                }
            case Direction.Right:
                {
                    checkingPosition1 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));
                    break;
                }
            case Direction.Up:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));
                    break;
                }
            case Direction.Down:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
                    break;
                }
            default:
                {
                    checkingPosition1 = new Vector2Int((int)currentPosition.x, (int)currentPosition.y);
                    checkingPosition2 = new Vector2Int((int)currentPosition.x, (int)currentPosition.y);
                    break;
                }
        }
        if (IsOutOfBounds(checkingPosition1, size) || IsOutOfBounds(checkingPosition2, size)) { return true; }
        var index1 = checkingPosition1.y * size.x + checkingPosition1.x;
        if (colliders[index1]) { return true; }
        var index2 = checkingPosition2.y * size.x + checkingPosition2.x;
        if (colliders[index2]) { return true; }
        return false;
    }
}
