using UnityEngine;

[System.Serializable]
public abstract class ExitToStatesBase
{
    public abstract int GetExitStateIndex(int currentIndex);
}
