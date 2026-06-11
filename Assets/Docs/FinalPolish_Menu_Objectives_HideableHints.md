# Final Polish: Menus, Dynamic Objectives, Hideable Hints

Implemented:
1. Level 1 is treated as the tutorial objective flow.
2. Main menu, pause menu, controls screen, and level select are added through GlobalMenuUI.
3. Press H to show/hide gameplay help, objectives, and interaction prompts.
4. Press ESC to open/close pause menu.
5. Dynamic objective UI shows the current task, reflection count, receiver state, and gate state.
6. Mirror rail highlight shows the currently selected rail in orange.
7. Laser color feedback:
   - Yellow: not enough reflections yet.
   - Blue: correct reflection count.
   - Red: too many reflections / decoy route.
8. Guard status labels show PATROL / SEARCH / ALERT / STUNNED when help is visible.
9. Existing top-right counter keeps inventory and reflection count.
10. Tutorial, menus, prompts, and counters use the cartoon GUI style.
