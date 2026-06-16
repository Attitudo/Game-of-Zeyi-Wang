using UnityEngine;

public static class StableCharacterVisuals
{
    private const string PlayerModelPath = "StableCharacters/StableHuman";
    private const string GuardModelPath = "StableCharacters/StableOrc";

    public static GameObject ApplyPlayer(GameObject root)
    {
        return Apply(root, "Stable_Player_Model", PlayerModelPath, 1.75f, false, 0f);
    }

    public static GameObject ApplyGuard(GameObject root)
    {
        return Apply(root, "Stable_Guard_Model", GuardModelPath, 1.85f, true, 0f);
    }

    private static GameObject Apply(GameObject root, string childName, string resourcePath, float targetHeight, bool isGuard, float visualYaw)
    {
        if (root == null)
        {
            return null;
        }

        Transform existing = root.transform.Find(childName);
        if (existing != null)
        {
            HideLegacyCharacterRenderers(root, childName);
            return existing.gameObject;
        }

        GameObject source = Resources.Load<GameObject>(resourcePath);
        if (source == null)
        {
            Debug.LogWarning("Stable character model not found at Resources/" + resourcePath + ". Keeping the original capsule visible.", root);
            return null;
        }

        GameObject wrapper = new GameObject(childName);
        wrapper.transform.SetParent(root.transform, false);
        wrapper.transform.localPosition = Vector3.zero;
        wrapper.transform.localRotation = Quaternion.Euler(0f, visualYaw, 0f);
        wrapper.transform.localScale = Vector3.one;

        GameObject model = Object.Instantiate(source, wrapper.transform);
        model.name = "Model";
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        Renderer[] renderers = wrapper.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            Object.Destroy(wrapper);
            Debug.LogWarning("Stable character model loaded but no renderer was found. Keeping the original capsule visible.", root);
            return null;
        }

        ForceMaterials(wrapper, isGuard);
        NormalizeToRoot(root, wrapper, targetHeight);
        HideCapsuleRenderer(root, childName);

        StableCharacterMotion motion = wrapper.AddComponent<StableCharacterMotion>();
        motion.owner = root.transform;
        motion.isGuard = isGuard;
        return wrapper;
    }

    private static void ForceMaterials(GameObject wrapper, bool isGuard)
    {
        Renderer[] renderers = wrapper.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = isGuard ? new Color(0.25f, 0.36f, 0.85f) : new Color(0.82f, 0.72f, 0.55f);
                renderer.material = mat;
            }
            renderer.enabled = true;
        }
    }

    private static void NormalizeToRoot(GameObject root, GameObject wrapper, float targetHeight)
    {
        Bounds b = GetBounds(wrapper);
        if (b.size.y > 0.001f)
        {
            float scale = targetHeight / b.size.y;
            scale = Mathf.Clamp(scale, 0.001f, 100f);
            wrapper.transform.localScale *= scale;
        }

        b = GetBounds(wrapper);
        float desiredFeetY = root.transform.position.y - 1f;
        wrapper.transform.position += new Vector3(0f, desiredFeetY - b.min.y, 0f);

        b = GetBounds(wrapper);
        wrapper.transform.position += new Vector3(root.transform.position.x - b.center.x, 0f, root.transform.position.z - b.center.z);
    }

    private static Bounds GetBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        bool found = false;
        Bounds bounds = new Bounds(root.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || !renderer.enabled)
            {
                continue;
            }

            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        return bounds;
    }

    private static void HideCapsuleRenderer(GameObject root, string visualName)
    {
        HideLegacyCharacterRenderers(root, visualName);
    }

    private static void HideLegacyCharacterRenderers(GameObject root, string visualName)
    {
        if (root == null)
        {
            return;
        }

        Renderer renderer = root.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        foreach (Transform child in root.transform)
        {
            if (child == null)
            {
                continue;
            }

            string lower = child.name.ToLowerInvariant();

            // Keep the real imported/stable model, camera, ground check, and weapon holder.
            if (child.name == visualName ||
                lower.Contains("stable_") ||
                lower.Contains("main camera") ||
                lower.Contains("camera") ||
                lower.Contains("groundcheck") ||
                lower.Contains("emp_blaster") ||
                lower.Contains("procedural_"))
            {
                continue;
            }

            bool looksLikeOldProceduralCharacter =
                lower.Contains("cartoonmodel") ||
                lower.Contains("procedural") ||
                lower.Contains("capsule") ||
                lower.Contains("body") ||
                lower.Contains("head") ||
                lower.Contains("arm") ||
                lower.Contains("leg") ||
                lower.Contains("player_") ||
                lower.Contains("guard_") ||
                lower.Contains("visual");

            if (looksLikeOldProceduralCharacter)
            {
                Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer childRenderer in renderers)
                {
                    if (childRenderer != null)
                    {
                        childRenderer.enabled = false;
                    }
                }
            }
        }
    }
}

public class StableCharacterMotion : MonoBehaviour
{
    public Transform owner;
    public bool isGuard;

    private Rigidbody ownerRigidbody;
    private CharacterController ownerController;
    private Vector3 lastOwnerPosition;
    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private float cycle;

    private Transform leftFoot;
    private Transform rightFoot;
    private Material footMaterial;

    private void Start()
    {
        if (owner == null && transform.parent != null)
        {
            owner = transform.parent;
        }

        if (owner != null)
        {
            ownerRigidbody = owner.GetComponent<Rigidbody>();
            ownerController = owner.GetComponent<CharacterController>();
            lastOwnerPosition = owner.position;
        }

        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;

        CreateStepFeet();
    }

    private void LateUpdate()
    {
        float speed = GetSpeed();
        bool moving = speed > 0.15f;
        cycle += Time.deltaTime * (moving ? Mathf.Lerp(4f, 10f, Mathf.Clamp01(speed / 5f)) : 1.4f);

        float bob = moving ? Mathf.Abs(Mathf.Sin(cycle * 2f)) * 0.055f : Mathf.Sin(cycle) * 0.008f;
        float yawSwing = moving ? Mathf.Sin(cycle) * 5.0f : Mathf.Sin(cycle) * 0.8f;
        float pitchLean = moving ? Mathf.Sin(cycle * 2f) * 1.8f : 0f;

        transform.localPosition = baseLocalPosition + new Vector3(0f, bob, 0f);
        transform.localRotation = baseLocalRotation * Quaternion.Euler(pitchLean, yawSwing, moving ? Mathf.Sin(cycle) * 1.3f : 0f);

        UpdateStepFeet(moving, speed);
    }


    private void CreateStepFeet()
    {
        if (owner == null)
        {
            return;
        }

        footMaterial = new Material(Shader.Find("Standard"));
        footMaterial.color = isGuard ? new Color(0.05f, 0.08f, 0.08f) : new Color(0.06f, 0.05f, 0.04f);

        leftFoot = CreateFoot("Procedural_LeftStepFoot", -0.16f);
        rightFoot = CreateFoot("Procedural_RightStepFoot", 0.16f);
    }

    private Transform CreateFoot(string name, float x)
    {
        Transform existing = owner.Find(name);
        if (existing != null)
        {
            return existing;
        }

        GameObject foot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        foot.name = name;
        foot.transform.SetParent(owner, false);
        foot.transform.localPosition = new Vector3(x, -0.92f, 0.10f);
        foot.transform.localRotation = Quaternion.identity;
        foot.transform.localScale = new Vector3(0.18f, 0.07f, 0.32f);

        Renderer r = foot.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = footMaterial;
        }

        Collider c = foot.GetComponent<Collider>();
        if (c != null)
        {
            Destroy(c);
        }

        return foot.transform;
    }

    private void UpdateStepFeet(bool moving, float speed)
    {
        if (owner == null || leftFoot == null || rightFoot == null)
        {
            return;
        }

        float stride = moving ? Mathf.Lerp(0.12f, 0.28f, Mathf.Clamp01(speed / 5f)) : 0f;
        float lift = moving ? 0.08f : 0f;

        float leftPhase = Mathf.Sin(cycle);
        float rightPhase = Mathf.Sin(cycle + Mathf.PI);

        leftFoot.localPosition = new Vector3(-0.16f, -0.92f + Mathf.Max(0f, leftPhase) * lift, 0.10f + leftPhase * stride);
        rightFoot.localPosition = new Vector3(0.16f, -0.92f + Mathf.Max(0f, rightPhase) * lift, 0.10f + rightPhase * stride);
    }

    private float GetSpeed()
    {
        if (ownerRigidbody != null)
        {
            Vector3 v = ownerRigidbody.velocity;
            v.y = 0f;
            return v.magnitude;
        }

        if (ownerController != null)
        {
            Vector3 v = ownerController.velocity;
            v.y = 0f;
            return v.magnitude;
        }

        if (owner != null)
        {
            Vector3 delta = owner.position - lastOwnerPosition;
            delta.y = 0f;
            lastOwnerPosition = owner.position;
            return delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        return 0f;
    }
}
