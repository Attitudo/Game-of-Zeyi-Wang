using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelTwoBootstrapper
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
        BuildLevelTwoAtRuntime(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildLevelTwoAtRuntime(scene);
    }

    private static void BuildLevelTwoAtRuntime(Scene scene)
    {
        if (scene.name != "Level02")
        {
            return;
        }

        EnsureGameManager();
        ConfigureObjective();
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

    private static void ConfigureObjective()
    {
        GameManager manager = Object.FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.ConfigureObjective(
                "LEVEL 2 OBJECTIVE:",
                "1. Redirect the beam through Mirror 01 and Mirror 02.\n" +
                "2. Keep the receiver powered to raise the security door.\n" +
                "3. Avoid the AI guard and reach the final green exit zone.\n" +
                "Controls: WASD move, Mouse look, Space jump, Q/E rotate nearby mirror, R restart after win/loss.",
                "All levels complete. You solved the two-mirror security lab. Press R to replay Level 2.");
        }
    }

    private static void ConfigurePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        player.transform.position = new Vector3(-7f, 1f, -5f);
        player.transform.rotation = Quaternion.Euler(0f, 35f, 0f);

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
        GameObject originalPlane = GameObject.Find("Plane");
        if (originalPlane != null)
        {
            originalPlane.SetActive(false);
        }

        GameObject lightSource = GameObject.Find("LightSource");
        if (lightSource != null)
        {
            lightSource.transform.position = new Vector3(-7f, 1.4f, -2f);
            lightSource.transform.rotation = Quaternion.LookRotation(Vector3.right);
            Light light = lightSource.GetComponent<Light>();
            if (light != null)
            {
                light.color = new Color(1f, 0.9f, 0.2f);
                light.intensity = 4.2f;
                light.range = 18f;
            }
        }

        LightReflection reflection = Object.FindObjectOfType<LightReflection>();
        if (reflection != null)
        {
            reflection.maxReflections = 8;
            reflection.maxDistance = 55f;
            reflection.receiverActivationRadius = 0.85f;
        }

        Material glassMat = CreateMaterial("Runtime_Level02_MirrorGlass", new Color(0.45f, 0.78f, 0.95f));
        Material frameMat = CreateMaterial("Runtime_Level02_MirrorFrame", new Color(0.04f, 0.05f, 0.06f));

        GameObject mirrorOne = GameObject.Find("Mirror_01");
        if (mirrorOne == null)
        {
            mirrorOne = GameObject.Find("Mirror");
        }

        if (mirrorOne != null)
        {
            mirrorOne.name = "Mirror_01";
            ConfigureMirror(mirrorOne, new Vector3(-3f, 1.25f, -2f), Quaternion.Euler(0f, 135f, 0f), glassMat, frameMat);
        }

        GameObject mirrorTwo = GameObject.Find("Mirror_02");
        if (mirrorTwo == null)
        {
            mirrorTwo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mirrorTwo.name = "Mirror_02";
            mirrorTwo.AddComponent<MirrorController>();
        }
        ConfigureMirror(mirrorTwo, new Vector3(-3f, 1.25f, 3f), Quaternion.Euler(0f, -45f, 0f), glassMat, frameMat);

        GameObject door = FindDoor();
        if (door != null)
        {
            door.name = "SecurityDoor";
            door.transform.position = new Vector3(7f, 1.25f, 5.6f);
            door.transform.rotation = Quaternion.identity;
            door.transform.localScale = new Vector3(3f, 2.5f, 0.35f);
            door.SetActive(true);

            DoorAnimator animator = door.GetComponent<DoorAnimator>();
            if (animator == null)
            {
                animator = door.AddComponent<DoorAnimator>();
            }
            animator.openOffset = new Vector3(0f, 3f, 0f);
            animator.openSpeed = 3f;
        }

        GameObject receiver = GameObject.Find("Receiver");
        if (receiver != null)
        {
            receiver.name = "Receiver_01";
            receiver.transform.position = new Vector3(5f, 1.25f, 3f);
            receiver.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
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

    private static void ConfigureMirror(GameObject mirror, Vector3 position, Quaternion rotation, Material glassMat, Material frameMat)
    {
        mirror.transform.position = position;
        mirror.transform.rotation = rotation;
        mirror.transform.localScale = new Vector3(2f, 2.1f, 0.08f);
        mirror.tag = "Mirror";

        BoxCollider collider = mirror.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = mirror.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;
        collider.size = new Vector3(2f, 2f, 2f);

        Rigidbody rb = mirror.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = mirror.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;

        MirrorController controller = mirror.GetComponent<MirrorController>();
        if (controller == null)
        {
            controller = mirror.AddComponent<MirrorController>();
        }
        controller.rotateStep = 15f;
        controller.interactDistance = 4f;

        Renderer renderer = mirror.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = glassMat;
        }

        CreateVisualChildCubeOnce(mirror.name + "_FrameTop", mirror.transform, new Vector3(0f, 0.54f, -0.08f), new Vector3(1.15f, 0.08f, 0.08f), frameMat);
        CreateVisualChildCubeOnce(mirror.name + "_FrameBottom", mirror.transform, new Vector3(0f, -0.54f, -0.08f), new Vector3(1.15f, 0.08f, 0.08f), frameMat);
        CreateVisualChildCubeOnce(mirror.name + "_FrameLeft", mirror.transform, new Vector3(-0.56f, 0f, -0.08f), new Vector3(0.08f, 1.1f, 0.08f), frameMat);
        CreateVisualChildCubeOnce(mirror.name + "_FrameRight", mirror.transform, new Vector3(0.56f, 0f, -0.08f), new Vector3(0.08f, 1.1f, 0.08f), frameMat);
    }

    private static void CreateLevelGeometry()
    {
        if (GameObject.Find("LevelTwoRuntimeGeometry") != null)
        {
            return;
        }

        GameObject parent = new GameObject("LevelTwoRuntimeGeometry");

        Material floorDark = CreateMaterial("Runtime_L2_FloorDark", new Color(0.14f, 0.15f, 0.17f));
        Material floorLight = CreateMaterial("Runtime_L2_FloorLight", new Color(0.22f, 0.23f, 0.25f));
        Material wallMat = CreateMaterial("Runtime_L2_WallPanel", new Color(0.38f, 0.4f, 0.45f));
        Material trimMat = CreateMaterial("Runtime_L2_DarkTrim", new Color(0.06f, 0.07f, 0.09f));
        Material warningMat = CreateMaterial("Runtime_L2_WarningRed", new Color(0.9f, 0.12f, 0.08f));
        Material greenMat = CreateMaterial("Runtime_L2_ExitGreen", new Color(0.1f, 0.85f, 0.45f));
        Material propMat = CreateMaterial("Runtime_L2_StorageCrate", new Color(0.22f, 0.25f, 0.31f));
        Material beamMat = CreateMaterial("Runtime_L2_LightDevice", new Color(1f, 0.85f, 0.1f));

        for (int x = -5; x <= 5; x++)
        {
            for (int z = -4; z <= 4; z++)
            {
                Material mat = ((x + z) % 2 == 0) ? floorDark : floorLight;
                CreateCube($"L2_FloorTile_{x}_{z}", new Vector3(x * 2.0f, -0.05f, z * 2.0f), new Vector3(1.95f, 0.08f, 1.95f), mat, parent.transform);
            }
        }

        CreateCube("L2_BackWall", new Vector3(0f, 1.5f, -8.5f), new Vector3(21f, 3f, 0.4f), wallMat, parent.transform);
        CreateCube("L2_LeftWall", new Vector3(-10.5f, 1.5f, 0f), new Vector3(0.4f, 3f, 17f), wallMat, parent.transform);
        CreateCube("L2_RightWall", new Vector3(10.5f, 1.5f, 0f), new Vector3(0.4f, 3f, 17f), wallMat, parent.transform);
        CreateCube("L2_FrontWallLeft", new Vector3(-4.5f, 1.5f, 7.5f), new Vector3(12f, 3f, 0.4f), wallMat, parent.transform);
        CreateCube("L2_FrontWallRight", new Vector3(9.0f, 1.5f, 7.5f), new Vector3(3f, 3f, 0.4f), wallMat, parent.transform);

        CreateCube("L2_DoorFrameTop", new Vector3(7f, 2.7f, 5.6f), new Vector3(3.7f, 0.25f, 0.55f), trimMat, parent.transform);
        CreateCube("L2_DoorFrameLeft", new Vector3(5.15f, 1.35f, 5.6f), new Vector3(0.25f, 2.7f, 0.55f), trimMat, parent.transform);
        CreateCube("L2_DoorFrameRight", new Vector3(8.85f, 1.35f, 5.6f), new Vector3(0.25f, 2.7f, 0.55f), trimMat, parent.transform);

        GameObject exitMarker = CreateCube("L2_ExitMarker", new Vector3(7f, 0.03f, 7.0f), new Vector3(3.2f, 0.05f, 1.1f), greenMat, parent.transform);
        RemoveCollider(exitMarker);

        CreateCube("L2_EmitterBase", new Vector3(-7f, 0.15f, -2f), new Vector3(1.1f, 0.3f, 0.8f), trimMat, parent.transform);
        CreateCube("L2_EmitterStand", new Vector3(-7f, 0.8f, -2f), new Vector3(0.25f, 1.2f, 0.25f), trimMat, parent.transform);
        CreateCube("L2_EmitterHousing", new Vector3(-7f, 1.4f, -2f), new Vector3(0.7f, 0.5f, 0.5f), beamMat, parent.transform);

        CreateCube("L2_MirrorBase_01", new Vector3(-3f, 0.08f, -2f), new Vector3(1.2f, 0.16f, 0.8f), trimMat, parent.transform);
        CreateCube("L2_MirrorPedestal_01", new Vector3(-3f, 0.55f, -2f), new Vector3(0.35f, 1.1f, 0.35f), trimMat, parent.transform);
        CreateCube("L2_MirrorBase_02", new Vector3(-3f, 0.08f, 3f), new Vector3(1.2f, 0.16f, 0.8f), trimMat, parent.transform);
        CreateCube("L2_MirrorPedestal_02", new Vector3(-3f, 0.55f, 3f), new Vector3(0.35f, 1.1f, 0.35f), trimMat, parent.transform);

        CreateCube("L2_ReceiverBase", new Vector3(5f, 0.08f, 3f), new Vector3(1.0f, 0.16f, 0.8f), trimMat, parent.transform);
        CreateCube("L2_ReceiverPedestal", new Vector3(5f, 0.55f, 3f), new Vector3(0.35f, 1.1f, 0.35f), trimMat, parent.transform);

        CreateCube("L2_CentralCover_A", new Vector3(1.5f, 0.75f, -4.5f), new Vector3(3.4f, 1.5f, 0.6f), propMat, parent.transform);
        CreateCube("L2_CentralCover_B", new Vector3(1.5f, 0.75f, 0.3f), new Vector3(0.6f, 1.5f, 2.8f), propMat, parent.transform);
        CreateCube("L2_StorageCrate_A", new Vector3(-8.2f, 0.55f, 4.8f), new Vector3(1.4f, 1.1f, 1.2f), propMat, parent.transform);
        CreateCube("L2_StorageCrate_B", new Vector3(2.9f, 0.55f, 5.3f), new Vector3(1.2f, 1.1f, 1.2f), propMat, parent.transform);
        CreateCube("L2_ControlPanel", new Vector3(8.7f, 0.65f, 1.2f), new Vector3(1.1f, 1.3f, 0.35f), warningMat, parent.transform);

        GameObject panelA = CreateCube("L2_WarningLightPanel_A", new Vector3(-10.28f, 1.8f, -4f), new Vector3(0.04f, 0.45f, 2.0f), warningMat, parent.transform);
        GameObject panelB = CreateCube("L2_ExitLightPanel_B", new Vector3(10.28f, 1.8f, 4f), new Vector3(0.04f, 0.45f, 2.0f), greenMat, parent.transform);
        RemoveCollider(panelA);
        RemoveCollider(panelB);
    }

    private static void CreateGuard()
    {
        if (GameObject.Find("GuardAI_Level02") != null)
        {
            return;
        }

        GameObject p1 = new GameObject("L2_GuardPatrolPoint_A");
        p1.transform.position = new Vector3(2.5f, 1f, -5.2f);
        GameObject p2 = new GameObject("L2_GuardPatrolPoint_B");
        p2.transform.position = new Vector3(7.2f, 1f, -5.2f);
        GameObject p3 = new GameObject("L2_GuardPatrolPoint_C");
        p3.transform.position = new Vector3(7.2f, 1f, 1.5f);
        GameObject p4 = new GameObject("L2_GuardPatrolPoint_D");
        p4.transform.position = new Vector3(2.5f, 1f, 1.5f);

        Material guardBodyMat = CreateMaterial("Runtime_L2_GuardBody", new Color(0.05f, 0.12f, 0.32f));
        Material guardVisorMat = CreateMaterial("Runtime_L2_GuardVisor", new Color(0.9f, 0.15f, 0.08f));

        GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        guard.name = "GuardAI_Level02";
        guard.transform.position = p1.transform.position;
        guard.transform.rotation = Quaternion.LookRotation(Vector3.forward);

        Renderer guardRenderer = guard.GetComponent<Renderer>();
        if (guardRenderer != null)
        {
            guardRenderer.material = guardBodyMat;
        }

        CreateVisualChildCubeOnce("L2_GuardVisor", guard.transform, new Vector3(0f, 0.42f, 0.43f), new Vector3(0.65f, 0.16f, 0.08f), guardVisorMat);
        CreateVisualChildCubeOnce("L2_GuardBackpack", guard.transform, new Vector3(0f, 0.05f, -0.42f), new Vector3(0.55f, 0.55f, 0.16f), guardBodyMat);

        Light searchLight = new GameObject("L2_GuardSearchLight").AddComponent<Light>();
        searchLight.transform.SetParent(guard.transform);
        searchLight.transform.localPosition = new Vector3(0f, 0.3f, 0.55f);
        searchLight.transform.localRotation = Quaternion.identity;
        searchLight.type = LightType.Spot;
        searchLight.color = new Color(1f, 0.75f, 0.35f);
        searchLight.intensity = 1.3f;
        searchLight.range = 8f;
        searchLight.spotAngle = 45f;

        GuardAI guardAI = guard.AddComponent<GuardAI>();
        guardAI.patrolPoints = new[] { p1.transform, p2.transform, p3.transform, p4.transform };
        guardAI.viewDistance = 8f;
        guardAI.viewAngle = 85f;
        guardAI.patrolSpeed = 2.1f;
        guardAI.chaseSpeed = 4.2f;
    }

    private static void CreateExitZone()
    {
        GameObject exitZone = GameObject.Find("ExitZone");
        if (exitZone == null)
        {
            exitZone = new GameObject("ExitZone");
            BoxCollider box = exitZone.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(3.2f, 2f, 1.1f);
        }

        exitZone.transform.position = new Vector3(7f, 1f, 7f);
        ExitZone exitScript = exitZone.GetComponent<ExitZone>();
        if (exitScript == null)
        {
            exitScript = exitZone.AddComponent<ExitZone>();
        }
        exitScript.isFinalLevel = true;
        exitScript.finalLevelMessage = "All levels complete. You solved the two-mirror security lab. Press R to replay Level 2.";
    }

    private static void ConfigureLighting()
    {
        RenderSettings.ambientLight = new Color(0.36f, 0.37f, 0.42f);

        GameObject keyLightObject = GameObject.Find("Runtime_L2_KeyLight");
        if (keyLightObject == null)
        {
            keyLightObject = new GameObject("Runtime_L2_KeyLight");
            Light keyLight = keyLightObject.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 0.85f;
            keyLight.color = new Color(0.9f, 0.95f, 1f);
            keyLightObject.transform.rotation = Quaternion.Euler(50f, -25f, 0f);
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

    private static GameObject CreateVisualChildCubeOnce(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing.gameObject;
        }

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
