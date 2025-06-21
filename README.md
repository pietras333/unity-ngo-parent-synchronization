# unity-ngo-parent-synchronization
Custom solution to parenting objects in Unitys NGO. Solution was tested only on distributed authority model.

For solution to work you need to:
1. On your network prefab have ClientNetworkTransform (In Local Space True), NetworkLateJoinParentSynchronizer and NetworkObject attached. On NetworkObject set AllowOwnerToParent to true, SyncOwnerTransformWhenParented to true and AutoObjectParentSync to false.

![image](https://github.com/user-attachments/assets/1dc42460-035a-4a59-b6bc-daff87936841)

3. Attach NetworkParentSynchronizer (usually to an object that is parent of desired future parent of object e.g. Player (NetworkParentSynchronizer) -> arm -> hand (future parent), and reference it in code, set its config:
![image](https://github.com/user-attachments/assets/acd02cc2-fec3-449a-9e69-5088254fb1c3)

Note: NetworkParentSynchronizer can be configured in editor for e.g. static spawnging or like in previous point with code.
