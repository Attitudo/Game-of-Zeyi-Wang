using UnityEngine;

public enum PickupType
{
    SecurityKeycard,
    EnergyCore,
    EmpDevice,
    EmpCharge
}

public class PickupItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public PickupType pickupType = PickupType.SecurityKeycard;
    public int amount = 1;
    public string displayName = "Item";

    [Header("Visual Motion")]
    public float rotationSpeed = 80f;
    public float bobHeight = 0.18f;
    public float bobSpeed = 2.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        BuildPickupVisual();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + Vector3.up * yOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = other.gameObject.AddComponent<PlayerInventory>();
        }

        inventory.AddPickup(pickupType, amount, displayName);
        Destroy(gameObject);
    }

    private void BuildPickupVisual()
    {
        Renderer ownRenderer = GetComponent<Renderer>();
        if (ownRenderer != null)
        {
            ownRenderer.enabled = false;
        }

        Transform old = transform.Find("PickupVisual");
        if (old != null)
        {
            Destroy(old.gameObject);
        }

        GameObject root = new GameObject("PickupVisual");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        switch (pickupType)
        {
            case PickupType.SecurityKeycard:
                BuildKeycard(root.transform);
                break;
            case PickupType.EnergyCore:
                BuildEnergyCore(root.transform);
                break;
            case PickupType.EmpDevice:
                BuildEmpGun(root.transform);
                break;
            case PickupType.EmpCharge:
                BuildEmpCharge(root.transform);
                break;
        }
    }

    private void BuildKeycard(Transform root)
    {
        Material card = CreateMaterial(new Color(0.05f, 0.45f, 1f), 0f);
        Material strip = CreateMaterial(new Color(1f, 0.86f, 0.12f), 0f);
        Material dark = CreateMaterial(new Color(0.02f, 0.02f, 0.025f), 0f);

        GameObject body = CreateCube("Keycard_Body", root, new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.045f, 0.48f), card);
        GameObject stripe = CreateCube("Keycard_GoldStrip", root, new Vector3(0f, 0.03f, -0.13f), new Vector3(0.62f, 0.02f, 0.07f), strip);
        GameObject chip = CreateCube("Keycard_Chip", root, new Vector3(-0.22f, 0.04f, 0.08f), new Vector3(0.14f, 0.025f, 0.13f), dark);
        body.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        stripe.transform.localRotation = Quaternion.identity;
        chip.transform.localRotation = Quaternion.identity;
    }

    private void BuildEnergyCore(Transform root)
    {
        Material core = CreateMaterial(new Color(0.1f, 0.9f, 1f), 2.2f);
        Material shell = CreateMaterial(new Color(0.05f, 0.08f, 0.12f), 0f);

        CreateSphere("EnergyCore_Glow", root, Vector3.zero, Vector3.one * 0.42f, core);
        CreateTorusLikeRing(root, shell, 0f);
        CreateTorusLikeRing(root, shell, 90f);

        GameObject lightObject = new GameObject("EnergyCore_Light");
        lightObject.transform.SetParent(root, false);
        lightObject.transform.localPosition = Vector3.zero;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.2f, 0.9f, 1f);
        light.intensity = 2.2f;
        light.range = 4f;
    }

    private void BuildEmpGun(Transform root)
    {
        Material body = CreateMaterial(new Color(0.12f, 0.14f, 0.18f), 0f);
        Material cyan = CreateMaterial(new Color(0.1f, 0.9f, 1f), 1.8f);
        Material grip = CreateMaterial(new Color(0.04f, 0.04f, 0.05f), 0f);

        CreateCube("EMP_Body", root, new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.26f, 0.28f), body);
        CreateCylinder("EMP_Barrel", root, new Vector3(0.52f, 0.02f, 0f), Quaternion.Euler(0f, 0f, 90f), new Vector3(0.12f, 0.45f, 0.12f), cyan);
        CreateCube("EMP_Handle", root, new Vector3(-0.18f, -0.3f, 0f), new Vector3(0.2f, 0.55f, 0.2f), grip);
        CreateSphere("EMP_Core", root, new Vector3(0.12f, 0.15f, 0f), Vector3.one * 0.18f, cyan);
    }

    private void BuildEmpCharge(Transform root)
    {
        Material shell = CreateMaterial(new Color(0.1f, 0.1f, 0.12f), 0f);
        Material charge = CreateMaterial(new Color(0.1f, 0.9f, 1f), 1.6f);
        CreateCylinder("EMPCharge_Cell", root, Vector3.zero, Quaternion.Euler(90f, 0f, 0f), new Vector3(0.22f, 0.55f, 0.22f), charge);
        CreateCube("EMPCharge_CapA", root, new Vector3(0f, 0f, 0.33f), new Vector3(0.32f, 0.32f, 0.08f), shell);
        CreateCube("EMPCharge_CapB", root, new Vector3(0f, 0f, -0.33f), new Vector3(0.32f, 0.32f, 0.08f), shell);
    }

    private void CreateTorusLikeRing(Transform root, Material mat, float yaw)
    {
        GameObject ringA = CreateCylinder("EnergyCore_RingA", root, Vector3.zero, Quaternion.Euler(90f, yaw, 0f), new Vector3(0.035f, 0.62f, 0.035f), mat);
        GameObject ringB = CreateCylinder("EnergyCore_RingB", root, Vector3.zero, Quaternion.Euler(90f, yaw + 90f, 0f), new Vector3(0.035f, 0.62f, 0.035f), mat);
    }

    private Material CreateMaterial(Color color, float emission)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        if (emission > 0f)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * emission);
        }
        return mat;
    }

    private GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        ApplyMatAndRemoveCollider(go, mat);
        return go;
    }

    private GameObject CreateSphere(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        ApplyMatAndRemoveCollider(go, mat);
        return go;
    }

    private GameObject CreateCylinder(string name, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;
        go.transform.localScale = localScale;
        ApplyMatAndRemoveCollider(go, mat);
        return go;
    }

    private void ApplyMatAndRemoveCollider(GameObject go, Material mat)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
        }

        Collider collider = go.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }
}
