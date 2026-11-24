using UnityEngine;

[System.Serializable]
public abstract class MapEventTrigger : System.ICloneable
{
    [SerializeField] protected int interactableCount = -1;
    [SerializeField] protected bool isSolid = true;
    [SerializeField] protected int tileIndex;
    [SerializeField] protected bool isVisible = true;
    public bool IsSolid => isSolid;
    public int TileIndex => tileIndex;
    public bool IsVisible => isVisible;
    public bool IsInteractable => interactableCount - interactedCount != 0 && interactableCount != 0;
    protected int interactedCount = 0;

    public MapEventTrigger Clone()
    {
        return MemberwiseClone() as MapEventTrigger;
    }
    object System.ICloneable.Clone()
    {
        return Clone();
    }

    public void TriggerEvent()
    {
        if (!IsInteractable) { return; }
        interactedCount++;
        OnTriggerEvent();
    }
    public void DisableAll()
    {
        Disable();
        SetVisibility(false);
        SetSolidState(false);
    }
    public void Disable() => interactableCount = 0;
    public void SetVisibility(bool isVisible) => this.isVisible = isVisible;
    public void SetSolidState(bool isSolid) => this.isSolid = isSolid;

    protected abstract void OnTriggerEvent();
}
