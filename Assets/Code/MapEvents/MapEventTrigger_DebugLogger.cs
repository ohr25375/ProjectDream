using UnityEngine;

[System.Serializable]
public class MapEventTrigger_DebugLogger : MapEventTrigger
{
    [SerializeField] private string message;
    protected override void OnTriggerEvent()
    {
        Debug.Log(message);
    }
}
