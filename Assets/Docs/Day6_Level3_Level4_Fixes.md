# Day 6 Fixes: Level 3 and Level 4

## Fixed issues

1. Laser beams are no longer accidentally blocked by decorative walls or level geometry.
   - The laser now only interacts with mirrors and receivers.
   - This prevents Level 3 and Level 4 beam paths from being interrupted by wall pieces.

2. EMP crosshair is available in Level 4.
   - Level 4 now starts the player with an EMP device and 3 EMP charges.
   - The cyan center crosshair appears when the EMP device is available.

3. The exit zone now requires the security door to be opened first.
   - The player can no longer complete the level by stepping onto the exit platform before solving the laser-door puzzle.

4. Guard vision is now visible in game.
   - Each guard displays a fan-shaped vision cone.
   - Yellow cone: patrol/search area.
   - Red cone: chasing the player.
   - Cyan cone: stunned by EMP.

## Suggested Git commit

```bash
git add .
git commit -m "Fix level three and four laser EMP exit and guard vision"
git push
```
