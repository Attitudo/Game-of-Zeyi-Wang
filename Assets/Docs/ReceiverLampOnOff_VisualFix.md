# Receiver Lamp On/Off Visual Fix

Fixes:
1. Receiver is rebuilt as a clearer lamp target with:
   - pedestal
   - pole
   - circular target disc
   - crystal lamp
   - powered halo
   - point light
2. Before being hit, the receiver is dark cyan with a very weak light.
3. After being hit, the crystal turns bright green, a halo appears, and the light intensity increases.
4. Receiver visual updates on Start, when powered changes, and when the runtime scene config rebuilds the lamp.
5. Original receiver collider is preserved for laser hit detection, while its plain renderer stays hidden.
