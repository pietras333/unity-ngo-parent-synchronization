using Unity.Netcode;
using UnityEngine;

public class NetworkParentSynchronizer : NetworkBehaviour, IParentSynchronizeService
{
    [SerializeField] private Transform futureParent;
    [SerializeField] private string synchronizerId;
    [SerializeField] private GameObject childPrefab;

    public Transform IParent => futureParent;
    public string ISynchronizerId => synchronizerId;

    public void Configure(Transform parent, GameObject prefab, string syncId)
    {
        futureParent = parent;
        childPrefab = prefab;
        synchronizerId = syncId;
    }

    public void TriggerSync(out NetworkObject spawnedObject)
    {
        spawnedObject = null;
        if (!IsOwner) return;

        GameObject futureChildLocal = Instantiate(childPrefab, IParent.position, IParent.rotation, IParent);
        futureChildLocal.transform.PartialResetLocalTransform();

        NetworkObject futureChildNetworkObject = futureChildLocal.GetComponent<NetworkObject>();
        futureChildNetworkObject.GetComponent<NetworkLateJoinParentSynchronizer>()
            .Configure(futureParent, ISynchronizerId);

        futureChildNetworkObject.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);
        spawnedObject = futureChildNetworkObject;

        futureChildLocal.transform.SetParent(IParent, false);
        futureChildLocal.transform.PartialResetLocalTransform();

        if (IsServer)
        {
            SyncClientRpc(futureChildNetworkObject, NetworkObject.NetworkObjectId);
        }
        else
        {
            SyncServerRpc(futureChildNetworkObject, NetworkObject.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncServerRpc(NetworkObjectReference spawnedObjectRef, ulong ownerPlayerNetworkObjectId)
    {
        SyncClientRpc(spawnedObjectRef, ownerPlayerNetworkObjectId);
    }

    [ClientRpc]
    private void SyncClientRpc(NetworkObjectReference spawnedObjectRef, ulong ownerPlayerNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(spawnedObjectRef.NetworkObjectId,
                out NetworkObject spawnedObject))
        {
            if (spawnedObject.OwnerClientId == NetworkManager.Singleton.LocalClientId) return;

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ownerPlayerNetworkObjectId,
                    out NetworkObject ownerObject))
            {
                IParentSynchronizeService parentService = ownerObject.GetComponent<IParentSynchronizeService>();
                if (parentService != null && parentService.ISynchronizerId == ISynchronizerId)
                {
                    spawnedObject.transform.SetParent(parentService.IParent, false);
                }
            }
        }
    }
}
