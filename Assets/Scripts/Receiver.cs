using UnityEngine;

public class Receiver : MonoBehaviour
{
    [Header("Receiver Settings")]
    public GameObject door;
    public bool powered;

    [Header("Receiver Lamp Visual")]
    public Color offCrystalColor = new Color(0.12f, 0.55f, 0.65f);
    public Color onCrystalColor = new Color(0.15f, 1.0f, 0.25f);
    public Color offLightColor = new Color(0.05f, 0.45f, 0.60f);
    public Color onLightColor = new Color(0.25f, 1.0f, 0.25f);

    private Renderer receiverRenderer;
    private Color defaultColor = Color.red;
    private Color poweredColor = Color.green;

    private Renderer crystalRenderer;
    private Renderer haloRenderer;
    private Light lampLight;

    private void Awake()
    {
        receiverRenderer = GetComponent<Renderer>();
        if (receiverRenderer != null)
        {
            defaultColor = receiverRenderer.material.color;
            receiverRenderer.enabled = false;
        }

        EnsureReceiverLampVisual();
        UpdateReceiverLampVisual(true);
    }

    private void Start()
    {
        EnsureReceiverLampVisual();
        UpdateReceiverLampVisual(true);
    }

    public void Activate()
    {
        SetPowered(true);
    }

    public void ForceRefreshVisual()
    {
        EnsureReceiverLampVisual();
        UpdateReceiverLampVisual(true);
    }

    public void SetPowered(bool value)
    {
        bool changed = powered != value;
        powered = value;

        if (changed && powered)
        {
            GameAudio.PlayReceiverPower();
        }

        if (door != null)
        {
            DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();
            if (doorAnimator != null)
            {
                if (powered)
                {
                    doorAnimator.OpenDoor();
                }
                else
                {
                    doorAnimator.CloseDoor();
                }
            }
            else
            {
                door.SetActive(!powered);
            }
        }

        if (receiverRenderer != null)
        {
            receiverRenderer.material.color = powered ? poweredColor : defaultColor;
        }

        EnsureReceiverLampVisual();
        UpdateReceiverLampVisual(false);

        Debug.Log(powered ? "Receiver powered: lamp on, door opened." : "Receiver lost power: lamp off, door closed.");
    }

    private void EnsureReceiverLampVisual()
    {
        Transform lamp = transform.Find("ReceiverLampModel");
        if (lamp == null)
        {
            GameObject lampObj = new GameObject("ReceiverLampModel");
            lamp = lampObj.transform;
            lamp.SetParent(transform, false);
        }

        lamp.localPosition = Vector3.zero;
        lamp.localRotation = Quaternion.identity;
        lamp.localScale = Vector3.one;
        lamp.gameObject.SetActive(true);

        GameObject basePart = FindOrCreatePrimitiveChild(lamp, "Receiver_Lamp_Base", PrimitiveType.Cylinder);
        basePart.transform.localPosition = new Vector3(0f, -0.78f, 0f);
        basePart.transform.localScale = new Vector3(0.55f, 0.15f, 0.55f);
        SetObjectColor(basePart, new Color(0.08f, 0.08f, 0.08f));

        GameObject pole = FindOrCreatePrimitiveChild(lamp, "Receiver_Lamp_Pole", PrimitiveType.Cylinder);
        pole.transform.localPosition = new Vector3(0f, -0.33f, 0f);
        pole.transform.localScale = new Vector3(0.08f, 0.38f, 0.08f);
        SetObjectColor(pole, new Color(0.10f, 0.10f, 0.10f));

        GameObject lampFrame = FindOrCreatePrimitiveChild(lamp, "Receiver_Lamp_Frame", PrimitiveType.Cube);
        lampFrame.transform.localPosition = new Vector3(0f, 0.13f, 0f);
        lampFrame.transform.localScale = new Vector3(0.70f, 0.12f, 0.70f);
        SetObjectColor(lampFrame, new Color(0.09f, 0.09f, 0.09f));

        GameObject crystal = FindOrCreatePrimitiveChild(lamp, "Receiver_Lamp_Crystal", PrimitiveType.Sphere);
        crystal.transform.localPosition = new Vector3(0f, 0.42f, 0f);
        crystal.transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);
        crystalRenderer = crystal.GetComponent<Renderer>();

        GameObject halo = FindOrCreatePrimitiveChild(lamp, "Receiver_Lamp_GlowHalo", PrimitiveType.Sphere);
        halo.transform.localPosition = new Vector3(0f, 0.42f, 0f);
        halo.transform.localScale = new Vector3(0.78f, 0.78f, 0.78f);
        haloRenderer = halo.GetComponent<Renderer>();

        if (lampLight == null)
        {
            Transform oldLight = lamp.Find("Receiver_Lamp_PointLight");
            if (oldLight != null)
            {
                lampLight = oldLight.GetComponent<Light>();
            }
        }

        if (lampLight == null)
        {
            GameObject lightObj = new GameObject("Receiver_Lamp_PointLight");
            lightObj.transform.SetParent(lamp, false);
            lightObj.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            lampLight = lightObj.AddComponent<Light>();
            lampLight.type = LightType.Point;
        }
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
            Destroy(c);
        }

        return part;
    }

    private void UpdateReceiverLampVisual(bool force)
    {
        Color crystalColor = powered ? onCrystalColor : offCrystalColor;
        Color haloColor = powered ? new Color(0.20f, 1.0f, 0.25f, 0.45f) : new Color(0.04f, 0.18f, 0.22f, 0.18f);

        if (crystalRenderer != null)
        {
            crystalRenderer.enabled = true;
            crystalRenderer.material.color = crystalColor;
            crystalRenderer.material.SetColor("_EmissionColor", powered ? onCrystalColor * 1.8f : offCrystalColor * 0.25f);
            crystalRenderer.material.EnableKeyword("_EMISSION");
        }

        if (haloRenderer != null)
        {
            haloRenderer.enabled = powered;
            haloRenderer.material.color = haloColor;
            haloRenderer.material.SetColor("_EmissionColor", powered ? onLightColor * 1.2f : Color.black);
            haloRenderer.material.EnableKeyword("_EMISSION");
        }

        if (lampLight != null)
        {
            lampLight.enabled = true;
            lampLight.color = powered ? onLightColor : offLightColor;
            lampLight.intensity = powered ? 4.2f : 0.22f;
            lampLight.range = powered ? 7.0f : 2.0f;
        }
    }

    private void SetObjectColor(GameObject obj, Color color)
    {
        if (obj == null)
        {
            return;
        }

        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            r.enabled = true;
            r.material.color = color;
        }
    }
}
