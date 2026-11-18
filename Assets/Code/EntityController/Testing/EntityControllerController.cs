using System.Collections.Generic;
using UnityEngine;

public class EntityControllerController : MonoBehaviour
{
    private EntityController enemyController = new();


    public void Initialize(EntityData entityData, List<SpriteController> spriteControllers, Vector2Int position, Vector2 mapPositionOffset)
    {
        OnInitialize(entityData, spriteControllers, position, mapPositionOffset);
    }
    public void UpdateEntity()
    {
        enemyController.Update();
    }
    private void OnInitialize(EntityData entityData, List<SpriteController> spriteControllers, Vector2Int position, Vector2 mapPositionOffset)
    {
        enemyController.InjectSpriteControllers(spriteControllers);
        enemyController.SetMapPositionOffset(mapPositionOffset);
        enemyController.SetCollisionFlags(entityData.InitialCollisionFlags);
        enemyController.InitializeGridBasedColliders(entityData);
        enemyController.SetAnimationFrames(entityData.AnimationFrames);
        enemyController.SetStateTree(entityData.EntityStateTree.GetClone());
        enemyController.Initialize(transform, position);
    }
    public void ReturnToPool() => enemyController.ReturnToPool();
}
