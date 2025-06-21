using UnityEngine;

public interface IParentSynchronizeService
{
    public Transform IParent { get; }
    public string ISynchronizerId { get; }
}
