# Single Mirror Control Fix

Fix:
- Only the nearest controllable mirror responds to Q/E rotation and Z/C rail sliding.
- This prevents multiple mirrors from rotating or sliding at the same time when their interaction ranges overlap.
- Rail endpoint recovery is kept: the player can still pull a mirror back from a rail end by standing near the rail.
