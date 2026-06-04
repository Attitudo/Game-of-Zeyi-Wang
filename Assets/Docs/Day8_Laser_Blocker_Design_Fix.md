# Day 8 Laser Blocker Design Fix

This update restores the intended mirror puzzle rule:

- Walls, cover blocks, crates, boxes, barriers and the security door block the laser.
- The laser must be redirected around obstacles using mirror reflections.
- Mirror frames, bases, stands, floors, pickups, guards and trigger zones do not accidentally stop the laser.
- Level 3 and Level 4 cover blocks were repositioned so a valid reflected path exists.
- Exit zones now require the receiver to be powered before the level can be completed.

Recommended commit message:

```bash
git add .
git commit -m "Fix laser blockers and redesign advanced level beam paths"
git push
```
