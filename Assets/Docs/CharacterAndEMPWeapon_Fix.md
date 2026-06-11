# Character Model and EMP Weapon Fix

Fixes:
1. Old procedural/blocky character model renderers are now hidden when the stable character model exists.
2. This prevents the player/guard from showing two overlapping models at the same time.
3. The hide logic also runs when the stable model was already created in the scene.
4. EMP weapon visual is forcibly hidden in Awake/Start until PlayerInventory.hasEmpDevice is true.
5. The EMP blaster is searched recursively under the camera/player, so inactive old scene children are handled correctly.
6. After the player picks up the EMP Device, the gun visual appears and F can be used normally.
