using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public class EntityData : ScriptableObject
{
    [SerializeField] private EntityStateTree _entityStateTree;
    public EntityStateTree EntityStateTree => _entityStateTree;
    [SerializeField] private List<Vector2> _colliderOffsets;
    public List<Vector2> ColliderOffsets => _colliderOffsets;
    [SerializeField] private TileCollisionFlags _initialCollisionFlags;
    public TileCollisionFlags InitialCollisionFlags => _initialCollisionFlags;
    [SerializeField] private List<EntityAnimationFrame> _animationFrames;
    public List<EntityAnimationFrame > AnimationFrames => _animationFrames;
    [System.Serializable]
    public class EntityAnimationFrame
    {
        public List<EntityAnimationFramePart> parts;
        [System.Serializable]
        public struct EntityAnimationFramePart
        {
            public Sprite sprite;
            public bool flipX;
        }
    }
}
