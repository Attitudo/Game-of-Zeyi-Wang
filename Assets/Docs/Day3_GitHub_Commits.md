# Day 3 Development Notes

## Completed Features

1. Added `Level02.unity` as the second playable scene.
2. Added scene transition from `MainScene` to `Level02` through `ExitZone`.
3. Added a two-mirror puzzle in Level 2.
4. Added an animated security door that moves upward when the receiver is powered.
5. Updated the objective UI so Level 1 and Level 2 show different English instructions.
6. Added Level 2 AI guard patrol, detection, chase, and final exit logic.

## Suggested Git Commits

```bash
git add .
git commit -m "Add level two scene and build settings"

git add .
git commit -m "Add transition from level one to level two"

git add .
git commit -m "Create two mirror puzzle for level two"

git add .
git commit -m "Add animated security door behavior"

git push
```

## Test Checklist

1. Open `Assets/Scenes/MainScene.unity`.
2. Press Play.
3. Solve Level 1 and reach the green exit zone.
4. The game should load `Level02` automatically.
5. In Level 2, redirect the beam through `Mirror_01` and `Mirror_02`.
6. The receiver should turn green and the security door should move upward.
7. Avoid the guard and reach the final exit zone.
8. Press `R` after win or loss to restart the current level.
