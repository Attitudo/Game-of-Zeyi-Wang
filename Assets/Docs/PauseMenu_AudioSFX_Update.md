# Pause Menu and Audio/SFX Update

Fixes:
1. Pause menu title/subtitle layout is adjusted so text no longer clips or overlaps.
2. Added procedural lightweight audio assets:
   - relaxed background music
   - switch on/off sound
   - mirror rotate sound
   - mirror rail slide sound
   - receiver powered sound
   - EMP fire sound
   - guard alert sound
   - player caught sound
3. Added GameAudio.cs to load and play clips from Resources/Audio.
4. Hooked SFX into:
   - LaserSwitch.ToggleLaser
   - MirrorController.RotateMirror
   - MovableMirrorRail.Slide
   - Receiver.SetPowered
   - EMPWeapon.Fire
   - GuardAI first alert
   - GameManager.PlayerCaught
5. Controls text no longer mentions jump because jump is disabled.
