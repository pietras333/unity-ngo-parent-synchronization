# 🧬 Unity NGO Parent Synchronization

A custom solution for parenting objects using Unity Netcode for GameObjects (NGO), tested in a **Distributed Authority** model.

This package allows you to safely manage parent-child object relationships over the network while avoiding common sync issues in NGO's built-in systems.

---

## ✅ Requirements

To use this system correctly, ensure your **network prefab** is set up with the following components:

### 1. Components on the Child Object (the object to be parented):

- `ClientNetworkTransform`  
  - ✅ Set **In Local Space** to `true`
- `NetworkLateJoinParentSynchronizer`
- `NetworkObject`
  - ✅ `AllowOwnerToParent = true`  
  - ✅ `SyncTransformWhenChildOfAnother = true`  
  - ✅ `AutoObjectParentSync = false`

![Child Object Setup](https://github.com/user-attachments/assets/1dc42460-035a-4a59-b6bc-daff87936841)

---

### 2. Parent Chain Setup

Attach the `NetworkParentSynchronizer` to the **nearest authoritative parent** or container object in the hierarchy.

Example structure:
