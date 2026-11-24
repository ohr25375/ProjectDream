using UnityEngine;

[System.Serializable]
public class MapEventTrigger_UnlockExit : MapEventTrigger
{
    [SerializeField] private CompassDirections direction;
    protected override void OnTriggerEvent()
    {
        if (ScreenController.UnlockExit(direction))
        {
            isSolid = false;
        }
        else
        {
            interactedCount--;
        }
    }
}
