using UnityEngine;

[System.Serializable]
public class ExitToRelative : ExitToStatesBase
{
    [SerializeField] private int stateIndex;
    public override int GetExitStateIndex(int currentIndex)
    {
        return currentIndex + stateIndex;
    }
}
