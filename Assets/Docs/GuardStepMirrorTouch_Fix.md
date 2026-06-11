# Guard AI, Walking Step, and Mirror Touch Fix

Fixes:
1. Guard AI now has wall-aware steering:
   - It sphere-casts in front.
   - If a wall/cover blocks the forward route, it slides along the wall instead of staring into it.
   - If a patrol guard is stuck, it advances to the next patrol point.
2. Character walking feedback is improved:
   - The stable character model now bobs/sways more visibly.
   - Small procedural step feet alternate while the player/guard moves.
3. Mirror interaction feedback is added:
   - When the player is close to the active mirror/rail mirror, a simple procedural arm/hand reaches toward the mirror.
   - This makes it look like the character is touching or adjusting the mirror.
4. The previous character model overlap fix is kept.
5. The previous EMP weapon pickup visibility fix is kept.
