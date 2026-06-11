# No Jump, Camera Clipping, and Sensitivity Fix

Fixes:
1. Jump is disabled by default because this mirror puzzle game does not need vertical movement.
2. Mouse sensitivity is increased from 100 to 185 for faster turning.
3. Player controls are blocked while the main/pause menu is open.
4. Camera wall clipping protection is added:
   - The camera restores its intended third-person position each frame.
   - If a wall or ceiling blocks the line from player to camera, the camera is pulled forward.
   - This prevents seeing outside the map when standing close to walls.
5. Boundary walls are taller and thicker.
6. Ceiling is raised and expanded slightly to reduce visible outside clipping.
