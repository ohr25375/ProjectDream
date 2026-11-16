using System.Collections.ObjectModel;
using UnityEngine;

public class GridBasedCollider
{
    private Vector2 _offset;

    #region INTERNAL
    private bool IsOutOfBounds(Vector2 position, Vector2 size) => position.x < 0 || position.x >= size.x || position.y < 0 || position.y >= size.y;
    #endregion

    public void SetOffset(Vector2 offset) => _offset = offset;
    public int IsColliding(ReadOnlyCollection<bool> colliders, Vector2Int size, Direction direction, Vector2 rootPosition) => IsColliding(colliders, size, direction, rootPosition, out _);
    public int IsColliding(ReadOnlyCollection<bool> colliders, Vector2Int size, Direction direction, Vector2 rootPosition, out int colliderBias)
    {
        var currentPosition = rootPosition + _offset;
        Vector2Int checkingPosition1, checkingPosition2;
        switch (direction)
        {
            case Direction.Left:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));   // negative side (down)
                    checkingPosition2 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));    // positive side (up)
                    break;
                }
            case Direction.Right:
                {
                    checkingPosition1 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));    // negative side (down)
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));     // positive side (up)
                    break;
                }
            case Direction.Up:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));    // negative side (left)
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));     // positive side (right)
                    break;
                }
            case Direction.Down:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));   // negative side (left)
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));    // positive side (right)
                    break;
                }
            default:
                {
                    checkingPosition1 = new Vector2Int((int)currentPosition.x, (int)currentPosition.y);
                    checkingPosition2 = new Vector2Int((int)currentPosition.x, (int)currentPosition.y);
                    break;
                }
        }
        colliderBias = 0;
        if (IsOutOfBounds(checkingPosition1, size) || IsOutOfBounds(checkingPosition2, size)) { return -1; }
        var index1 = checkingPosition1.y * size.x + checkingPosition1.x;
        var index2 = checkingPosition2.y * size.x + checkingPosition2.x;
        bool isNegativeColliderColliding = colliders[index1];
        bool isPositiveColliderColliding = colliders[index2];
        colliderBias = (isNegativeColliderColliding ? -1 : 0) + (isPositiveColliderColliding ? 1 : 0);
        if (isNegativeColliderColliding || isPositiveColliderColliding) return 1;
        return 0;
    }
}
