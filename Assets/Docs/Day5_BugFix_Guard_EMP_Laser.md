# Day 5 Bug Fix: Guard Collision, EMP Aiming, and Level 1 Laser

## Changes

1. Guard collision has been improved.
   - Guards now use a CharacterController for movement.
   - They should no longer pass through walls, crates, doors, or other solid level geometry.

2. EMP aiming has been improved.
   - A cyan center crosshair appears after the player picks up the EMP device.
   - EMP uses a wider SphereCast plus aim assist, so it is easier to hit guards.
   - A successful hit stuns the guard and changes its color to cyan.

3. Level 1 laser alignment has been improved.
   - Mirror rotation step was reduced from 15 degrees to 5 degrees.
   - Receiver detection tolerance was increased.
   - Level 1 Mirror_01 starts closer to a solvable angle.

## Test Checklist

- Start from MainScene.
- Rotate Mirror_01 with Q / E until the receiver turns green.
- Confirm the door opens.
- In Level03 or Level04, pick up the EMP device.
- Aim the crosshair at a guard and press F.
- Confirm the guard turns cyan and stops moving temporarily.
- Confirm guards cannot pass through solid geometry.
