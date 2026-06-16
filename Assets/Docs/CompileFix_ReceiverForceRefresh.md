# Compile Fix: Receiver ForceRefreshVisual

Fix:
- Added public ForceRefreshVisual() to Receiver.cs.
- SceneExpansionAndLevelConfig.cs calls this method when rebuilding the receiver lamp visual.
- This resolves CS1061: Receiver does not contain a definition for ForceRefreshVisual.
