using Unity.Netcode;
using UnityEngine;

public class NetworkLateJoinParentSynchronizer : NetworkBehaviour, IParentSynchronizeService
{
    #region IParentSynchronizeService Implementation

    public Transform IParent => futureParent;
    public string ISynchronizerId => synchronizerId;

    #endregion

    #region Private Fields

    private NetworkVariable<ulong> parentNetworkObjectId = new();
    private Transform futureParent;
    private string synchronizerId;

    #endregion

    #region Public Methods

    public void Configure(Transform parent, string syncId)
    {
        futureParent = parent;
        synchronizerId = syncId;
    }

    #endregion

    #region Network Lifecycle

    public override void OnNetworkSpawn()
    {
        InitializeParentReference();
        SynchronizeParenting();
    }

    #endregion

    #region Initialization

    private void InitializeParentReference()
    {
        if (!IsOwner) return;

        ulong clientId = NetworkObject.OwnerClientId;
        ulong parentObjectId = FindParentObjectId(clientId);

        if (parentObjectId != 0)
        {
            parentNetworkObjectId.Value = parentObjectId;
        }
        else
        {
            Debug.LogError($"Could not find parent object for client ID: {clientId}");
        }
    }

    private ulong FindParentObjectId(ulong clientId)
    {
        foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            if (!IsValidParentCandidate(spawnedObject, clientId)) continue;

            if (spawnedObject.TryGetComponent<IParentSynchronizeService>(out var parentService) &&
                parentService.ISynchronizerId == synchronizerId)
            {
                Debug.Log($"Found parent object for client {clientId}");
                return spawnedObject.NetworkObjectId;
            }
        }

        return 0;
    }

    private bool IsValidParentCandidate(NetworkObject networkObject, ulong clientId)
    {
        return networkObject.OwnerClientId == clientId;
    }

    #endregion

    #region Synchronization

    private void SynchronizeParenting()
    {
        if (IsServer)
        {
            SyncParentClientRpc(NetworkObject, parentNetworkObjectId.Value);
        }
        else
        {
            SyncParentServerRpc(NetworkObject, parentNetworkObjectId.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncParentServerRpc(NetworkObjectReference spawnedObjectRef, ulong ownerPlayerNetworkObjectId)
    {
        SyncParentClientRpc(spawnedObjectRef, ownerPlayerNetworkObjectId);
    }

    [ClientRpc]
    private void SyncParentClientRpc(NetworkObjectReference spawnedObjectRef, ulong ownerPlayerNetworkObjectId)
    {
        if (!TryGetSpawnedObject(spawnedObjectRef, out NetworkObject spawnedObject)) return;
        if (!TryGetParentService(ownerPlayerNetworkObjectId, out IParentSynchronizeService parentService)) return;

        ApplyParenting(spawnedObject, parentService);
    }

    private void ApplyParenting(NetworkObject childObject, IParentSynchronizeService parentService)
    {
        childObject.transform.SetParent(parentService.IParent, false);
        childObject.transform.PartialResetLocalTransform();
    }

    #endregion

    #region Helper Methods

    private bool TryGetSpawnedObject(NetworkObjectReference objectRef, out NetworkObject spawnedObject)
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
            objectRef.NetworkObjectId, out spawnedObject);
    }

    private bool TryGetParentService(ulong parentNetworkObjectId, out IParentSynchronizeService parentService)
    {
        parentService = null;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
                parentNetworkObjectId, out NetworkObject parentObject))
        {
            return false;
        }

        return parentObject.TryGetComponent(out parentService);
    }

    #endregion
}
