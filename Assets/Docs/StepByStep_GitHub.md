# Step-by-Step GitHub Commit Plan

This document gives a clean commit history you can show during a project presentation. Each stage represents one small, understandable development step.

## Commit 0: Upload the original prototype

```bash
git init
git add .
git commit -m "Upload original Unity prototype"
git branch -M main
git remote add origin YOUR_GITHUB_REPOSITORY_URL
git push -u origin main
```

## Commit 1: Add first-person player control

```bash
git add Assets/Scripts/PlayerController.cs
git commit -m "Add first-person player controller"
git push
```

## Commit 2: Improve beam reflection logic

```bash
git add Assets/Scripts/LightReflection.cs Assets/Scripts/Receiver.cs
git commit -m "Add mirror beam reflection and receiver logic"
git push
```

## Commit 3: Add mirror interaction

```bash
git add Assets/Scripts/MirrorController.cs
git commit -m "Add interactive mirror rotation"
git push
```

## Commit 4: Add AI guard gameplay

```bash
git add Assets/Scripts/GuardAI.cs Assets/Scripts/GameManager.cs Assets/Scripts/ExitZone.cs
git commit -m "Add AI guard patrol and level win lose states"
git push
```

## Commit 5: Build a playable Level 1 test chamber

```bash
git add Assets/Scripts/LevelOneBootstrapper.cs Assets/Scenes/MainScene.unity ProjectSettings/EditorBuildSettings.asset
git commit -m "Build playable Level 1 test chamber"
git push
```

## Commit 6: Convert project text to English and polish visuals

```bash
git add .
git commit -m "Convert UI text to English and polish level visuals"
git push
```

## Important Unity Git Ignore Rules

Do not commit these generated folders:

- `Library`
- `Temp`
- `Logs`
- `Obj`
- `Build`
- `Builds`
- `.vs`

This project already includes a `.gitignore` file for those folders.
