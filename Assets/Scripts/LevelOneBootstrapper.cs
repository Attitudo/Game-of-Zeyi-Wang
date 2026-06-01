using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelOneBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneReloadCallback()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BuildInitialLevel()
    {
        BuildLevelOneAtRuntime(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildLevelOneAtRuntime(scene);
    }

    private static void BuildLevelOneAtRuntime(Scene scene)
    {
        if (scene.name != "MainScene")
        {
            return;
        }

        EnsureGameManager();
        ConfigurePlayer();
        ConfigureLightPuzzle();
        CreateLevelGeometry();
        CreateGuard();
        CreateExitZone();
        ConfigureLighting();
    }

    private static void EnsureGameManager()
    {
        if (Object.FindObjectOfType<GameManager>() != null)
        {
            return;
        }

        GameObject manager = new GameObject("GameManager");
        manager.AddComponent<GameManager>();
    }

    private static void ConfigurePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        player.transform.position = new Vector3(-2f, 1f, -3f);
        player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.mass = 1f;
        }

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.groundLayer = ~0;
        }
    }

    private static void ConfigureLightPuzzle()
    {
        GameObject lightSource = GameObject.Find("LightSource");
        if (lightSource != null)
        {
            lightSource.transform.position = new Vector3(-6.5f, 1.4f, -0.2f);
            lightSource.transform.rotation = Quaternion.LookRotation(Vector3.right);
            Light light = lightSource.GetComponent<Light>();
            if (light != null)
            {
                light.color = new Color(1f, 0.9f, 0.2f);
                light.intensity = 3.8f;
                light.range = 14f;
            }
        }

        GameObject mirror = GameObject.Find("Mirror");
        if (mirror != null)
        {
            mirror.transform.position = new Vector3(0f, 1.25f, 0f);
            mirror.transform.rotation = Quaternion.Euler(0f, -30f, 0f);
            mirror.transform.localScale = new Vector3(2f, 2.1f, 0.08f);
            mirror.tag = "Mirror";

            MirrorController controller = mirror.GetComponent<MirrorController>();
            if (controller != null)
            {
                controller.rotateStep = 15f;
                controller.interactDistance = 4f;
            }
        }

        GameObject door = FindDoor();
        if (door != null)
        {
            door.name = "SecurityDoor";
            door.transform.position = new Vector3(0f, 1.25f, 7.5f);
            door.transform.localScale = new Vector3(3f, 2.5f, 0.35f);
            door.SetActive(true);
        }

        GameObject receiver = GameObject.Find("Receiver");
        if (receiver != null)
        {
            receiver.transform.position = new Vector3(0f, 1.25f, 5f);
            receiver.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            receiver.tag = "Receiver";

            SphereCollider receiverCollider = receiver.GetComponent<SphereCollider>();
            if (receiverCollider != null)
            {
                receiverCollider.radius = 1.1f;
                receiverCollider.isTrigger = false;
            }

            Receiver receiverScript = receiver.GetComponent<Receiver>();
            if (receiverScript != null)
            {
                receiverScript.door = door;
                receiverScript.SetPowered(false);
            }
        }
    }

    private static void CreateLevelGeometry()
    {
        if (GameObject.Find("LevelOneRuntimeGeometry") != null)
        {
            return;
        }

        GameObject parent = new GameObject("LevelOneRuntimeGeometry");

        Material floorDark = CreateMaterial("Runtime_FloorDark", new Color(0.16f, 0.17f, 0.18f));
        Material floorLight = CreateMaterial("Runtime_FloorLight", new Color(0.23f, 0.24f, 0.25f));
        Material wallMat = CreateMaterial("Runtime_WallPanel", new Color(0.42f, 0.43f, 0.46f));
        Material trimMat = CreateMaterial("Runtime_DarkTrim", new Color(0.08f, 0.09f, 0.1f));
        Material glassMat = CreateMaterial("Runtime_MirrorGlass", new Color(0.45f, 0.75f, 0.95f));
        Material frameMat = CreateMaterial("Runtime_MirrorFrame", new Color(0.04f, 0.05f, 0.06f));
        Material beamMat = CreateMaterial("Runtime_LightDevice", new Color(1f, 0.85f, 0.1f));
        Material redMat = CreateMaterial("Runtime_RedPanel", new Color(0.85f, 0.12f, 0.08f));
        Material greenMat = CreateMaterial("Runtime_ExitGreen", new Color(0.1f, 0.85f, 0.45f));
        Material propMat = CreateMaterial("Runtime_StorageCrate", new Color(0.23f, 0.26f, 0.3f));

        // Tiled laboratory floor.
        for (int x = -4; x <= 4; x++)
        {
            for (int z = -4; z <= 4; z++)
            {
                Material mat = ((x + z) % 2 == 0) ? floorDark : floorLight;
                CreateCube($"FloorTile_{x}_{z}", new Vector3(x * 2.1f, -0.05f, z * 2.1f), new Vector3(2.05f, 0.08f, 2.05f), mat, parent.transform);
            }
        }

        // Outer room shell.
        CreateCube("BackWall", new Vector3(0f, 1.5f, -9.5f), new Vector3(20f, 3f, 0.4f), wallMat, parent.transform);
        CreateCube("LeftWall", new Vector3(-9.5f, 1.5f, 0f), new Vector3(0.4f, 3f, 20f), wallMat, parent.transform);
        CreateCube("RightWall", new Vector3(9.5f, 1.5f, 0f), new Vector3(0.4f, 3f, 20f), wallMat, parent.transform);
        CreateCube("FrontWallLeft", new Vector3(-5.8f, 1.5f, 8.2f), new Vector3(7.4f, 3f, 0.4f), wallMat, parent.transform);
        CreateCube("FrontWallRight", new Vector3(5.8f, 1.5f, 8.2f), new Vector3(7.4f, 3f, 0.4f), wallMat, parent.transform);
        CreateCube("CeilingStripBack", new Vector3(0f, 3.05f, -9.3f), new Vector3(18f, 0.12f, 0.25f), trimMat, parent.transform);
        CreateCube("CeilingStripLeft", new Vector3(-9.3f, 3.05f, 0f), new Vector3(0.25f, 0.12f, 18f), trimMat, parent.transform);
        CreateCube("CeilingStripRight", new Vector3(9.3f, 3.05f, 0f), new Vector3(0.25f, 0.12f, 18f), trimMat, parent.transform);

        // Interior cover and sight blockers for guard gameplay.
        CreateCube("CentralCoverWall", new Vector3(3.2f, 1.2f, 2.5f), new Vector3(0.5f, 2.4f, 5f), wallMat, parent.transform);
        CreateCube("LowCoverBlock_A", new Vector3(-3.3f, 0.55f, 2.2f), new Vector3(1.6f, 1.1f, 1.2f), propMat, parent.transform);
        CreateCube("LowCoverBlock_B", new Vector3(5.7f, 0.55f, -1.8f), new Vector3(1.6f, 1.1f, 1.2f), propMat, parent.transform);
        CreateCube("StorageCrate_A", new Vector3(-6.6f, 0.45f, -6.4f), new Vector3(1.2f, 0.9f, 1.2f), propMat, parent.transform);
        CreateCube("StorageCrate_B", new Vector3(-5.3f, 0.75f, -6.4f), new Vector3(1.1f, 1.5f, 1.1f), propMat, parent.transform);
        CreateCube("SecurityConsole", new Vector3(1.9f, 0.65f, 5.2f), new Vector3(1.1f, 1.3f, 0.35f), redMat, parent.transform);

        // Door frame and exit area.
        CreateCube("DoorFrameTop", new Vector3(0f, 2.7f, 7.55f), new Vector3(3.6f, 0.25f, 0.55f), trimMat, parent.transform);
        CreateCube("DoorFrameLeft", new Vector3(-1.8f, 1.35f, 7.55f), new Vector3(0.25f, 2.7f, 0.55f), trimMat, parent.transform);
        CreateCube("DoorFrameRight", new Vector3(1.8f, 1.35f, 7.55f), new Vector3(0.25f, 2.7f, 0.55f), trimMat, parent.transform);
        GameObject exitMarker = CreateCube("ExitMarker", new Vector3(0f, 0.03f, 9f), new Vector3(3f, 0.05f, 1.2f), greenMat, parent.transform);
        RemoveCollider(exitMarker);

        // Mirror frame and stand.
        GameObject mirror = GameObject.Find("Mirror");
        if (mirror != null)
        {
            Renderer mirrorRenderer = mirror.GetComponent<Renderer>();
            if (mirrorRenderer != null)
            {
                mirrorRenderer.material = glassMat;
            }

            CreateVisualChildCube("MirrorFrameTop", mirror.transform, new Vector3(0f, 0.54f, -0.08f), new Vector3(1.15f, 0.08f, 0.08f), frameMat);
            CreateVisualChildCube("MirrorFrameBottom", mirror.transform, new Vector3(0f, -0.54f, -0.08f), new Vector3(1.15f, 0.08f, 0.08f), frameMat);
            CreateVisualChildCube("MirrorFrameLeft", mirror.transform, new Vector3(-0.56f, 0f, -0.08f), new Vector3(0.08f, 1.1f, 0.08f), frameMat);
            CreateVisualChildCube("MirrorFrameRight", mirror.transform, new Vector3(0.56f, 0f, -0.08f), new Vector3(0.08f, 1.1f, 0.08f), frameMat);
            CreateCube("MirrorPedestal", new Vector3(0f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 0.35f), frameMat, parent.transform);
            CreateCube("MirrorBase", new Vector3(0f, 0.08f, 0f), new Vector3(1.2f, 0.16f, 0.8f), frameMat, parent.transform);
        }

        // Light emitter stand.
        CreateCube("EmitterBase", new Vector3(-6.5f, 0.15f, -0.2f), new Vector3(1.1f, 0.3f, 0.8f), trimMat, parent.transform);
        CreateCube("EmitterStand", new Vector3(-6.5f, 0.8f, -0.2f), new Vector3(0.25f, 1.2f, 0.25f), trimMat, parent.transform);
        CreateCube("EmitterHousing", new Vector3(-6.5f, 1.4f, -0.2f), new Vector3(0.7f, 0.5f, 0.5f), beamMat, parent.transform);

        // Receiver stand.
        CreateCube("ReceiverPedestal", new Vector3(0f, 0.55f, 5f), new Vector3(0.35f, 1.1f, 0.35f), trimMat, parent.transform);
        CreateCube("ReceiverBase", new Vector3(0f, 0.08f, 5f), new Vector3(1.0f, 0.16f, 0.8f), trimMat, parent.transform);

        // Accent light panels. They are decorative only.
        GameObject panelA = CreateCube("WallLightPanel_A", new Vector3(-9.28f, 1.8f, -4f), new Vector3(0.04f, 0.45f, 2.0f), greenMat, parent.transform);
        GameObject panelB = CreateCube("WallLightPanel_B", new Vector3(9.28f, 1.8f, 4f), new Vector3(0.04f, 0.45f, 2.0f), greenMat, parent.transform);
        RemoveCollider(panelA);
        RemoveCollider(panelB);
    }

    private static void CreateGuard()
    {
        if (GameObject.Find("GuardAI") != null)
        {
            return;
        }

        GameObject p1 = new GameObject("GuardPatrolPoint_A");
        p1.transform.position = new Vector3(4f, 1f, -4f);
        GameObject p2 = new GameObject("GuardPatrolPoint_B");
        p2.transform.position = new Vector3(4f, 1f, 4f);
        GameObject p3 = new GameObject("GuardPatrolPoint_C");
        p3.transform.position = new Vector3(7f, 1f, 4f);
        GameObject p4 = new GameObject("GuardPatrolPoint_D");
        p4.transform.position = new Vector3(7f, 1f, -4f);

        Material guardBodyMat = CreateMaterial("Runtime_GuardBody", new Color(0.05f, 0.12f, 0.32f));
        Material guardVisorMat = CreateMaterial("Runtime_GuardVisor", new Color(0.9f, 0.15f, 0.08f));

        GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        guard.name = "GuardAI";
        guard.transform.position = p1.transform.position;
        guard.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        guard.transform.localScale = new Vector3(1f, 1f, 1f);

        Renderer guardRenderer = guard.GetComponent<Renderer>();
        if (guardRenderer != null)
        {
            guardRenderer.material = guardBodyMat;
        }

        CreateVisualChildCube("GuardVisor", guard.transform, new Vector3(0f, 0.42f, 0.43f), new Vector3(0.65f, 0.16f, 0.08f), guardVisorMat);
        CreateVisualChildCube("GuardBackpack", guard.transform, new Vector3(0f, 0.05f, -0.42f), new Vector3(0.55f, 0.55f, 0.16f), guardBodyMat);

        Light searchLight = new GameObject("GuardSearchLight").AddComponent<Light>();
        searchLight.transform.SetParent(guard.transform);
        searchLight.transform.localPosition = new Vector3(0f, 0.3f, 0.55f);
        searchLight.transform.localRotation = Quaternion.identity;
        searchLight.type = LightType.Spot;
        searchLight.color = new Color(1f, 0.75f, 0.35f);
        searchLight.intensity = 1.2f;
        searchLight.range = 7f;
        searchLight.spotAngle = 45f;

        GuardAI guardAI = guard.AddComponent<GuardAI>();
        guardAI.patrolPoints = new[] { p1.transform, p2.transform, p3.transform, p4.transform };
        guardAI.viewDistance = 7f;
        guardAI.viewAngle = 80f;
        guardAI.patrolSpeed = 2f;
        guardAI.chaseSpeed = 4f;
    }

    private static void CreateExitZone()
    {
        if (GameObject.Find("ExitZone") != null)
        {
            return;
        }

        GameObject exitZone = new GameObject("ExitZone");
        exitZone.transform.position = new Vector3(0f, 1f, 9f);
        BoxCollider box = exitZone.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(3f, 2f, 1.2f);
        exitZone.AddComponent<ExitZone>();
    }

    private static void ConfigureLighting()
    {
        RenderSettings.ambientLight = new Color(0.35f, 0.36f, 0.4f);

        GameObject keyLightObject = GameObject.Find("Runtime_KeyLight");
        if (keyLightObject == null)
        {
            keyLightObject = new GameObject("Runtime_KeyLight");
            Light keyLight = keyLightObject.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 0.8f;
            keyLight.color = new Color(0.9f, 0.95f, 1f);
            keyLightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }

    private static GameObject FindDoor()
    {
        GameObject door = GameObject.Find("Door");
        if (door != null)
        {
            return door;
        }

        return GameObject.Find("SecurityDoor");
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
        return cube;
    }

    private static GameObject CreateVisualChildCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = localScale;
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
        RemoveCollider(cube);
        return cube;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = name;
        material.color = color;
        return material;
    }

    private static void RemoveCollider(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
    }
}
