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
    private Vector2 _mapPositionOffset;
    private InputStates _currentInputStates;
    private bool _isLastHorizontalInputLeft;
    private Direction _currentMovingDirection;
    
    #region UNITY FUNCTIONS
    private void Awake()
    {
        OnInitialize();
    }
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

    private void OnInitialize()
    {
        _currentPosition = transform.position;
        _colliders = new(new GridBasedCollider[_colliderOffsets.Count]);
        for (var i = 0; i < _colliders.Count; i++)
        {
            var offsetPosition = _colliderOffsets[i];
            _colliders[i] = new();
            _colliders[i].SetOffset(offsetPosition);
        }
    }
    public void SetMapPositionOffset(Vector2 offset) => _mapPositionOffset = offset;

    private void Update()
    {
        _currentMovingDirection = SolveInput(_currentInputStates);
        var newPosition = _currentPosition + _speed * Time.deltaTime * (Vector3)_currentMovingDirection.ToVector2();
        Vector3 roundedPosition = new()
        {
            x = Mathf.RoundToInt(newPosition.x * 8) / 8.0f,
            y = Mathf.RoundToInt(newPosition.y * 8) / 8.0f,
            z = 0,
        };
        var currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
        bool hasVerticalInput = _currentInputStates.HasFlag(InputStates.Up) || _currentInputStates.HasFlag(InputStates.Down);
        bool hasHorizontalInput = _currentInputStates.HasFlag(InputStates.Left) || _currentInputStates.HasFlag(InputStates.Right);
        if (hasVerticalInput && hasHorizontalInput)
        {
            foreach (var collider in _colliders)
            {
                if (!collider.IsColliding(CreateCollisionTestMap.CollisionTiles, CreateCollisionTestMap.MapSize, _currentMovingDirection, currentMapPosition)) continue;
                _currentMovingDirection = SolveInput(_currentInputStates, true);
                newPosition = _currentPosition + _speed * Time.deltaTime * (Vector3)_currentMovingDirection.ToVector2();
                roundedPosition = new()
                {
                    x = Mathf.RoundToInt(newPosition.x * 8) / 8.0f,
                    y = Mathf.RoundToInt(newPosition.y * 8) / 8.0f,
                    z = 0,
                };
                currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
                break;
            }
        }
        foreach (var collider in _colliders)
        {
            if (collider.IsColliding(CreateCollisionTestMap.CollisionTiles, CreateCollisionTestMap.MapSize, _currentMovingDirection, currentMapPosition)) return;
        }
        _currentPosition = newPosition;
        transform.position = roundedPosition;
    }
}
