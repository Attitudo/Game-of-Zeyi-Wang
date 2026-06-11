# Remove Reach Arm and Stop Control on Caught Fix

Fixes:
1. Removed the procedural mirror-reaching arm/hand because it looked like a strange stick.
2. Walking step/bob animation is kept.
3. If the player is caught by a guard, player movement and camera control are now blocked.
4. If the level is completed, player movement is also blocked.
5. Horizontal Rigidbody velocity is cleared when control is blocked, so the player cannot keep sliding/walking after being caught.
