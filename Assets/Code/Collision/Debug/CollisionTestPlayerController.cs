using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private struct CollisionResults
    {
        public bool isColliding;
        public bool hasFlatCollision;
        public int totalBias;
    }

    [SerializeField] private InputActionReference _inputLeft;
    [SerializeField] private InputActionReference _inputRight;
    [SerializeField] private InputActionReference _inputUp;
    [SerializeField] private InputActionReference _inputDown;
    [SerializeField] private float _speed;
    [SerializeField] private List<Vector2> _colliderOffsets;
    [SerializeField] private GameObject _sprite;
    private List<SpriteController> _spriteControllers;
    private Palette _currentPalette = Palette.A;
    [Header("Experimental")]
    [SerializeField] private StaminaControl _staminaController;
    public StaminaControl StaminaController => _staminaController;
    private List<GridBasedCollider> _colliders;
    private Vector3 _currentPosition;
    private Vector2 _mapPositionOffset;
    private InputStates _currentInputStates;
    private bool _isLastHorizontalInputLeft;
    private Direction _currentMovingDirection;
    [SerializeField] private TileCollisionFlags _tileCollisionFlags;
    private bool _canMove = true;
    private bool _isKnockbacking = false;
    private Direction _knockbackDirection;
    private Coroutine _knockbackTimer;
    private Coroutine _flashTimer;
    
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
        DoKnockback();
    }
    #endregion

    #region ACCESSORS
    public Vector3 CurrentPosition => _currentPosition;
    public void SetCanMove(bool state) => _canMove = state;
    public void SetMapPositionOffset(Vector2 offset)
    {
        _mapPositionOffset = offset;
    }
    public void SetCurrentPosition(Vector2 position)
    {
        _currentPosition = position;
        transform.localPosition = ExtensionFunctions.RoundPosition(position);
    }
    public void AddCurrentPosition(Vector2 position)
    {
        _currentPosition += (Vector3)position;
        transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);
    }
    public List<Vector2> GetColliderPositions()
    {
        var colliderPositions = new List<Vector2>(new Vector2[_colliderOffsets.Count]);
        for (var i = 0; i < _colliderOffsets.Count; i++)
        {
            colliderPositions[i] = (Vector2)transform.localPosition + _colliderOffsets[i];
        }
        return colliderPositions;
    }
    public void RegisterKnockback(Vector2 damageOrigin)
    {
        if (_staminaController.IsHitCooldown) { return; }
        var damageDirection = (damageOrigin - (Vector2)transform.localPosition).normalized;
        var xAbs = Mathf.Abs(damageDirection.x);
        var yAbs = Mathf.Abs(damageDirection.y);
        if (xAbs > yAbs)
        {
            _knockbackDirection = damageDirection.x < 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            _knockbackDirection = damageDirection.y < 0 ? Direction.Up : Direction.Down;
        }
        if (_knockbackTimer != null)
        {
            StopCoroutine(_knockbackTimer);
        }
        _knockbackTimer = StartCoroutine(KnockbackTimer());
    }
    public void Damage(int amount)
    {
        if (!_staminaController.Damage(amount)) return;
        if (_flashTimer != null)
        {
            StopCoroutine(_flashTimer);
        }
        _flashTimer = StartCoroutine(FlashTimer());
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

    #region INTERNAL
    private bool ComputeCollision(Direction movingDirection, float deltaSpeed, Vector2Int mapSize, ReadOnlyCollection<bool> colliders, out CollisionResults collisionResults, out Vector3 newPosition, bool isMoveMap = true)
    {
        collisionResults = new()
        {
            isColliding = false,
            hasFlatCollision = false,
            totalBias = 0
        };
        newPosition = ExtensionFunctions.GetNewPosition(_currentPosition, deltaSpeed, movingDirection);
        Vector3 roundedPosition = ExtensionFunctions.RoundPosition(newPosition);
        var currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
        foreach (var collider in _colliders)
        {
            var ic = collider.IsColliding(colliders, mapSize, movingDirection, currentMapPosition, out int colliderBias);
            if (ic < 0 && isMoveMap)
            {
                ScreenController.MoveMap(movingDirection);
                return false;
            }
            collisionResults.totalBias += colliderBias;
            if (ic != 0) collisionResults.isColliding = true;
            if (ic != 0 && colliderBias == 0) collisionResults.hasFlatCollision = true;
        }
        return true;
    }
    private bool DoCollision(CollisionResults collisionResults, Vector3 newPosition)
    {
        if (!collisionResults.isColliding)
        {
            _currentPosition = newPosition;
            transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);
            return false;
        }
        if (!collisionResults.hasFlatCollision)
        {
            _currentMovingDirection = _currentMovingDirection.GetRelativeDirection(collisionResults.totalBias);
        }

        return true;
    }
    #endregion

    public void Initialize(Vector2 position)
    {
        OnInitialize(position);
    }
    public bool ForceMove(Direction direction, float speed)
    {
        var colliders = ScreenController.MainMapData.GetColliders(_tileCollisionFlags);
        var mapSize = ScreenController.MainMapData.Size * 2;
        ComputeCollision(direction, speed * Time.deltaTime, mapSize, colliders, out CollisionResults collisionResults, out Vector3 newPosition, false);
        if (collisionResults.isColliding)
        {
            return false;
        }

        _currentPosition = newPosition;
        transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);
        return true;
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
        transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);

        _spriteControllers = new(new SpriteController[_colliderOffsets.Count]);
        for (var i = 0; i < _colliderOffsets.Count; i++)
        {
            var obj = Instantiate(_sprite, transform);
            _spriteControllers[i] = new();
            _spriteControllers[i].Initialize(obj.GetComponent<SpriteRenderer>());
            _spriteControllers[i].SetGlobalPosition(transform.position + (Vector3)_colliderOffsets[i] + new Vector3(0.5f, 0.5f));
        }
    }
    private void DoMove()
    {
        if (!_canMove) { return; }
        _currentMovingDirection = SolveInput(_currentInputStates);
        var colliders = ScreenController.MainMapData.GetColliders(_tileCollisionFlags);
        var mapSize = ScreenController.MainMapData.Size * 2;
        bool hasVerticalInput = _currentInputStates.HasFlag(InputStates.Up) || _currentInputStates.HasFlag(InputStates.Down);
        if (hasVerticalInput)
        {
            if (!ComputeCollision(_currentMovingDirection, _speed * Time.deltaTime, mapSize, colliders, out CollisionResults collisionResults, out Vector3 newPosition)) { return; }
            if (!DoCollision(collisionResults, newPosition)) { return; }
        }
        bool hasHorizontalInput = _currentInputStates.HasFlag(InputStates.Left) || _currentInputStates.HasFlag(InputStates.Right);
        if (hasHorizontalInput)
        {
            _currentMovingDirection = SolveInput(_currentInputStates, true);
            if (!ComputeCollision(_currentMovingDirection, _speed * Time.deltaTime, mapSize, colliders, out CollisionResults collisionResults, out Vector3 newPosition)) { return; }
            if (!DoCollision(collisionResults, newPosition)) { return; }
        }
        {
            if (!ComputeCollision(_currentMovingDirection, _speed * Time.deltaTime, mapSize, colliders, out CollisionResults collisionResults, out Vector3 newPosition)) { return; }
            if (collisionResults.hasFlatCollision) { return; }
            if (!collisionResults.isColliding)
            {
                _currentPosition = newPosition;
                transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);
                return;
            }
        }
    }
    private void DoKnockback()
    {
        if (!_isKnockbacking) { return; }
        if (ForceMove(_knockbackDirection, 60)) { return; }
        _isKnockbacking = false;
        if (_knockbackTimer != null)
        {
            StopCoroutine( _knockbackTimer);
            _knockbackTimer = null;
        }
    }
    private IEnumerator KnockbackTimer()
    {
        _isKnockbacking = true;
        yield return new WaitForSeconds(0.1f);
        _isKnockbacking = false;
        _knockbackTimer = null;
    }
    public void SetPalette(Palette palette)
    {
        _currentPalette = palette;
    }
    public void SetTemporaryPalette(Palette palette)
    {
        foreach (var spriteController in _spriteControllers)
        {
            spriteController.SetPalette(palette);
            spriteController.UpdateRenderer();
        }
    }
    private IEnumerator FlashTimer()
    {
        var modifiedPalette = Palette.A;
        while (_staminaController.IsHitCooldown)
        {
            modifiedPalette += 1;
            SetTemporaryPalette(modifiedPalette);
            yield return new WaitForSeconds(0.05f);
        }
        SetTemporaryPalette(_currentPalette);
    }
}
