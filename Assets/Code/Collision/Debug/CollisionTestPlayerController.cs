using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollisionTestPlayerController : MonoBehaviour
{
    [System.Serializable]
    [System.Flags]
    private enum InputStates
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
    }

    [SerializeField] private InputActionReference _inputLeft;
    [SerializeField] private InputActionReference _inputRight;
    [SerializeField] private InputActionReference _inputUp;
    [SerializeField] private InputActionReference _inputDown;
    [SerializeField] private float _speed;
    [SerializeField] private List<Vector2> _colliderOffsets;
    private List<GridBasedCollider> _colliders;
    private Vector3 _currentPosition;
    public Vector3 CurrentPosition => _currentPosition;
    private Vector2 _mapPositionOffset;
    private InputStates _currentInputStates;
    private bool _isLastHorizontalInputLeft;
    private Direction _currentMovingDirection;
    [SerializeField] private TileCollisionFlags _tileCollisionFlags;
    private bool _canMove = true;
    
    #region UNITY FUNCTIONS
    private void OnEnable()
    {
        _inputLeft.action.started += OnInputLeft;
        _inputRight.action.started += OnInputRight;
        _inputUp.action.started += OnInputUp;
        _inputDown.action.started += OnInputDown;

        _inputLeft.action.canceled += OnInputLeft;
        _inputRight.action.canceled += OnInputRight;
        _inputUp.action.canceled += OnInputUp;
        _inputDown.action.canceled += OnInputDown;
    }
    private void OnDisable()
    {
        _inputLeft.action.started -= OnInputLeft;
        _inputRight.action.started -= OnInputRight;
        _inputUp.action.started -= OnInputUp;
        _inputDown.action.started -= OnInputDown;

        _inputLeft.action.canceled -= OnInputLeft;
        _inputRight.action.canceled -= OnInputRight;
        _inputUp.action.canceled -= OnInputUp;
        _inputDown.action.canceled -= OnInputDown;
    }
    private void Update()
    {
        DoMove();
    }
    #endregion

    #region INPUT
    private void OnInputLeft(InputAction.CallbackContext context)
    {
        OnInputs(context, InputStates.Left);
        if (context.started) _isLastHorizontalInputLeft = true;
    }
    private void OnInputRight(InputAction.CallbackContext context)
    {
        OnInputs(context, InputStates.Right);
        if (context.started) _isLastHorizontalInputLeft = false;
    }
    private void OnInputUp(InputAction.CallbackContext context) => OnInputs(context, InputStates.Up);
    private void OnInputDown(InputAction.CallbackContext context) => OnInputs(context, InputStates.Down);
    private void OnInputs(InputAction.CallbackContext context, InputStates inputID)
    {
        if (context.action.IsPressed())
        {
            _currentInputStates |= inputID;
        }
        else
        {
            _currentInputStates &= ~inputID;
        }
    }
    private Direction SolveInput(InputStates currentInputStates, bool isSkipVertical = false)
    {
        if (currentInputStates == InputStates.None) return Direction.None;
        if (currentInputStates.HasFlag(InputStates.Up) && !isSkipVertical) return Direction.Up;
        if (currentInputStates.HasFlag(InputStates.Down) && !isSkipVertical) return Direction.Down;
        bool isBothPressed = currentInputStates.HasFlag(InputStates.Left) == currentInputStates.HasFlag(InputStates.Right);
        if (!isBothPressed && currentInputStates.HasFlag(InputStates.Left) == !_isLastHorizontalInputLeft)
        {
            _isLastHorizontalInputLeft = !_isLastHorizontalInputLeft;
        }
        return _isLastHorizontalInputLeft ? Direction.Left : Direction.Right;
    }
    #endregion

    public void SetCanMove(bool state) => _canMove = state;
    public void Initialize(Vector2 position)
    {
        OnInitialize(position);
    }
    private void OnInitialize(Vector2 position)
    {
        _currentPosition = position;
        _colliders = new(new GridBasedCollider[_colliderOffsets.Count]);
        for (var i = 0; i < _colliders.Count; i++)
        {
            var offsetPosition = _colliderOffsets[i];
            _colliders[i] = new();
            _colliders[i].SetOffset(offsetPosition);
        }
    }
    public void SetMapPositionOffset(Vector2 offset)
    {
        _mapPositionOffset = offset;
    }
    public void SetCurrentPosition(Vector2 position)
    {
        _currentPosition = position;
        transform.localPosition = RoundPosition(position);
    }

    public void AddCurrentPosition(Vector2 position)
    {
        _currentPosition += (Vector3)position;
        transform.localPosition = RoundPosition(_currentPosition);
    }
    private Vector3 RoundPosition(Vector3 position)
    {
        return new()
        {
            x = Mathf.RoundToInt(position.x * 8) / 8.0f,
            y = Mathf.RoundToInt(position.y * 8) / 8.0f,
            z = 0,
        };
    }

    private void DoMove()
    {
        if (!_canMove) { return; }
        _currentMovingDirection = SolveInput(_currentInputStates);
        var newPosition = _currentPosition + _speed * Time.deltaTime * (Vector3)_currentMovingDirection.ToVector2();
        Vector3 roundedPosition = RoundPosition(newPosition);
        var currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
        bool hasVerticalInput = _currentInputStates.HasFlag(InputStates.Up) || _currentInputStates.HasFlag(InputStates.Down);
        bool hasHorizontalInput = _currentInputStates.HasFlag(InputStates.Left) || _currentInputStates.HasFlag(InputStates.Right);
        var colliders = ScreenController.MainMapData.GetColliders(_tileCollisionFlags);
        var mapSize = ScreenController.MainMapData.Size * 2;
        if (hasVerticalInput && hasHorizontalInput)
        {
            foreach (var collider in _colliders)
            {
                var ic = collider.IsColliding(colliders, mapSize, _currentMovingDirection, currentMapPosition);
                if (ic < 0)
                {
                    ScreenController.MoveMap(_currentMovingDirection);
                    return;
                }
                if (ic == 0) continue;
                _currentMovingDirection = SolveInput(_currentInputStates, true);
                newPosition = _currentPosition + _speed * Time.deltaTime * (Vector3)_currentMovingDirection.ToVector2();
                roundedPosition = RoundPosition(newPosition);
                currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
                break;
            }
        }
        foreach (var collider in _colliders)
        {
            var ic = collider.IsColliding(colliders, mapSize, _currentMovingDirection, currentMapPosition);
            if (ic < 0)
            {
                ScreenController.MoveMap(_currentMovingDirection);
                return;
            }
            if (ic != 0) return;
        }
        _currentPosition = newPosition;
        transform.localPosition = roundedPosition;
    }
}
