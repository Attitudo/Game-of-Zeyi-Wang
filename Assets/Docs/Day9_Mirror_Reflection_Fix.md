# Day 9 - Mirror Reflection Fix

This patch fixes the laser reflection logic after adding physical laser blockers.

## Fixed
- Mirror trigger colliders are now detected before generic trigger objects are ignored.
- Mirrors can reflect the laser again.
- Walls, crates, barriers, cover blocks and closed security doors still block the laser.
- Exit zones, pickup trigger colliders and guard vision cones do not block the laser.

## Design Rule
The intended puzzle is:

Laser -> Mirror path around blockers -> Receiver -> Door opens -> Exit unlocks.

The laser should not pass through real walls or crates, but it should reflect from mirror surfaces even if the mirror collider is a trigger.
