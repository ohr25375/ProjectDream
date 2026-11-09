using UnityEngine;

[System.Serializable]
public class ExitToPrevious : ExitToStatesBase
{
    public override int GetExitStateIndex(int currentIndex)
    {
        return currentIndex - 1;
    }
}
