# Clean Structure Refactor

This version removes or simplifies code that only existed because of earlier temporary fixes.

Changes:
1. Removed unused compatibility scripts that were not referenced by scenes or prefabs:
   - AdvancedLevelsBootstrapper.cs
   - LevelOneBootstrapper.cs
   - LevelTwoBootstrapper.cs
   - GateThresholdCleaner.cs
2. Removed old mirror-reaching arm cleanup logic from StableCharacterVisuals / StableCharacterMotion.
   The current character system no longer creates those arm/hand objects, so the repeated cleanup function was unnecessary.
3. Renamed and simplified door placeholder cleanup in SceneExpansionAndLevelConfig:
   - HideLegacyDoorFramePieces(prefix)
   - became HideOriginalDoorPlaceholders()
   It now only hides original scene placeholders, while the final gate is generated cleanly by CreateRefinedExitGate().
4. Kept necessary runtime configuration systems:
   - SceneExpansionAndLevelConfig still acts as the level builder.
   - StableCharacterVisuals still hides original capsule/blocky renderers so imported models display correctly.
   - Receiver, Door, Switch, Laser, Guard, EMP, UI, and Audio systems are unchanged in behavior.

Goal:
Make the project easier to explain in presentation without changing gameplay.
