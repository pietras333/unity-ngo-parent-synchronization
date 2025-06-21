🧬 Unity NGO Parent Synchronization

A custom solution for parenting objects using Unity Netcode for GameObjects (NGO), tested in a Distributed Authority model.

This package allows you to safely manage parent-child object relationships over the network while avoiding common sync issues in NGO's built-in systems.
✅ Requirements

To use this system correctly, ensure your network prefab is set up with the following components:

    Components on the Child Object (the object to be parented):

        ClientNetworkTransform

            ✅ Set In Local Space to true

        NetworkLateJoinParentSynchronizer

        NetworkObject

            ✅ AllowOwnerToParent = true

            ✅ SyncTransformWhenChildOfAnother = true

            ✅ AutoObjectParentSync = false

    Child Object Setup

    Parent Chain Setup

        Attach the NetworkParentSynchronizer to the nearest authoritative parent or container object in the hierarchy.

            For example:

            Player (NetworkParentSynchronizer)
             └── Arm
                 └── Hand (actual parent of child object)

        Configure the NetworkParentSynchronizer via the Inspector or dynamically in code.

    Parent Synchronizer Setup

🛠️ Usage

    Static Parenting (e.g. at spawn)
    Configure NetworkParentSynchronizer in the editor with predefined parent references.

    Dynamic Parenting (e.g. picking up or attaching in runtime)
    Call the appropriate synchronization methods from your own logic (e.g., during pickup/attach/detach actions).

    ⚠️ Important: NetworkParentSynchronizer must always be present somewhere above the intended parent in the hierarchy.

🧪 Tested Model

    ✅ Tested in Distributed Authority Model

    ❌ Not tested in Host/Server-authoritative model (use at your own risk or contribute improvements)

📌 Notes

    This system is designed to bypass limitations in NGO’s default parenting and syncing behavior.

    It’s lightweight and customizable, ideal for use cases like item pickup, weapon attachment, or modular avatars.

📄 License

MIT License — free to use and modify with attribution.
