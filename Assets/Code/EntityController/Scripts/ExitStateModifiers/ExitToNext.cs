using UnityEngine;

[System.Serializable]
public class ExitToNext : ExitToStatesBase
{
    public override int GetExitStateIndex(int currentIndex)
    {
        return currentIndex + 1;
    }
}
