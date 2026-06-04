# Day 7 Fixes: Laser, Exit Lock, and Guard Vision

## Fixed issues

1. Laser beams no longer stop on decorative walls, mirror frames, crates, floors, or door frames.
   - The laser now reacts only to objects tagged `Mirror` and objects with a `Receiver` component.
   - Mirror child frame objects are ignored so the beam hits the actual mirror surface.

2. Exit zones cannot complete the level before the security door is opened.
   - The exit checks the `DoorAnimator` state.
   - If the door is closed, the player is pushed away from the exit zone and a message is shown.

3. Guard vision cones are easier to see.
   - The vision cone mesh is double-sided.
   - It uses a transparent material and changes color based on AI state.

## Test checklist

- Level 3 laser should pass decorative obstacles and only reflect on mirror surfaces.
- Level 4 exit should not complete while the door is closed.
- When standing on a locked exit zone, the player should see a lock message and be pushed back.
- Guard vision cones should be visible on the floor.
