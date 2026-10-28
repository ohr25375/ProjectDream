using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CollisionTestColliderChecker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Vector2 currentPosition;
    [SerializeField] private Vector2Int checkingPosition1;
    [SerializeField] private Vector2Int checkingPosition2;
    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    public void SetOffset(Vector2 offset) => _offset = offset;
    private bool IsOutOfBounds(Vector2 position, Vector2 size) => position.x < 0 || position.x >= size.x || position.y < 0 || position.y >= size.y;
    public bool IsColliding(List<bool> colliders, Vector2Int size, Direction direction, Vector2 rootPosition)
    {
        currentPosition = rootPosition + _offset;
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
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x),Mathf.CeilToInt(currentPosition.y));
                    checkingPosition2 = new Vector2Int(Mathf.CeilToInt(currentPosition.x), Mathf.CeilToInt(currentPosition.y));
                    break;
                }
            case Direction.Down:
                {
                    checkingPosition1 = new Vector2Int(Mathf.FloorToInt(currentPosition.x),Mathf.FloorToInt(currentPosition.y));
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
        if (IsOutOfBounds(checkingPosition1, size) || IsOutOfBounds(checkingPosition2, size))
        {
            _spriteRenderer.color = Color.purple;
            return true;
        }
        var index1 = checkingPosition1.y * size.x + checkingPosition1.x;
        if (colliders[index1])
        {
            _spriteRenderer.color = Color.red;
            return true;
        }
        var index2 = checkingPosition2.y * size.x + checkingPosition2.x;
        if (colliders[index2])
        {
            _spriteRenderer.color = Color.cyan;
            return true;
        }
        _spriteRenderer.color = Color.green;
        return false;
    }
}
