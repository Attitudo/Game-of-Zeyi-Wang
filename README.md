# Mirror Puzzle Guard

## 1. Project Overview

**Mirror Puzzle Guard** is a 3D cartoon-style dungeon puzzle game made with Unity.  
The player is trapped inside an underground **Mirror Vault** and must use light, mirrors, switches, receivers, and EMP devices to escape from a series of locked rooms.

The core gameplay is based on laser reflection. The player rotates mirrors, slides rail mirrors, powers lamps with switches, and guides a laser beam into a receiver. When the receiver is powered with the correct reflection path, the gate opens and the player can move to the next level.

The game also includes guard AI, EMP shooting, level progression, story messages, UI menus, audio feedback, and multiple puzzle levels.

---

## 2. Story

The player is an apprentice light engineer who wakes up inside an underground facility called the **Mirror Vault**.  
The vault was originally designed to protect a powerful crystal known as the **Sun Core**.

After the security system loses control, the vault gates are locked and robotic guards begin attacking intruders. To escape, the player must restore the light routes in each chamber by reflecting laser beams through ancient mirrors.

Each level represents a deeper part of the vault:

- **Level 1 - Tutorial Entrance:** Learn basic mirror reflection.
- **Level 2 - Switch Room:** Turn on the lamp and restore the first powered beam.
- **Level 3 - Guard Vault:** Avoid guards and pick up the EMP device.
- **Level 4 - Mirror Chain:** Complete a multi-mirror reflection puzzle.
- **Level 5 - Expert Maze:** Avoid decoy mirrors and solve a harder path.
- **Level 6 - Final Catacomb:** Restore the Sun Core and escape the vault.

Story messages are shown in-game through trigger logs near the spawn area of each level.

---

## 3. Main Features

### Laser Reflection Puzzle

- A laser beam is emitted from the light source.
- The laser uses raycasting to detect mirrors, walls, receivers, and blockers.
- When the laser hits a mirror, the reflection direction is calculated with `Vector3.Reflect`.
- A `LineRenderer` draws the full laser path.
- The game counts how many unique mirrors the beam reflects from.

### Exact Reflection Count

Each level requires an exact number of mirror reflections.  
For example:

- Level 1 requires 1 mirror.
- Level 2 requires 2 mirrors.
- Level 3 requires 3 mirrors.
- Level 4 requires 4 mirrors.
- Level 5 requires 5 mirrors.
- Level 6 requires 6 mirrors.

If the player uses too few mirrors, the receiver will not activate.  
If the player uses too many mirrors or decoy mirrors, the receiver will also reject the path.

### Mirror Interaction

- `Q / E`: Rotate the nearest mirror.
- `Z / C`: Slide the nearest rail mirror.
- Only the nearest mirror can respond at one time, preventing multiple mirrors from moving together.
- Rail mirrors start away from the correct position, so players must actively solve the puzzle.

### Switch and Lamp System

From Level 2 onward, the laser starts turned off.  
The player must find a floor lever switch near the spawn area and press `X` to power the lamp.

The switch is placed on a small pedestal box so it is easier to recognize.

### Receiver and Door

The receiver is represented as a lamp/crystal target.

- Before receiving light, the receiver is dim.
- When the correct laser path hits the receiver, it lights up and becomes bright green.
- The powered receiver opens the gate.
- The gate is sealed on both sides so the player cannot bypass it.

### Guard AI

Guards use a simple state machine:

- `Patrol`
- `Chase`
- `Search`
- `Stunned`

The guard checks:

- Distance to player
- Viewing angle
- Raycast line of sight

If the guard sees the player, it chases them.  
If the player is caught, movement stops and the player must restart.

### EMP System

The player can pick up an EMP device in later levels.

- `F`: Fire EMP
- EMP uses a sphere cast from the camera direction.
- If the EMP hits a guard, the guard is stunned temporarily.
- EMP charges are shown in the UI.

The EMP weapon is hidden until the player actually picks up the EMP device.

### UI and Menus

The game includes:

- Main menu
- Pause menu
- Level select menu
- Controls menu
- Dynamic objective UI
- Reflection counter
- Inventory counter
- Hide/show help system

Controls:

- `WASD`: Move
- Mouse: Look around
- `Q / E`: Rotate mirror
- `Z / C`: Slide rail mirror
- `X`: Use switch
- `F`: Fire EMP
- `H`: Show/hide help
- `ESC`: Pause
- `R`: Restart after being caught

Jumping is disabled because the game is designed as a ground-based puzzle game.

### Audio

Audio is managed by `GameAudio.cs`.

Audio files are stored in:

```text
Assets/Resources/Audio/
```

Current audio events include:

- Background music
- Switch sound
- Mirror rotation sound
- Mirror sliding sound
- Receiver power sound
- EMP fire sound
- Guard alert sound
- Caught/failure sound

The sound files can be replaced with custom audio by using the same filenames.

---

## 4. Implementation Details

### Laser System

The laser system is implemented with raycasting.  
A ray is emitted from the light source. When it hits a mirror, the next direction is calculated using:

```csharp
Vector3.Reflect(incomingDirection, hit.normal)
```

The laser path is stored as a list of points and rendered using a `LineRenderer`.

### Receiver Logic

The receiver only activates when:

1. The laser reaches the receiver.
2. The number of reflected mirrors equals the required number for the current level.

When powered, the receiver updates its lamp visual and opens the door through `DoorAnimator`.

### Mirror Control

Each mirror checks whether the player is close enough to interact.  
To avoid multiple mirrors responding to the same input, the project selects only the nearest valid mirror or rail mirror.

### Guard AI

The guard AI uses a state machine.  
It patrols between waypoints, detects the player using distance/angle/raycast checks, and can chase or search for the player.  
EMP can temporarily change the guard into the stunned state.

### UI

The UI uses Unity `OnGUI`.  
The player can press `H` to hide large hints so the UI does not block the view.

### Audio

The `GameAudio` manager loads clips from `Resources/Audio` and exposes static methods such as:

```csharp
GameAudio.PlaySwitch();
GameAudio.PlayMirrorRotate();
GameAudio.PlayReceiverPower();
GameAudio.PlayEmpFire();
```

---

## 5. Development Process

This project was developed through several stages:

1. Basic Unity project and first playable scene
2. Laser reflection system
3. Mirror rotation and rail mirror movement
4. Receiver and door system
5. Multi-level design
6. Guard AI and EMP mechanics
7. UI menus and dynamic objectives
8. Audio and visual polish
9. Bug fixing and final presentation preparation

A GitHub Project Kanban board is used retrospectively to organize the full development process, including completed tasks, testing tasks, current polish work, and future improvements.

---

## 6. How to Run

1. Open the project in Unity.
2. Use Unity version 2022.3 or a compatible version.
3. Open the main scene.
4. Press Play.
5. Use the main menu to start the game or select a level.

---

## 7. Known Issues / Testing Focus

The following areas should be tested before final presentation:

- Whether every level can be completed.
- Whether receiver lamp feedback is clear.
- Whether the gate side walls fully block bypassing.
- Whether switches are placed correctly in each level.
- Whether audio volume is balanced.
- Whether guard detection feels fair.
- Whether mirror interaction only controls one mirror at a time.

---

## 8. Future Improvements

Possible future improvements include:

- Replace temporary sound effects with higher-quality audio.
- Add better character animations.
- Add volume settings.
- Add save/load progress.
- Add more levels.
- Add final cutscene for the Sun Core ending.
- Replace primitive objects with imported models.
- Add more story notes and environmental storytelling.

---

## 9. Team Workflow

The project uses GitHub for version control.  
The Kanban board is organized into:

- **Backlog**
- **Todo**
- **In Progress**
- **Testing**
- **Done**

The board was created retrospectively near the end of development to clearly present the actual development process, completed features, remaining tests, and final polish tasks.
