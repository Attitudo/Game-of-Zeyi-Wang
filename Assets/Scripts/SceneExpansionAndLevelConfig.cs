using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps the selected base scene style, then adds stable character models,
/// expands/seals maps, configures rail mirrors/manual lasers, and refines the expert Level 5-6 layouts.
/// </summary>
public class SceneExpansionAndLevelConfig : MonoBehaviour
{
    private static SceneExpansionAndLevelConfig instance;
    private Material floorMaterial;
    private Material wallMaterial;
    private Material coverMaterial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("SceneExpansionAndLevelConfig");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<SceneExpansionAndLevelConfig>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ApplyCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyCurrentScene();
    }

    private void ApplyCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        LevelLayout layout = GetLayout(sceneName);
        if (!layout.valid)
        {
            return;
        }

        ApplyCharacterModels();
        HideDecorativeClutter();
        CacheMaterials(layout.existingPrefix);
        EnsureMirrorCount(layout.mirrors.Length);
        RepositionGameplayObjects(layout);
        ConfigureAdvancedGuards(sceneName, layout);
        ConfigureMirrorRails(layout);
        CreateDecoyMirrors(layout);
        ConfigureManualLaserSwitch(sceneName, layout);
        ConfigureEmitterPresentation(sceneName, layout);
        ExpandBoundaryWalls(layout);
        CreateFloorAndCeiling(layout);
        MoveSideObstacles(layout);
        CreateExpertBlockers(layout);
        ConfigureExit(sceneName, layout);
        CreateRefinedExitGate(layout);
        ConfigureExpertPickups(sceneName);
        ImproveLaserDistance();
        ConfigureRequiredMirrorReflections(layout);
    }

    private void ApplyCharacterModels()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            StableCharacterVisuals.ApplyPlayer(player);
        }

        foreach (GuardAI guard in Object.FindObjectsOfType<GuardAI>())
        {
            if (guard != null)
            {
                StableCharacterVisuals.ApplyGuard(guard.gameObject);
            }
        }
    }

    private void HideDecorativeClutter()
    {
        // The uploaded base scene already has the desired terrain style.
        // Remove visual clutter only: candles/torches, decorative stone columns, banners and supply crates.
        // Gameplay terrain, puzzle blockers, walls, doors, mirrors and pickups are preserved.
        string[] clutterKeywords =
        {
            "DungeonTheme_Props",
            "Torch_",
            "Candle",
            "StoneColumn",
            "ColumnCap",
            "CastleBanner",
            "WoodenSupplyCrate"
        };

        Transform[] transforms = Object.FindObjectsOfType<Transform>();
        foreach (Transform t in transforms)
        {
            if (t == null || t.gameObject == null)
            {
                continue;
            }

            string name = t.gameObject.name;
            foreach (string keyword in clutterKeywords)
            {
                if (name.Contains(keyword))
                {
                    t.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    private LevelLayout GetLayout(string sceneName)
    {
        if (sceneName == "MainScene")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L1",
                runtimePrefix = "L1",
                halfX = 12.5f,
                halfZ = 10.5f,
                player = new Vector3(-9.0f, 1f, -7.2f),
                light = new Vector3(-8.6f, 1.4f, -4.8f),
                mirrors = new Vector3[] { new Vector3(-2.2f, 1.25f, -4.8f) },
                receiver = new Vector3(6.6f, 1.25f, 2.2f),
                door = new Vector3(8.5f, 1.25f, 7.2f),
                exit = new Vector3(8.5f, 1f, 8.8f),
                mirrorYaws = new float[] { 142f },
                requiredMirrorReflections = 1
            };
        }

        if (sceneName == "Level02")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L2",
                runtimePrefix = "L2",
                halfX = 16f,
                halfZ = 14f,
                player = new Vector3(-10.5f, 1f, -8.0f),
                light = new Vector3(-10.0f, 1.4f, -5.8f),
                mirrors = new Vector3[]
                {
                    new Vector3(-4.0f, 1.25f, -5.8f),
                    new Vector3(-4.0f, 1.25f, 3.2f)
                },
                receiver = new Vector3(8.2f, 1.25f, 3.2f),
                door = new Vector3(9.8f, 1.25f, 8.2f),
                exit = new Vector3(9.8f, 1f, 9.8f),
                mirrorYaws = new float[] { 150f, 25f },
                requiredMirrorReflections = 2,
                blockerPositions = new Vector3[] { new Vector3(2.0f, 0.85f, -1.4f) },
                blockerScales = new Vector3[] { new Vector3(0.85f, 1.7f, 7.2f) }
            };
        }

        if (sceneName == "Level03")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L3",
                runtimePrefix = "L3",
                halfX = 21f,
                halfZ = 17f,
                player = new Vector3(-12.0f, 1f, -9.2f),
                light = new Vector3(-11.2f, 1.4f, -7.0f),
                mirrors = new Vector3[]
                {
                    new Vector3(-5.4f, 1.25f, -7.0f),
                    new Vector3(-5.4f, 1.25f, 1.2f),
                    new Vector3(4.8f, 1.25f, 1.2f)
                },
                receiver = new Vector3(4.8f, 1.25f, 7.2f),
                door = new Vector3(11.8f, 1.25f, 9.8f),
                exit = new Vector3(11.8f, 1f, 11.3f),
                mirrorYaws = new float[] { 150f, 15f, 126f },
                requiredMirrorReflections = 3,
                decoyMirrors = new Vector3[]
                {
                    new Vector3(1.0f, 1.25f, -6.2f)
                },
                decoyYaws = new float[] { 70f }
            };
        }

        if (sceneName == "Level04")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L4",
                halfX = 21f,
                halfZ = 17f,
                player = new Vector3(-14.0f, 1f, -11.0f),
                light = new Vector3(-13.0f, 1.4f, -9.2f),
                mirrors = new Vector3[]
                {
                    new Vector3(-7.0f, 1.25f, -9.2f),
                    new Vector3(-7.0f, 1.25f, -1.4f),
                    new Vector3(2.8f, 1.25f, -1.4f),
                    new Vector3(2.8f, 1.25f, 6.6f)
                },
                receiver = new Vector3(10.8f, 1.25f, 6.6f),
                door = new Vector3(14.0f, 1.25f, 11.5f),
                exit = new Vector3(14.0f, 1f, 13.0f),
                mirrorYaws = new float[] { 155f, 20f, 135f, -18f },
                requiredMirrorReflections = 4
            };
        }

        if (sceneName == "Level05")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L5",
                halfX = 27f,
                halfZ = 22f,
                player = new Vector3(-19.0f, 1f, -15.0f),
                light = new Vector3(-18.0f, 1.4f, -12.0f),
                mirrors = new Vector3[]
                {
                    new Vector3(-11.0f, 1.25f, -12.0f),
                    new Vector3(-11.0f, 1.25f, -3.0f),
                    new Vector3(-1.0f, 1.25f, -3.0f),
                    new Vector3(-1.0f, 1.25f, 8.0f),
                    new Vector3(11.0f, 1.25f, 8.0f)
                },
                receiver = new Vector3(18.0f, 1.25f, 8.0f),
                door = new Vector3(20.5f, 1.25f, 14.6f),
                exit = new Vector3(20.5f, 1f, 16.2f),
                mirrorYaws = new float[] { 145f, 20f, 138f, -15f, 125f },
                requiredMirrorReflections = 5,
                blockerPositions = new Vector3[]
                {
                    new Vector3(-18.5f, 0.9f, 4.0f),
                    new Vector3(4.5f, 0.9f, -14.0f),
                    new Vector3(16.0f, 0.9f, -3.0f),
                    new Vector3(15.5f, 0.9f, 14.0f)
                },
                blockerScales = new Vector3[]
                {
                    new Vector3(5.0f, 1.8f, 0.9f),
                    new Vector3(0.9f, 1.8f, 6.0f),
                    new Vector3(0.9f, 1.8f, 7.0f),
                    new Vector3(6.0f, 1.8f, 0.9f)
                },
                decoyMirrors = new Vector3[]
                {
                    new Vector3(-18.0f, 1.25f, -2.0f),
                    new Vector3(3.5f, 1.25f, -12.5f),
                    new Vector3(14.5f, 1.25f, 0.5f),
                    new Vector3(8.0f, 1.25f, 15.0f)
                },
                decoyYaws = new float[] { 62f, -38f, 108f, -58f }
            };
        }

        if (sceneName == "Level06")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L6",
                halfX = 31f,
                halfZ = 25f,
                player = new Vector3(-22.0f, 1f, -18.0f),
                light = new Vector3(-21.0f, 1.4f, -15.0f),
                mirrors = new Vector3[]
                {
                    new Vector3(-14.0f, 1.25f, -15.0f),
                    new Vector3(-14.0f, 1.25f, -5.0f),
                    new Vector3(-3.0f, 1.25f, -5.0f),
                    new Vector3(-3.0f, 1.25f, 6.0f),
                    new Vector3(9.0f, 1.25f, 6.0f),
                    new Vector3(9.0f, 1.25f, 16.0f)
                },
                receiver = new Vector3(21.0f, 1.25f, 16.0f),
                door = new Vector3(23.5f, 1.25f, 20.5f),
                exit = new Vector3(23.5f, 1f, 22.2f),
                mirrorYaws = new float[] { 148f, 15f, 135f, -12f, 135f, -20f },
                requiredMirrorReflections = 6,
                blockerPositions = new Vector3[]
                {
                    new Vector3(-21.0f, 0.9f, 0.0f),
                    new Vector3(2.0f, 0.9f, -16.0f),
                    new Vector3(14.0f, 0.9f, -2.0f),
                    new Vector3(-9.0f, 0.9f, 14.0f),
                    new Vector3(18.0f, 0.9f, 4.0f),
                    new Vector3(18.0f, 0.9f, 20.0f)
                },
                blockerScales = new Vector3[]
                {
                    new Vector3(6.0f, 1.8f, 0.9f),
                    new Vector3(0.9f, 1.8f, 7.0f),
                    new Vector3(0.9f, 1.8f, 8.0f),
                    new Vector3(7.0f, 1.8f, 0.9f),
                    new Vector3(0.9f, 1.8f, 7.0f),
                    new Vector3(7.0f, 1.8f, 0.9f)
                },
                decoyMirrors = new Vector3[]
                {
                    new Vector3(-19.0f, 1.25f, -5.0f),
                    new Vector3(-4.0f, 1.25f, -15.0f),
                    new Vector3(12.0f, 1.25f, -5.0f),
                    new Vector3(16.0f, 1.25f, 7.0f),
                    new Vector3(3.0f, 1.25f, 18.0f)
                },
                decoyYaws = new float[] { 70f, -35f, 105f, -70f, 42f }
            };
        }

        return new LevelLayout();
    }

    private void CacheMaterials(string existingPrefix)
    {
        floorMaterial = null;
        wallMaterial = null;
        coverMaterial = null;

        GameObject floor = GameObject.Find(existingPrefix + "_FloorTile_0_0");
        if (floor != null && floor.GetComponent<Renderer>() != null)
        {
            floorMaterial = floor.GetComponent<Renderer>().sharedMaterial;
        }

        GameObject wall = GameObject.Find(existingPrefix + "_BackWall");
        if (wall != null && wall.GetComponent<Renderer>() != null)
        {
            wallMaterial = wall.GetComponent<Renderer>().sharedMaterial;
        }

        GameObject cover = GameObject.Find(existingPrefix + "_Cover_1");
        if (cover != null && cover.GetComponent<Renderer>() != null)
        {
            coverMaterial = cover.GetComponent<Renderer>().sharedMaterial;
        }
    }

    private void EnsureMirrorCount(int count)
    {
        GameObject template = GameObject.Find("Mirror_04") ?? GameObject.Find("Mirror_03") ?? GameObject.Find("Mirror_02") ?? GameObject.Find("Mirror_01");
        if (template == null)
        {
            return;
        }

        for (int i = 1; i <= count; i++)
        {
            string name = "Mirror_" + i.ToString("00");
            if (GameObject.Find(name) == null)
            {
                GameObject clone = Instantiate(template);
                clone.name = name;
                clone.tag = "Mirror";
                if (clone.GetComponent<MirrorController>() == null)
                {
                    clone.AddComponent<MirrorController>();
                }
            }
        }
    }

    private void RepositionGameplayObjects(LevelLayout layout)
    {
        SetPosition("Player", layout.player);
        SetPosition("LightSource", layout.light);
        LookAtHorizontal("LightSource", layout.mirrors.Length > 0 ? layout.mirrors[0] : layout.receiver);

        Vector3 previousMirrorPathPoint = layout.light;
        for (int i = 0; i < layout.mirrors.Length; i++)
        {
            string name = "Mirror_" + (i + 1).ToString("00");
            SetPosition(name, layout.mirrors[i]);

            Vector3 nextMirrorPathPoint = (i == layout.mirrors.Length - 1) ? layout.receiver : layout.mirrors[i + 1];
            float solvedYaw = CalculateMirrorYaw(previousMirrorPathPoint, layout.mirrors[i], nextMirrorPathPoint);
            float scrambledStartYaw = GetScrambledStartYaw(solvedYaw, i);
            SetYaw(name, scrambledStartYaw);
            previousMirrorPathPoint = layout.mirrors[i];
        }

        // Move unused mirrors far outside the level if the copied scene had more than this level needs.
        for (int i = layout.mirrors.Length + 1; i <= 6; i++)
        {
            string name = "Mirror_" + i.ToString("00");
            GameObject unused = GameObject.Find(name);
            if (unused != null)
            {
                unused.transform.position = new Vector3(500f + i, -50f, 500f + i);
            }
        }

        SetPosition("Receiver_01", layout.receiver);
        SetPosition("SecurityDoor", layout.door);
        SetPosition("ExitZone", layout.exit);

        SetPosition("Guard_01", new Vector3(2f, 1f, layout.halfZ - 4f));
        SetPosition("Guard_01_PatrolPoint_A", new Vector3(-layout.halfX + 4f, 0.1f, layout.halfZ - 4f));
        SetPosition("Guard_01_PatrolPoint_B", new Vector3(layout.halfX - 5f, 0.1f, layout.halfZ - 4f));
        SetPosition("Guard_01_PatrolPoint_C", new Vector3(layout.halfX - 5f, 0.1f, -layout.halfZ + 4f));
        SetPosition("Guard_01_PatrolPoint_D", new Vector3(-layout.halfX + 4f, 0.1f, -layout.halfZ + 4f));

        if (SceneManager.GetActiveScene().name == "Level06")
        {
            EnsureSecondGuard(layout);
        }
    }

    private void EnsureSecondGuard(LevelLayout layout)
    {
        GameObject guard2 = GameObject.Find("Guard_02");
        if (guard2 == null)
        {
            GameObject source = GameObject.Find("Guard_01");
            if (source != null)
            {
                guard2 = Instantiate(source);
                guard2.name = "Guard_02";
            }
        }

        if (guard2 != null)
        {
            guard2.transform.position = new Vector3(-2f, 1f, 0f);
            StableCharacterVisuals.ApplyGuard(guard2);
        }
    }

    private void ConfigureAdvancedGuards(string sceneName, LevelLayout layout)
    {
        if (sceneName == "Level05")
        {
            ConfigureGuardRoute("Guard_01", new Vector3(0f, 1f, layout.halfZ - 4f), new Vector3[]
            {
                new Vector3(-layout.halfX + 5f, 0.1f, layout.halfZ - 4f),
                new Vector3(layout.halfX - 6f, 0.1f, layout.halfZ - 4f),
                new Vector3(layout.halfX - 6f, 0.1f, 2f),
                new Vector3(-layout.halfX + 5f, 0.1f, 2f)
            });

            ConfigureGuardRoute("Guard_02", new Vector3(9f, 1f, -2f), new Vector3[]
            {
                new Vector3(5f, 0.1f, -10f),
                new Vector3(16f, 0.1f, -10f),
                new Vector3(16f, 0.1f, 3f),
                new Vector3(5f, 0.1f, 3f)
            });
        }
        else if (sceneName == "Level06")
        {
            ConfigureGuardRoute("Guard_01", new Vector3(-2f, 1f, layout.halfZ - 5f), new Vector3[]
            {
                new Vector3(-layout.halfX + 5f, 0.1f, layout.halfZ - 5f),
                new Vector3(layout.halfX - 6f, 0.1f, layout.halfZ - 5f),
                new Vector3(layout.halfX - 6f, 0.1f, 4f),
                new Vector3(-layout.halfX + 5f, 0.1f, 4f)
            });

            ConfigureGuardRoute("Guard_02", new Vector3(12f, 1f, -4f), new Vector3[]
            {
                new Vector3(6f, 0.1f, -12f),
                new Vector3(18f, 0.1f, -12f),
                new Vector3(18f, 0.1f, 1f),
                new Vector3(6f, 0.1f, 1f)
            });

            ConfigureGuardRoute("Guard_03", new Vector3(-12f, 1f, 12f), new Vector3[]
            {
                new Vector3(-18f, 0.1f, 10f),
                new Vector3(-4f, 0.1f, 10f),
                new Vector3(-4f, 0.1f, 19f),
                new Vector3(-18f, 0.1f, 19f)
            });
        }
        else
        {
            GameObject extra2 = GameObject.Find("Guard_02");
            if (extra2 != null) extra2.SetActive(false);
            GameObject extra3 = GameObject.Find("Guard_03");
            if (extra3 != null) extra3.SetActive(false);
        }
    }

    private void ConfigureGuardRoute(string guardName, Vector3 guardPosition, Vector3[] patrolPositions)
    {
        GameObject source = GameObject.Find("Guard_01");
        if (source == null)
        {
            return;
        }

        GameObject guard = GameObject.Find(guardName);
        if (guard == null)
        {
            guard = Instantiate(source);
            guard.name = guardName;
        }

        guard.SetActive(true);
        guard.transform.position = guardPosition;
        StableCharacterVisuals.ApplyGuard(guard);

        GuardAI ai = guard.GetComponent<GuardAI>();
        if (ai == null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            ai.player = playerObject.transform;
        }

        Transform[] patrols = new Transform[patrolPositions.Length];
        for (int i = 0; i < patrolPositions.Length; i++)
        {
            string patrolName = guardName + "_PatrolPoint_" + (char)('A' + i);
            GameObject patrol = GameObject.Find(patrolName);
            if (patrol == null)
            {
                patrol = new GameObject(patrolName);
            }
            patrol.transform.position = patrolPositions[i];
            patrols[i] = patrol.transform;
        }
        ai.patrolPoints = patrols;
        ai.showVisionCone = true;
        ai.viewDistance = Mathf.Max(ai.viewDistance, 9f);
    }

    private void ExpandBoundaryWalls(LevelLayout layout)
    {
        float wallHeight = 4.2f;
        float wallY = wallHeight / 2f - 0.05f;

        SetTransform(layout.existingPrefix + "_BackWall", new Vector3(0f, wallY, layout.halfZ), new Vector3(layout.halfX * 2f, wallHeight, 0.35f));
        SetTransform(layout.existingPrefix + "_FrontWall", new Vector3(0f, wallY, -layout.halfZ), new Vector3(layout.halfX * 2f, wallHeight, 0.35f));
        SetTransform(layout.existingPrefix + "_LeftWall", new Vector3(-layout.halfX, wallY, 0f), new Vector3(0.35f, wallHeight, layout.halfZ * 2f));
        SetTransform(layout.existingPrefix + "_RightWall", new Vector3(layout.halfX, wallY, 0f), new Vector3(0.35f, wallHeight, layout.halfZ * 2f));
    }

    private void CreateFloorAndCeiling(LevelLayout layout)
    {
        CreateOrUpdateCube(layout.runtimePrefix + "_ExpandedFloor", new Vector3(0f, -0.12f, 0f), new Vector3(layout.halfX * 2f, 0.18f, layout.halfZ * 2f), floorMaterial);
        CreateOrUpdateCube(layout.runtimePrefix + "_SealedCeiling", new Vector3(0f, 4.25f, 0f), new Vector3(layout.halfX * 2f, 0.30f, layout.halfZ * 2f), wallMaterial);

        for (int i = -2; i <= 2; i++)
        {
            CreateOrUpdateCube(layout.runtimePrefix + "_CeilingBeam_Z_" + i, new Vector3(0f, 4.05f, i * layout.halfZ / 3f), new Vector3(layout.halfX * 2f, 0.18f, 0.18f), wallMaterial);
            CreateOrUpdateCube(layout.runtimePrefix + "_CeilingBeam_X_" + i, new Vector3(i * layout.halfX / 3f, 4.06f, 0f), new Vector3(0.18f, 0.18f, layout.halfZ * 2f), wallMaterial);
        }
    }

    private void MoveSideObstacles(LevelLayout layout)
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-layout.halfX + 3.5f, 0.75f, layout.halfZ - 4f),
            new Vector3(layout.halfX - 4.5f, 0.75f, -layout.halfZ + 4f),
            new Vector3(-layout.halfX + 4f, 0.55f, -layout.halfZ + 4f),
            new Vector3(layout.halfX - 5f, 0.55f, layout.halfZ - 5f),
            new Vector3(0f, 0.55f, layout.halfZ - 3f),
            new Vector3(layout.halfX - 5f, 0.55f, 0f),
            new Vector3(-layout.halfX + 5f, 0.55f, 0f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            string name = layout.existingPrefix + "_Cover_" + (i + 1);
            GameObject go = GameObject.Find(name);
            if (go == null)
            {
                continue;
            }

            go.transform.position = positions[i];
            go.transform.localScale = i < 2 ? new Vector3(2.2f, 1.5f, 0.7f) : new Vector3(1.2f, 1.1f, 1.2f);
        }
    }

    private void CreateExpertBlockers(LevelLayout layout)
    {
        if (layout.blockerPositions == null || layout.blockerScales == null)
        {
            return;
        }

        for (int i = 0; i < layout.blockerPositions.Length && i < layout.blockerScales.Length; i++)
        {
            GameObject blocker = CreateOrUpdateCube(layout.runtimePrefix + "_LaserCover_" + i, layout.blockerPositions[i], layout.blockerScales[i], coverMaterial != null ? coverMaterial : wallMaterial);
            if (blocker != null)
            {
                blocker.name = layout.runtimePrefix + "_LaserCover_" + i;
            }
        }
    }


    private void ConfigureMirrorRails(LevelLayout layout)
    {
        if (layout.mirrors == null)
        {
            return;
        }

        for (int i = 0; i < layout.mirrors.Length; i++)
        {
            string mirrorName = "Mirror_" + (i + 1).ToString("00");
            GameObject mirror = GameObject.Find(mirrorName);
            if (mirror == null)
            {
                continue;
            }

            MovableMirrorRail rail = mirror.GetComponent<MovableMirrorRail>();
            if (rail == null)
            {
                rail = mirror.AddComponent<MovableMirrorRail>();
            }

            Vector3 axis = GetRailAxis(i);
            float range = Mathf.Clamp(4.2f + i * 0.35f, 4.2f, 5.8f);
            float initialOffset = GetMirrorStartOffset(i, range);

            rail.railDirection = axis;
            rail.correctPosition = layout.mirrors[i];
            rail.railRange = range;
            rail.slideSpeed = 2.4f;
            rail.interactDistance = 5.5f;
            rail.startAwayFromCorrectPosition = true;
            rail.initialStartOffset = initialOffset;

            // Put the mirror at the start offset immediately. This makes sure the level
            // never begins solved, even before MovableMirrorRail.Start() runs.
            mirror.transform.position = layout.mirrors[i] + axis.normalized * initialOffset;
            rail.ResetMirrorToStartOffset();
        }
    }

    private float GetMirrorStartOffset(int mirrorIndex, float range)
    {
        // Alternate ends of the rail and use a large offset so the beam cannot
        // accidentally hit a 2-meter-wide mirror at the solution point.
        float sign = (mirrorIndex % 2 == 0) ? -1f : 1f;
        return sign * range * 0.90f;
    }

    private Vector3 GetRailAxis(int mirrorIndex)
    {
        switch (mirrorIndex % 4)
        {
            case 0: return Vector3.right;
            case 1: return Vector3.forward;
            case 2: return Vector3.right;
            default: return Vector3.forward;
        }
    }

    private void CreateDecoyMirrors(LevelLayout layout)
    {
        if (layout.decoyMirrors == null || layout.decoyMirrors.Length == 0)
        {
            // Hide old decoys when changing to simpler levels.
            for (int i = 1; i <= 6; i++)
            {
                GameObject old = GameObject.Find("DecoyMirror_" + i.ToString("00"));
                if (old != null)
                {
                    old.SetActive(false);
                }
            }
            return;
        }

        GameObject template = GameObject.Find("Mirror_01");
        if (template == null)
        {
            return;
        }

        for (int i = 0; i < layout.decoyMirrors.Length; i++)
        {
            string name = "DecoyMirror_" + (i + 1).ToString("00");
            GameObject decoy = GameObject.Find(name);

            if (decoy == null)
            {
                decoy = Instantiate(template);
                decoy.name = name;
            }

            decoy.SetActive(true);
            decoy.tag = "Mirror";
            decoy.transform.position = layout.decoyMirrors[i];

            float yaw = layout.decoyYaws != null && i < layout.decoyYaws.Length ? layout.decoyYaws[i] : 65f + i * 37f;
            decoy.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            MirrorController controller = decoy.GetComponent<MirrorController>();
            if (controller == null)
            {
                controller = decoy.AddComponent<MirrorController>();
            }

            MovableMirrorRail rail = decoy.GetComponent<MovableMirrorRail>();
            if (rail == null)
            {
                rail = decoy.AddComponent<MovableMirrorRail>();
            }

            Vector3 axis = (i % 2 == 0) ? Vector3.forward : Vector3.right;
            float range = 3.8f;
            rail.railDirection = axis;
            rail.correctPosition = layout.decoyMirrors[i];
            rail.railRange = range;
            rail.slideSpeed = 2.4f;
            rail.interactDistance = 5.5f;
            rail.startAwayFromCorrectPosition = true;
            rail.initialStartOffset = ((i % 2 == 0) ? 1f : -1f) * range * 0.75f;

            // Decoys also start offset, so they look equally plausible.
            decoy.transform.position = layout.decoyMirrors[i] + axis.normalized * rail.initialStartOffset;
            rail.ResetMirrorToStartOffset();
        }

        for (int i = layout.decoyMirrors.Length + 1; i <= 6; i++)
        {
            GameObject old = GameObject.Find("DecoyMirror_" + i.ToString("00"));
            if (old != null)
            {
                old.SetActive(false);
            }
        }
    }

    private void ConfigureManualLaserSwitch(string sceneName, LevelLayout layout)
    {
        LightReflection laser = Object.FindObjectOfType<LightReflection>();
        if (laser == null)
        {
            return;
        }

        GameObject lightObject = GameObject.Find("LightSource");
        Light sourceLight = lightObject != null ? lightObject.GetComponent<Light>() : null;

        bool requiresManualSwitch = sceneName != "MainScene";
        if (!requiresManualSwitch)
        {
            laser.SetLaserEnabled(true);
            if (sourceLight != null) sourceLight.enabled = true;
            return;
        }

        GameObject switchObject = GameObject.Find("LaserSwitch");
        if (switchObject == null)
        {
            switchObject = new GameObject("LaserSwitch");
        }

        switchObject.transform.position = GetSafeSwitchPosition(layout);
        switchObject.transform.rotation = Quaternion.LookRotation((layout.light - switchObject.transform.position).normalized, Vector3.up);
        switchObject.transform.localScale = Vector3.one;

        SphereCollider trigger = switchObject.GetComponent<SphereCollider>();
        if (trigger == null)
        {
            trigger = switchObject.AddComponent<SphereCollider>();
        }
        trigger.isTrigger = true;
        trigger.radius = 0.8f;

        BuildSwitchVisual(switchObject);

        LaserSwitch laserSwitch = switchObject.GetComponent<LaserSwitch>();
        if (laserSwitch == null)
        {
            laserSwitch = switchObject.AddComponent<LaserSwitch>();
        }

        laserSwitch.targetLaser = laser;
        laserSwitch.sourceLight = sourceLight;
        laserSwitch.isOn = false;
        laserSwitch.interactDistance = 3.2f;
        laserSwitch.interactKey = KeyCode.X;

        laser.SetLaserEnabled(false);
        if (sourceLight != null)
        {
            sourceLight.enabled = false;
        }
    }

    private Vector3 GetSafeSwitchPosition(LevelLayout layout)
    {
        Vector3 player = layout.player;
        Vector3 light = layout.light;
        Vector3 firstMirror = layout.mirrors != null && layout.mirrors.Length > 0 ? layout.mirrors[0] : light;

        // Put the switch on the side opposite the lamp and first mirror.
        Vector3 dangerDirection = (light - player) + (firstMirror - player) * 0.35f;
        dangerDirection.y = 0f;
        if (dangerDirection.sqrMagnitude < 0.001f)
        {
            dangerDirection = Vector3.forward;
        }
        dangerDirection.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, dangerDirection).normalized;
        Vector3 candidate = player - dangerDirection * 1.8f + side * 1.6f;
        candidate.y = 1.0f;

        return candidate;
    }

    private void ConfigureEmitterPresentation(string sceneName, LevelLayout layout)
    {
        GameObject lightObject = GameObject.Find("LightSource");
        if (lightObject == null)
        {
            return;
        }

        Light sourceLight = lightObject.GetComponent<Light>();

        // Keep Level 1 simple and visible; from Level 2 onward, hide the raw source object
        // and present it as a lamp fixture instead.
        Renderer lightRenderer = lightObject.GetComponent<Renderer>();
        if (lightRenderer != null)
        {
            lightRenderer.enabled = sceneName == "MainScene";
        }

        BuildLampVisual(lightObject, sceneName == "MainScene");

        if (sourceLight != null)
        {
            sourceLight.range = Mathf.Max(sourceLight.range, 10f);
            sourceLight.intensity = Mathf.Max(sourceLight.intensity, 2.2f);
        }
    }

    private void BuildLampVisual(GameObject lightObject, bool showRawLightSource)
    {
        Transform lampRoot = lightObject.transform.Find("EmitterLamp");
        if (lampRoot == null)
        {
            GameObject root = new GameObject("EmitterLamp");
            lampRoot = root.transform;
            lampRoot.SetParent(lightObject.transform, false);
        }

        lampRoot.localPosition = Vector3.zero;
        lampRoot.localRotation = Quaternion.identity;
        lampRoot.gameObject.SetActive(true);

        GameObject pedestal = FindOrCreatePrimitiveChild(lampRoot, "LampPedestal", PrimitiveType.Cylinder);
        pedestal.transform.localPosition = new Vector3(0f, -1.05f, 0f);
        pedestal.transform.localScale = new Vector3(0.34f, 0.12f, 0.34f);
        SetColor(pedestal, new Color(0.16f, 0.16f, 0.16f));

        GameObject stem = FindOrCreatePrimitiveChild(lampRoot, "LampStem", PrimitiveType.Cylinder);
        stem.transform.localPosition = new Vector3(0f, -0.42f, 0f);
        stem.transform.localScale = new Vector3(0.06f, 0.38f, 0.06f);
        SetColor(stem, new Color(0.10f, 0.10f, 0.10f));

        GameObject topCap = FindOrCreatePrimitiveChild(lampRoot, "LampTopCap", PrimitiveType.Cylinder);
        topCap.transform.localPosition = new Vector3(0f, 0.22f, 0f);
        topCap.transform.localScale = new Vector3(0.25f, 0.04f, 0.25f);
        SetColor(topCap, new Color(0.22f, 0.18f, 0.12f));

        GameObject frameLeft = FindOrCreatePrimitiveChild(lampRoot, "LampFrameLeft", PrimitiveType.Cube);
        frameLeft.transform.localPosition = new Vector3(-0.13f, -0.03f, 0f);
        frameLeft.transform.localScale = new Vector3(0.03f, 0.26f, 0.03f);
        SetColor(frameLeft, new Color(0.14f, 0.14f, 0.14f));

        GameObject frameRight = FindOrCreatePrimitiveChild(lampRoot, "LampFrameRight", PrimitiveType.Cube);
        frameRight.transform.localPosition = new Vector3(0.13f, -0.03f, 0f);
        frameRight.transform.localScale = new Vector3(0.03f, 0.26f, 0.03f);
        SetColor(frameRight, new Color(0.14f, 0.14f, 0.14f));

        GameObject frameFront = FindOrCreatePrimitiveChild(lampRoot, "LampFrameFront", PrimitiveType.Cube);
        frameFront.transform.localPosition = new Vector3(0f, -0.03f, 0.13f);
        frameFront.transform.localScale = new Vector3(0.24f, 0.26f, 0.03f);
        SetColor(frameFront, new Color(0.14f, 0.14f, 0.14f));

        GameObject frameBack = FindOrCreatePrimitiveChild(lampRoot, "LampFrameBack", PrimitiveType.Cube);
        frameBack.transform.localPosition = new Vector3(0f, -0.03f, -0.13f);
        frameBack.transform.localScale = new Vector3(0.24f, 0.26f, 0.03f);
        SetColor(frameBack, new Color(0.14f, 0.14f, 0.14f));

        GameObject bulb = FindOrCreatePrimitiveChild(lampRoot, "LampBulb", PrimitiveType.Sphere);
        bulb.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        bulb.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
        SetColor(bulb, new Color(1.0f, 0.82f, 0.20f));

        foreach (Renderer r in lampRoot.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    private void BuildSwitchVisual(GameObject switchObject)
    {
        GameObject stand = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchStand", PrimitiveType.Cylinder);
        stand.transform.localPosition = new Vector3(0f, 0.40f, 0f);
        stand.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
        SetColor(stand, new Color(0.10f, 0.10f, 0.10f));

        GameObject basePart = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchBase", PrimitiveType.Cube);
        basePart.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        basePart.transform.localScale = new Vector3(0.54f, 0.10f, 0.54f);
        SetColor(basePart, new Color(0.18f, 0.18f, 0.18f));

        GameObject housing = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchHousing", PrimitiveType.Cube);
        housing.transform.localPosition = new Vector3(0f, 0.70f, 0f);
        housing.transform.localScale = new Vector3(0.40f, 0.32f, 0.24f);
        SetColor(housing, new Color(0.20f, 0.20f, 0.20f));

        GameObject panelPart = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchPanel", PrimitiveType.Cube);
        panelPart.transform.localPosition = new Vector3(0f, 0.70f, 0.13f);
        panelPart.transform.localScale = new Vector3(0.30f, 0.22f, 0.04f);
        SetColor(panelPart, new Color(0.32f, 0.32f, 0.32f));

        GameObject leverPart = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchLever", PrimitiveType.Cylinder);
        leverPart.transform.localPosition = new Vector3(0f, 0.70f, 0.05f);
        leverPart.transform.localScale = new Vector3(0.04f, 0.18f, 0.04f);
        leverPart.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        SetColor(leverPart, new Color(0.68f, 0.12f, 0.12f));

        GameObject handlePart = FindOrCreatePrimitiveChild(leverPart.transform, "SwitchHandle", PrimitiveType.Sphere);
        handlePart.transform.localPosition = new Vector3(0f, 0.22f, 0f);
        handlePart.transform.localScale = new Vector3(0.26f, 0.26f, 0.26f);
        SetColor(handlePart, new Color(0.88f, 0.18f, 0.18f));

        GameObject indicatorPart = FindOrCreatePrimitiveChild(switchObject.transform, "SwitchIndicator", PrimitiveType.Sphere);
        indicatorPart.transform.localPosition = new Vector3(0.10f, 0.78f, 0.13f);
        indicatorPart.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
        SetColor(indicatorPart, new Color(0.8f, 0.15f, 0.15f));
    }

    private GameObject FindOrCreatePrimitiveChild(Transform parent, string childName, PrimitiveType primitive)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = childName;
        part.transform.SetParent(parent, false);
        Collider c = part.GetComponent<Collider>();
        if (c != null)
        {
            Object.Destroy(c);
        }
        return part;
    }

    private void SetColor(GameObject obj, Color color)
    {
        if (obj == null)
        {
            return;
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    private void ConfigureExit(string sceneName, LevelLayout layout)
    {
        ExitZone exit = Object.FindObjectOfType<ExitZone>();
        if (exit == null)
        {
            return;
        }

        exit.allowExitWithoutDoor = false;
        exit.requiredDoor = Object.FindObjectOfType<DoorAnimator>();
        exit.requiredKeycards = 0;
        exit.requiredEnergyCores = 0;
        exit.requiresEmpDevice = false;
        exit.isFinalLevel = false;

        if (sceneName == "MainScene") exit.nextSceneName = "Level02";
        else if (sceneName == "Level02") exit.nextSceneName = "Level03";
        else if (sceneName == "Level03") exit.nextSceneName = "Level04";
        else if (sceneName == "Level04") exit.nextSceneName = "Level05";
        else if (sceneName == "Level05")
        {
            exit.nextSceneName = "Level06";
            exit.requiredKeycards = 1;
            exit.requiredEnergyCores = 2;
            exit.requiresEmpDevice = true;
        }
        else if (sceneName == "Level06")
        {
            exit.isFinalLevel = true;
            exit.nextSceneName = "";
            exit.requiredKeycards = 1;
            exit.requiredEnergyCores = 3;
            exit.requiresEmpDevice = true;
            exit.finalLevelMessage = "All six dungeon trials complete. Press R to replay the final level.";
        }
    }

    private void CreateRefinedExitGate(LevelLayout layout)
    {
        string prefix = layout.runtimePrefix;
        Vector3 doorCenter = layout.door;
        Vector3 rawExitCenter = layout.exit;

        HideLegacyDoorFramePieces(prefix);

        // Snap the exit direction to the dominant world axis so the door and exit pad
        // stay perfectly aligned instead of drifting diagonally.
        Vector3 delta = rawExitCenter - doorCenter;
        delta.y = 0f;
        Vector3 forward;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            forward = new Vector3(Mathf.Sign(delta.x), 0f, 0f);
        }
        else
        {
            forward = new Vector3(0f, 0f, Mathf.Sign(delta.z == 0f ? 1f : delta.z));
        }

        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Quaternion frameRotation = Quaternion.LookRotation(forward, Vector3.up);

        // Put the exit directly behind the door on the same centerline.
        Vector3 exitCenter = doorCenter + forward * 2.4f;

        // Clear a wider zone around the doorway so no stray box/cover remains in front of it.
        ClearExitArea(doorCenter, exitCenter, 5.2f);

        float openingWidth = 2.95f;
        float postHeight = 3.35f;
        float postThickness = 0.36f;
        float gateDepth = 0.42f;

        GameObject securityDoor = GameObject.Find("SecurityDoor");
        if (securityDoor != null)
        {
            securityDoor.transform.position = doorCenter + Vector3.up * 0.50f;
            securityDoor.transform.rotation = frameRotation;
            securityDoor.transform.localScale = new Vector3(2.55f, 3.15f, 0.18f);

            Renderer doorRenderer = securityDoor.GetComponent<Renderer>();
            if (doorRenderer != null)
            {
                if (coverMaterial != null)
                {
                    doorRenderer.material = coverMaterial;
                }
                doorRenderer.material.color = new Color(0.34f, 0.24f, 0.16f);
            }

            DoorAnimator animator = securityDoor.GetComponent<DoorAnimator>();
            if (animator != null)
            {
                animator.openOffset = new Vector3(0f, 3.8f, 0f);
                animator.SnapClosedAtCurrentPosition();
            }
        }

        GameObject leftPost = CreateOrUpdateCube(prefix + "_CleanDoor_LeftPost",
            doorCenter - right * (openingWidth * 0.5f + postThickness * 0.5f) + Vector3.up * 1.22f,
            new Vector3(postThickness, postHeight, gateDepth), wallMaterial);

        GameObject rightPost = CreateOrUpdateCube(prefix + "_CleanDoor_RightPost",
            doorCenter + right * (openingWidth * 0.5f + postThickness * 0.5f) + Vector3.up * 1.22f,
            new Vector3(postThickness, postHeight, gateDepth), wallMaterial);

        GameObject topBeam = CreateOrUpdateCube(prefix + "_CleanDoor_TopBeam",
            doorCenter + Vector3.up * 2.93f,
            new Vector3(openingWidth + postThickness * 2f, postThickness, gateDepth), wallMaterial);

        foreach (GameObject g in new GameObject[] { leftPost, rightPost, topBeam })
        {
            if (g != null)
            {
                g.transform.rotation = frameRotation;
            }
        }


        GameObject exitZoneObject = GameObject.Find("ExitZone");
        if (exitZoneObject != null)
        {
            exitZoneObject.transform.position = exitCenter;
            exitZoneObject.transform.rotation = frameRotation;
            exitZoneObject.transform.localScale = new Vector3(2.55f, exitZoneObject.transform.localScale.y, 2.05f);
            foreach (Renderer exitZoneRenderer in exitZoneObject.GetComponentsInChildren<Renderer>())
            {
                if (exitZoneRenderer != null)
                {
                    exitZoneRenderer.enabled = false;
                }
            }
        }
    }

    private void ClearExitArea(Vector3 doorCenter, Vector3 exitCenter, float radius)
    {
        Vector3 center = (doorCenter + exitCenter) * 0.5f;

        foreach (Transform t in Object.FindObjectsOfType<Transform>())
        {
            if (t == null || t.gameObject == null)
            {
                continue;
            }

            string n = t.gameObject.name;
            if (n == "SecurityDoor" || n == "ExitZone" || n.Contains("_CleanDoor_"))
            {
                continue;
            }

            if (Vector3.Distance(t.position, center) > radius)
            {
                continue;
            }

            bool removeNearExit =
                n.Contains("Cover") ||
                n.Contains("LaserCover") ||
                n.Contains("Pickup") ||
                n.Contains("Keycard") ||
                n.Contains("Core") ||
                n.Contains("EMP") ||
                n.Contains("Crate") ||
                n.Contains("Box");

            if (removeNearExit)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    private void HideLegacyDoorFramePieces(string prefix)
    {
        foreach (Transform t in Object.FindObjectsOfType<Transform>())
        {
            if (t == null || t.gameObject == null)
            {
                continue;
            }

            string n = t.gameObject.name;

            if (n == "SecurityDoor" || n == "ExitZone")
            {
                continue;
            }

            bool isOldDoorPiece =
                n.Contains("DoorFrame") ||
                n.Contains("_Gate_") ||
                n.Contains("_Exit_LeftFrame") ||
                n.Contains("_Exit_RightFrame") ||
                n.Contains("_Exit_TopFrame") ||
                n.Contains("_ExitPad") ||
                n.Contains("ExitFloor") ||
                n.Contains("ExitMarker") ||
                n.Contains("GoalPad") ||
                (n.Contains("_Gate") && !n.Contains("_CleanDoor_"));

            if (isOldDoorPiece)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    private float GetScrambledStartYaw(float solvedYaw, int mirrorIndex)
    {
        // Mirrors should begin clearly unsolved, while remaining plausible.
        // Use deterministic offsets so every level always opens in a puzzle state.
        float[] offsets = { 37f, -44f, 58f, -31f, 49f, -53f, 41f, -47f };
        float offset = offsets[mirrorIndex % offsets.Length];
        float scrambled = solvedYaw + offset;

        // Avoid near-solved angles after wrapping.
        float delta = Mathf.DeltaAngle(scrambled, solvedYaw);
        if (Mathf.Abs(delta) < 20f)
        {
            scrambled += (delta >= 0f ? 25f : -25f);
        }

        return scrambled;
    }

    private void ConfigureExpertPickups(string sceneName)
    {
        if (sceneName != "Level05" && sceneName != "Level06")
        {
            return;
        }

        foreach (PickupItem p in Object.FindObjectsOfType<PickupItem>())
        {
            if (p != null)
            {
                Destroy(p.gameObject);
            }
        }

        if (sceneName == "Level05")
        {
            CreatePickup("L5_EMP_Device", PickupType.EmpDevice, new Vector3(-20f, 1.2f, -4f), "EMP Device");
            CreatePickup("L5_Keycard", PickupType.SecurityKeycard, new Vector3(-19f, 1.2f, 14.5f), "Crypt Keycard");
            CreatePickup("L5_Core_A", PickupType.EnergyCore, new Vector3(6f, 1.2f, -15.0f), "Energy Core A");
            CreatePickup("L5_Core_B", PickupType.EnergyCore, new Vector3(17f, 1.2f, -2.0f), "Energy Core B");
        }
        else if (sceneName == "Level06")
        {
            CreatePickup("L6_EMP_Device", PickupType.EmpDevice, new Vector3(-21f, 1.2f, -12f), "Advanced EMP Device");
            CreatePickup("L6_Master_Keycard", PickupType.SecurityKeycard, new Vector3(-21f, 1.2f, 16.5f), "Master Keycard");
            CreatePickup("L6_Core_A", PickupType.EnergyCore, new Vector3(-4f, 1.2f, -18f), "Energy Core A");
            CreatePickup("L6_Core_B", PickupType.EnergyCore, new Vector3(13f, 1.2f, -7f), "Energy Core B");
            CreatePickup("L6_Core_C", PickupType.EnergyCore, new Vector3(19f, 1.2f, 6f), "Energy Core C");
            CreatePickup("L6_EMP_Charge", PickupType.EmpCharge, new Vector3(2f, 1.2f, 19f), "EMP Charge");
        }
    }

    private void CreatePickup(string name, PickupType type, Vector3 position, string displayName)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.65f;
        Collider col = go.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
        PickupItem item = go.AddComponent<PickupItem>();
        item.pickupType = type;
        item.displayName = displayName;
        item.amount = 1;
    }

    private void ImproveLaserDistance()
    {
        LightReflection lr = Object.FindObjectOfType<LightReflection>();
        if (lr != null)
        {
            lr.maxDistance = Mathf.Max(lr.maxDistance, 140f);
            lr.receiverActivationRadius = Mathf.Max(lr.receiverActivationRadius, 1.85f);
        }
    }

    private void ConfigureRequiredMirrorReflections(LevelLayout layout)
    {
        LightReflection lr = Object.FindObjectOfType<LightReflection>();
        if (lr == null || layout.mirrors == null)
        {
            return;
        }

        // Level difficulty rule:
        // The receiver only accepts the beam after it has reflected from enough different mirrors.
        // For these layouts, the required count equals the intended solution mirror count.
        lr.requiredMirrorReflections = Mathf.Max(1, layout.requiredMirrorReflections);
        lr.requireExactMirrorReflectionCount = true;
        lr.maxReflections = Mathf.Max(lr.maxReflections, lr.requiredMirrorReflections + 4);
    }

    private GameObject CreateOrUpdateCube(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
        }

        go.transform.position = position;
        go.transform.localScale = scale;
        Renderer r = go.GetComponent<Renderer>();
        if (r != null && material != null)
        {
            r.material = material;
        }
        return go;
    }

    private void SetPosition(string name, Vector3 position)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) go.transform.position = position;
    }

    private void SetTransform(string name, Vector3 position, Vector3 scale)
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.localScale = scale;
        }
    }


    private float CalculateMirrorYaw(Vector3 previousPoint, Vector3 mirrorPoint, Vector3 nextPoint)
    {
        Vector3 incomingDirection = mirrorPoint - previousPoint;
        incomingDirection.y = 0f;
        incomingDirection.Normalize();

        Vector3 outgoingDirection = nextPoint - mirrorPoint;
        outgoingDirection.y = 0f;
        outgoingDirection.Normalize();

        Vector3 mirrorNormal = incomingDirection - outgoingDirection;
        mirrorNormal.y = 0f;

        if (mirrorNormal.sqrMagnitude < 0.0001f)
        {
            mirrorNormal = Vector3.forward;
        }
        else
        {
            mirrorNormal.Normalize();
        }

        return Mathf.Atan2(mirrorNormal.x, mirrorNormal.z) * Mathf.Rad2Deg;
    }

    private void SetYaw(string name, float yaw)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void LookAtHorizontal(string name, Vector3 target)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Vector3 dir = target - go.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f) go.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    private struct LevelLayout
    {
        public bool valid;
        public string existingPrefix;
        public string runtimePrefix;
        public float halfX;
        public float halfZ;
        public Vector3 player;
        public Vector3 light;
        public Vector3[] mirrors;
        public Vector3 receiver;
        public Vector3 door;
        public Vector3 exit;
        public float[] mirrorYaws;
        public int requiredMirrorReflections;
        public Vector3[] blockerPositions;
        public Vector3[] blockerScales;
        public Vector3[] decoyMirrors;
        public float[] decoyYaws;
    }
}
