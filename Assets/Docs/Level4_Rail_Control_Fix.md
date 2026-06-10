# Level 4 and Rail Control Fix

Fixes:
1. Level 4 now uses a clean four-mirror route only.
2. Level 4 decoy mirrors are disabled to avoid accidental extra reflection count and unsolvable routes.
3. Mirror rail interaction now uses the whole rail area, not only the mirror object's current position.
4. This prevents mirrors from becoming unreachable when they slide to a rail endpoint.
5. Mirror rotation also works when the player is near the rail, not only near the mirror.
6. Receiver tolerance is slightly increased to reduce pixel-perfect laser alignment issues.
7. Exit green pad visuals are hidden more robustly; exit logic remains unchanged.
