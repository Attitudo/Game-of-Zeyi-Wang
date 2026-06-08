using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps the selected base scene style, then adds stable character models,
/// expands/seals maps, corrects Level 2 mirror routing, and configures expert Levels 5-6.
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
        CacheMaterials(layout.existingPrefix);
        EnsureMirrorCount(layout.mirrors.Length);
        RepositionGameplayObjects(layout);
        ExpandBoundaryWalls(layout);
        CreateFloorAndCeiling(layout);
        MoveSideObstacles(layout);
        CreateExpertBlockers(layout);
        ConfigureExit(sceneName, layout);
        ConfigureExpertPickups(sceneName);
        ImproveLaserDistance();
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
                mirrorYaws = new float[] { 142f }
            };
        }

        if (sceneName == "Level02")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L2",
                runtimePrefix = "L2",
                halfX = 14f,
                halfZ = 12f,
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
                halfX = 16f,
                halfZ = 13f,
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
                mirrorYaws = new float[] { 150f, 15f, 126f }
            };
        }

        if (sceneName == "Level04")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L4",
                halfX = 18f,
                halfZ = 15f,
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
                mirrorYaws = new float[] { 155f, 20f, 135f, -18f }
            };
        }

        if (sceneName == "Level05")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L5",
                halfX = 22f,
                halfZ = 18f,
                player = new Vector3(-17.5f, 1f, -13.5f),
                light = new Vector3(-16.5f, 1.4f, -10.5f),
                mirrors = new Vector3[]
                {
                    new Vector3(-10.5f, 1.25f, -10.5f),
                    new Vector3(-10.5f, 1.25f, -2.0f),
                    new Vector3(-2.5f, 1.25f, -2.0f),
                    new Vector3(-2.5f, 1.25f, 6.5f),
                    new Vector3(8.5f, 1.25f, 6.5f)
                },
                receiver = new Vector3(15.0f, 1.25f, 10.5f),
                door = new Vector3(17.5f, 1.25f, 14.6f),
                exit = new Vector3(17.5f, 1f, 16.2f),
                mirrorYaws = new float[] { 145f, 20f, 138f, -15f, 125f },
                blockerPositions = new Vector3[]
                {
                    new Vector3(-1.0f, 0.9f, -9.0f),
                    new Vector3(4.5f, 0.9f, 1.8f),
                    new Vector3(11.0f, 0.9f, 6.5f),
                    new Vector3(-14.5f, 0.9f, 4.5f)
                },
                blockerScales = new Vector3[]
                {
                    new Vector3(0.9f, 1.8f, 9.0f),
                    new Vector3(8.0f, 1.8f, 0.9f),
                    new Vector3(0.9f, 1.8f, 7.0f),
                    new Vector3(5.0f, 1.8f, 0.9f)
                }
            };
        }

        if (sceneName == "Level06")
        {
            return new LevelLayout
            {
                valid = true,
                existingPrefix = "L4",
                runtimePrefix = "L6",
                halfX = 25f,
                halfZ = 20f,
                player = new Vector3(-20f, 1f, -15.5f),
                light = new Vector3(-19.0f, 1.4f, -13.5f),
                mirrors = new Vector3[]
                {
                    new Vector3(-13.5f, 1.25f, -13.5f),
                    new Vector3(-13.5f, 1.25f, -5.0f),
                    new Vector3(-5.0f, 1.25f, -5.0f),
                    new Vector3(-5.0f, 1.25f, 4.5f),
                    new Vector3(5.5f, 1.25f, 4.5f),
                    new Vector3(5.5f, 1.25f, 12.5f)
                },
                receiver = new Vector3(18.0f, 1.25f, 12.5f),
                door = new Vector3(20.5f, 1.25f, 16.8f),
                exit = new Vector3(20.5f, 1f, 18.5f),
                mirrorYaws = new float[] { 148f, 15f, 135f, -12f, 135f, -20f },
                blockerPositions = new Vector3[]
                {
                    new Vector3(-7.0f, 0.9f, -12.0f),
                    new Vector3(-1.0f, 0.9f, -1.0f),
                    new Vector3(9.0f, 0.9f, 0.0f),
                    new Vector3(13.0f, 0.9f, 9.0f),
                    new Vector3(-18.0f, 0.9f, 4.0f)
                },
                blockerScales = new Vector3[]
                {
                    new Vector3(0.9f, 1.8f, 10.0f),
                    new Vector3(9.0f, 1.8f, 0.9f),
                    new Vector3(0.9f, 1.8f, 10.0f),
                    new Vector3(8.0f, 1.8f, 0.9f),
                    new Vector3(6.0f, 1.8f, 0.9f)
                }
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

        for (int i = 0; i < layout.mirrors.Length; i++)
        {
            string name = "Mirror_" + (i + 1).ToString("00");
            SetPosition(name, layout.mirrors[i]);
            SetYaw(name, layout.mirrorYaws.Length > i ? layout.mirrorYaws[i] : 45f);
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
            CreateOrUpdateCube(layout.runtimePrefix + "_LaserCover_" + i, layout.blockerPositions[i], layout.blockerScales[i], coverMaterial);
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
            CreatePickup("L5_EMP_Device", PickupType.EmpDevice, new Vector3(-15.5f, 1.2f, -6f), "EMP Device");
            CreatePickup("L5_Keycard", PickupType.SecurityKeycard, new Vector3(-16f, 1.2f, 12.5f), "Crypt Keycard");
            CreatePickup("L5_Core_A", PickupType.EnergyCore, new Vector3(4f, 1.2f, -12.5f), "Energy Core A");
            CreatePickup("L5_Core_B", PickupType.EnergyCore, new Vector3(14f, 1.2f, -3f), "Energy Core B");
        }
        else if (sceneName == "Level06")
        {
            CreatePickup("L6_EMP_Device", PickupType.EmpDevice, new Vector3(-18f, 1.2f, -10f), "Advanced EMP Device");
            CreatePickup("L6_Master_Keycard", PickupType.SecurityKeycard, new Vector3(-18f, 1.2f, 13.5f), "Master Keycard");
            CreatePickup("L6_Core_A", PickupType.EnergyCore, new Vector3(-2f, 1.2f, -15f), "Energy Core A");
            CreatePickup("L6_Core_B", PickupType.EnergyCore, new Vector3(12f, 1.2f, -6f), "Energy Core B");
            CreatePickup("L6_Core_C", PickupType.EnergyCore, new Vector3(17f, 1.2f, 4f), "Energy Core C");
            CreatePickup("L6_EMP_Charge", PickupType.EmpCharge, new Vector3(0f, 1.2f, 14f), "EMP Charge");
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
            lr.maxDistance = Mathf.Max(lr.maxDistance, 110f);
            lr.receiverActivationRadius = Mathf.Max(lr.receiverActivationRadius, 1.15f);
        }
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
        public Vector3[] blockerPositions;
        public Vector3[] blockerScales;
    }
}
