# Mirror Puzzle Guard

Unity version: 2022.3.62f3c1

## Current Version

This is a playable Level 1 prototype for a first-person mirror-reflection puzzle game with an AI guard.

Completed features:

1. First-person player movement: WASD movement, mouse look, and Space jump.
2. Mirror-based light reflection: the beam is drawn with a LineRenderer and reflects from objects tagged as `Mirror`.
3. Receiver-door puzzle: when the beam reaches the `Receiver`, the security door opens. If the beam moves away, the door closes again.
4. Mirror interaction: approach the mirror and press `Q / E` to rotate it.
5. AI guard: the guard patrols, detects the player with a vision cone, chases the player, and triggers a failure state when close enough.
6. Win condition: after opening the door, reach the green exit zone to complete the level.
7. Runtime level construction: the project automatically builds a polished Level 1 test chamber at play time, including walls, floor tiles, props, cover objects, a door frame, a mirror pedestal, a light emitter, and a receiver stand.

## How to Run

1. Open the `Project` folder with Unity Hub.
2. Open `Assets/Scenes/MainScene.unity`.
3. Click Play.
4. Controls:
   - `WASD`: Move
   - `Mouse`: Look around
   - `Space`: Jump
   - `Q / E`: Rotate the mirror when close to it
   - `R`: Restart after winning or losing
   - `Esc`: Unlock the mouse cursor

## Level 1 Gameplay

The mirror is not aligned correctly at the start. The player must approach the mirror and rotate it until the yellow beam reflects into the red receiver. When the receiver turns green, the security door opens. The player then needs to avoid the AI guard and enter the green exit zone.

## Main Scripts

- `LightReflection.cs`: Handles beam emission, raycast collision, mirror reflection, and receiver activation.
- `MirrorController.cs`: Allows the player to rotate the mirror when nearby.
- `Receiver.cs`: Receives the beam and controls the security door.
- `PlayerController.cs`: Handles first-person movement and camera control.
- `GuardAI.cs`: Handles guard patrol, vision detection, chasing, and searching.
- `GameManager.cs`: Handles level hints, failure state, win state, and restart.
- `ExitZone.cs`: Triggers level completion.
- `LevelOneBootstrapper.cs`: Builds the Level 1 test chamber and visual polish at runtime.

## Visual Direction

This prototype currently uses Unity primitive objects with improved proportions, materials, lighting, and layout. For the next visual upgrade, there are two possible paths:

1. Add textures and materials for walls, floors, metal frames, glass, warning signs, and the security door.
2. Replace primitive shapes with modular 3D assets, such as wall panels, sci-fi doors, laboratory props, and a proper guard model.

Textures improve the surface appearance, but they do not change the silhouette of an object. Better modeling requires improved geometry, modular assets, or imported 3D models.

## Suggested Next Commits

1. Add Level 2 with two mirrors and multiple reflections.
2. Replace the temporary OnGUI hints with a Canvas and TextMeshPro UI.
3. Add door opening animation.
4. Add sound effects for beam activation, door opening, and guard alert.
5. Replace primitive visual placeholders with imported 3D models or modular environment assets.

## Day 3 Update

This project now contains two playable scenes:

- `MainScene`: Level 1, single-mirror laser puzzle with AI guard.
- `Level02`: Level 2, two-mirror laser puzzle with AI guard and final exit.

Level 1 automatically transitions into Level 2 when the player reaches the green exit zone. The receiver now drives an animated security door instead of instantly hiding the door object.

### Day 3 Test Flow

1. Open `Assets/Scenes/MainScene.unity`.
2. Press Play.
3. Solve Level 1 and enter the green exit zone.
4. The game loads `Level02` automatically.
5. Use `Mirror_01` and `Mirror_02` to redirect the beam into the receiver.
6. Reach the final green exit zone to complete the current game version.
