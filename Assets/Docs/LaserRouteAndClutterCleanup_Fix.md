# Laser Route and Clutter Cleanup Fix

Changes in this version:

1. Decorative clutter is removed from the playable scenes:
   - torches / candle-like flame objects
   - decorative stone columns and caps
   - banners and supply crates
   - DungeonTheme_Props root groups

2. Terrain and gameplay objects are preserved:
   - floors, boundary walls and ceilings
   - cover / puzzle blocker objects
   - mirrors, laser source, receiver, doors, exit zones
   - guards and pickups

3. Later-level laser routes were repaired:
   - Level 5 and Level 6 no longer spawn large runtime laser blocker walls across the intended optical path.
   - Mirror angles are computed from the intended route: LightSource -> Mirrors -> Receiver.
   - The laser distance and receiver tolerance remain increased for the expanded maps.

Recommended commit:
`git add . && git commit -m "Clean clutter and fix advanced laser routes"`
