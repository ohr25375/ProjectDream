using UnityEngine;

[System.Serializable]
public abstract class EntityValueBase
{
    public abstract int GetValue(EntityController controller);
}
