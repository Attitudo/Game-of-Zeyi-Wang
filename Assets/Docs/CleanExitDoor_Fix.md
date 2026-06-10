# Clean Exit Door Fix

This patch fixes the messy exit gate caused by overlapping old door-frame pieces and a moving door returning to its old scene position.

Changes:
1. Hides legacy DoorFrame / previous Gate pieces at runtime.
2. Rebuilds a simpler clean gate with two posts, one top beam, a threshold, and a green exit pad.
3. Adds DoorAnimator.SnapClosedAtCurrentPosition() so the moving SecurityDoor stays aligned with the new gate.
4. Keeps the door-opening logic, receiver lock, and exit lock unchanged.
