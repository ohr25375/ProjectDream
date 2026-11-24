using System.Collections.Generic;
using UnityEngine;
using static EntityData;

[System.Serializable]
public class EntityController
{
    #region BASICS
    private Transform _transform;
    private Vector2 _currentPosition;
    private Vector2 _mapPositionOffset;
    private List<SpriteController> _spriteControllers;
    #endregion

    #region PUBLIC PARAMETERS
    public Vector2Int MapSize { get; private set; } = Vector2Int.zero;
    public int[] Registers = new int[System.Enum.GetNames(typeof(Register)).Length];
    public float[] FloatingRegisters = new float[System.Enum.GetNames(typeof(Register)).Length];
    public int CurrentStateIndex { get; private set; } = 0;
    public bool IsPaused = false;
    public TileCollisionFlags TileCollisionFlags;
    #endregion

    [System.Serializable]
    public enum Register
    {
        A,
        B,
    }
    private List<EntityState> _entityStates;
    private List<GridBasedCollider> _gridBasedColliders;
    private List<Vector2> _colliderOffsets;
    private EntityState CurrentEnemyState => _entityStates[CurrentStateIndex];
    private List<EntityAnimationFrame> _animationFrames;

    public List<Vector2> GetColliderPositions()
    {
        var colliderPositions = new List<Vector2>(new Vector2[_colliderOffsets.Count]);
        for (var i = 0; i < _colliderOffsets.Count; i++)
        {
            colliderPositions[i] = (Vector2)_transform.localPosition + _colliderOffsets[i];
        }
        return colliderPositions;
    }
    public void Initialize(Transform transform, Vector2Int position) => OnInitialize(transform, position);
    public void SetMapSize(Vector2Int mapSize) => MapSize = mapSize;
    public void InjectSpriteControllers(List<SpriteController> spriteControllers)
    {
        _spriteControllers = spriteControllers;
        foreach (var spriteController in _spriteControllers)
        {
            spriteController.Enable();
        }
    }
    public void SetMapPositionOffset(Vector2 mapPositionOffset) => _mapPositionOffset = mapPositionOffset;
    public void SetStateTree(List<EntityState> states) => _entityStates = states;
    public void SetAnimationFrames(List<EntityAnimationFrame> animationFrames) => _animationFrames = animationFrames;
    public void SetCollisionFlags(TileCollisionFlags collisionFlags) => TileCollisionFlags = collisionFlags;
    public void InitializeGridBasedColliders(EntityData entityData) => InitializeGridBasedColliders(entityData.ColliderOffsets);
    public void InitializeGridBasedColliders(List<Vector2> colliderOffsets)
    {
        _colliderOffsets = colliderOffsets;
        _gridBasedColliders = new(new GridBasedCollider[_colliderOffsets.Count]);
        for (var i = 0; i < _colliderOffsets.Count; i++)
        {
            _gridBasedColliders[i] = new();
            _gridBasedColliders[i].SetOffset(_colliderOffsets[i]);
        }
    }
    public void Update() => OnUpdate();
    public void SetEntityState(int index)
    {
        if (index < 0)
        {
            index = _entityStates.Count - ((-index) % _entityStates.Count);
        }
        CurrentStateIndex = index % _entityStates.Count;
        CurrentEnemyState.EnterState(this);
    }
    public void SetEntityStateRelative(int index)
    {
        if (index < 0)
        {
            index = _entityStates.Count - ((-index) % _entityStates.Count);
        }
        int stateIndex = (CurrentStateIndex + index) % _entityStates.Count;
        SetEntityState(stateIndex);
    }
    public void MoveSpriteRenderers()
    {
        var position = _transform.position + new Vector3(0.5f, 0.5f, 0);
        for (var i = 0; i < _colliderOffsets.Count; i++)
        {
            _spriteControllers[i].SetGlobalPosition(position + (Vector3)_colliderOffsets[i]);
        }
    }
    public void Move(Vector2 direction, float speed = 1)
    {
        var newPosition = ExtensionFunctions.GetNewPosition(_currentPosition, speed, direction);
        var roundedPosition = ExtensionFunctions.RoundPosition(newPosition);
        _currentPosition = newPosition;
        _transform.localPosition = roundedPosition;
    }
    public void Move(Direction direction, bool isColliderUsed, float speed = 1)
    {
        if (isColliderUsed)
        {
            MoveWithCollision(direction, speed);
        }
        else
        {
            Move(direction.ToVector2(), speed);
        }
        MoveSpriteRenderers();
    }
    public bool IsCollidingTile(TileCollisionFlags collisionFlags)
    {
        var colliders = ScreenController.GetColliders(TileCollisionFlags);
        var mapSize = ScreenController.MainMapData.Size * 2;
        var newPosition = ExtensionFunctions.GetNewPosition(_currentPosition, 0, Direction.None);
        Vector3 roundedPosition = ExtensionFunctions.RoundPosition(newPosition);
        var currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
        bool isColliding = false;
        foreach (var collider in _gridBasedColliders)
        {
            var ic = collider.IsColliding(colliders, mapSize, Direction.None, currentMapPosition);
            if (ic != 0)
            {
                isColliding = true;
                break;
            }
        }
        return isColliding;
    }
    public void MoveWithCollision(Direction direction, float speed = 1)
    {
        var colliders = ScreenController.GetColliders(TileCollisionFlags);
        var mapSize = ScreenController.MainMapData.Size * 2;
        var newPosition = ExtensionFunctions.GetNewPosition(_currentPosition, speed, direction);
        Vector3 roundedPosition = ExtensionFunctions.RoundPosition(newPosition);
        var currentMapPosition = roundedPosition - (Vector3)_mapPositionOffset;
        bool isColliding = false;
        foreach (var collider in _gridBasedColliders)
        {
            var ic = collider.IsColliding(colliders, mapSize, direction, currentMapPosition);
            if (ic != 0)
            {
                isColliding = true;
                break;
            }
        }
        if (isColliding) return;
        _currentPosition = newPosition;
        _transform.localPosition = roundedPosition;
    }
    public void ReturnToPool()
    {
        foreach (var spriteController in _spriteControllers)
        {
            spriteController.ReturnToPool();
        }
        _transform.gameObject.SetActive(false);
    }

    #region INTERNAL
    private void OnInitialize(Transform transform, Vector2Int position)
    {
        if (_entityStates == null)
        {
            Debug.LogError("Entity state not initialized. Check parent code", transform.gameObject);
            return;
        }
        CurrentStateIndex = 0;
        _transform = transform;
        _currentPosition = position;
        _transform.localPosition = ExtensionFunctions.RoundPosition(_currentPosition);
        for (var i = 0; i < Registers.Length; i++)
        {
            Registers[i] = 0;
        }
        CurrentEnemyState.EnterState(this);
    }
    private void OnUpdate()
    {
        if (IsPaused) return;
        CurrentEnemyState.UpdateState(Time.deltaTime);
        int frameIndex = CurrentEnemyState.animationFrame;
        var frame = _animationFrames[frameIndex].parts;
        for (var i = 0; i < _spriteControllers.Count; i++)
        {
            _spriteControllers[i].SetSprite(frame[i].sprite);
            _spriteControllers[i].SetFlipX(frame[i].flipX);
        }
    }
    #endregion
}
