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
Player (NetworkParentSynchronizer)
└── Arm
└── Hand (actual parent of the child object)

You can configure the `NetworkParentSynchronizer` either in the editor or dynamically via code.

![Parent Synchronizer Setup](https://github.com/user-attachments/assets/acd02cc2-fec3-449a-9e69-5088254fb1c3)

---

## 🛠️ Usage

### Static Parenting (e.g. at spawn):
Configure `NetworkParentSynchronizer` in the Inspector with preset parent references.

### Dynamic Parenting (e.g. during runtime):
Use scripting to assign parent references and trigger synchronization manually.

> ⚠️ **Important:** `NetworkParentSynchronizer` must always be present **somewhere above** the intended parent in the object hierarchy.

---

## 🧪 Tested Model

- ✅ **Distributed Authority Model**  
- ❌ Not tested in Server- or Host-authority modes (PRs welcome)

---

## 📌 Notes

- This system is designed to bypass limitations in NGO’s default parenting and syncing behavior.
- It’s lightweight and customizable — ideal for use cases like item pickup, weapon attachment, or modular avatars.

---

## 📄 License

MIT License — free to use and modify with attribution.
