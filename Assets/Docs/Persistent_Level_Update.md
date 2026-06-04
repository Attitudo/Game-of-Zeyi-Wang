# Persistent Level Update

This update removes the dependency on runtime level generation. The main geometry, mirrors, doors, receivers, guards, pickups, and exit zones are now saved directly inside the scene files.

## Updated Scenes

- `MainScene.unity`
- `Level02.unity`
- `Level03.unity`
- `Level04.unity`

## Level 2 Fixes

- The security door starts closed.
- The laser emitter starts turned off.
- A physical laser switch has been added near the player spawn.
- Press `X` near the switch to toggle the laser.
- Mirror angles are scrambled at the start, so the puzzle requires manual adjustment.

## Level 3 and Level 4

- Level 3 adds a Security Keycard, an EMP Device, and a three-mirror puzzle.
- Level 4 adds two Energy Cores, a Master Keycard, two guards, and a four-mirror final puzzle.

## Suggested Commit

```bash
git add .
git commit -m "Make levels persistent and rebalance advanced puzzles"
git push
```
