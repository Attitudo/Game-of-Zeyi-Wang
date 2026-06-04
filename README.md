# Mirror Puzzle Guard

A Unity 2022.3 3D puzzle game about redirecting laser beams with mirrors while avoiding AI security guards.

## Current Version

This version contains four playable levels. All main level geometry and gameplay objects are saved directly inside the Unity scene files instead of being generated only at runtime. This makes the project easier to inspect, edit, and present.

1. **Level 1** - Basic mirror reflection puzzle with one AI guard.
2. **Level 2** - Two-mirror laser puzzle with a manual laser switch. The door starts closed, the laser starts off, and the mirror angles are intentionally scrambled.
3. **Level 3** - Three-mirror puzzle, Security Keycard pickup, EMP Device weapon, and a stronger guard route.
4. **Level 4** - Final challenge with four mirrors, two Energy Cores, a Master Keycard, two AI guards, and limited EMP charges.

## Controls

- `WASD` - Move
- `Mouse` - Look around
- `Space` - Jump
- `Q / E` - Rotate a nearby mirror
- `X` - Toggle the Level 2 laser switch
- `F` - Fire EMP stun weapon after picking up the EMP Device
- `R` - Restart after win/loss
- `Esc` - Unlock mouse cursor

## Gameplay Systems

- Persistent scene-based level layouts
- Mirror-based laser reflection
- Receiver-powered animated security doors
- Level 2 manual laser switch
- AI guards with patrol, chase, search, and EMP stun states
- Inventory pickups: Security Keycards, Energy Cores, EMP Device, EMP Charges
- Item-locked exit zones
- Four-level scene progression

## How to Open

1. Open Unity Hub.
2. Choose **Open**.
3. Select this project folder.
4. Open `Assets/Scenes/MainScene.unity`.
5. Press Play.

## Build Settings

The following scenes are included in Build Settings:

1. `Assets/Scenes/MainScene.unity`
2. `Assets/Scenes/Level02.unity`
3. `Assets/Scenes/Level03.unity`
4. `Assets/Scenes/Level04.unity`

## GitHub Commit Suggestion

```bash
git add .
git commit -m "Make levels persistent and add advanced level challenges"
git push
```


## Day 5 Fixes

- Guards now use collision-aware movement and should not pass through level geometry.
- EMP now has a center crosshair, wider hit detection, and aim assist.
- Level 1 laser alignment is easier because mirror rotation is finer and receiver detection is more forgiving.
