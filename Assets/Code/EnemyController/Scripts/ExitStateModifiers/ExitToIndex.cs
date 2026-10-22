using UnityEngine;

[System.Serializable]
public class ExitToIndex : ExitToStatesBase
{
    [SerializeField] private int stateIndex;
    public override int GetExitStateIndex(int currentIndex)
    {
        return stateIndex;
    }
}
