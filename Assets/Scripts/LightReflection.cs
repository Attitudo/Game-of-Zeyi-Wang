using System.Collections.Generic;
using UnityEngine;

public class LightReflection : MonoBehaviour
{
    public static LightReflection Instance;

    [Header("Light Settings")]
    public LineRenderer lineRenderer;
    public Light sourceLight;
    public int maxReflections = 8;
    public float maxDistance = 60f;
    public float surfaceOffset = 0.05f;
    public bool laserEnabled = true;

    [Header("Laser Collision")]
    [Tooltip("Walls, cover boxes, barriers and closed doors block the laser. Mirror surfaces reflect it. Receivers consume it.")]
    public bool obstaclesBlockLaser = true;

    [Header("Receiver Detection")]
    [Tooltip("Extra tolerance used when the beam visually touches the receiver but the exact ray misses the small collider.")]
    public float receiverActivationRadius = 1.0f;

    [Header("Puzzle Rule")]
    [Tooltip("Receiver only powers on when the beam has reflected from exactly this many different mirrors.")]
    public int requiredMirrorReflections = 1;

    [Tooltip("If true, extra mirror reflections will not count as a valid solution. This keeps decoy mirrors misleading instead of useful.")]
    public bool requireExactMirrorReflectionCount = true;

    private int mirrorReflectionsThisPath;
    private readonly HashSet<int> mirrorsHitThisPath = new HashSet<int>();

    public int CurrentMirrorReflections
    {
        get { return mirrorReflectionsThisPath; }
    }

    public int RequiredMirrorReflections
    {
        get { return requiredMirrorReflections; }
    }

    private readonly HashSet<Receiver> receiversHitThisFrame = new HashSet<Receiver>();
    private readonly List<Receiver> receiversPoweredLastFrame = new List<Receiver>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    private void Start()
    {
        if (sourceLight == null)
        {
            GameObject lightObject = GameObject.Find("LightSource");
            if (lightObject != null)
            {
                sourceLight = lightObject.GetComponent<Light>();
            }
        }

        ConfigureLineRenderer();
        RefreshLightPath();
    }

    private void Update()
    {
        RefreshLightPath();
    }

    public void SetLaserEnabled(bool enabled)
    {
        laserEnabled = enabled;

        if (sourceLight != null)
        {
            sourceLight.enabled = enabled;
        }

        if (!laserEnabled)
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }

            foreach (Receiver receiver in Object.FindObjectsOfType<Receiver>())
            {
                if (receiver != null)
                {
                    receiver.SetPowered(false);
                }
            }

            receiversHitThisFrame.Clear();
            receiversPoweredLastFrame.Clear();
            mirrorsHitThisPath.Clear();
            mirrorReflectionsThisPath = 0;
        }
    }

    public void RefreshLightPath()
    {
        if (lineRenderer == null || sourceLight == null)
        {
            return;
        }

        if (!laserEnabled)
        {
            lineRenderer.positionCount = 0;
            TurnOffReceiversNotHit();
            return;
        }

        receiversHitThisFrame.Clear();
        mirrorsHitThisPath.Clear();
        mirrorReflectionsThisPath = 0;

        Vector3 currentPosition = sourceLight.transform.position;
        Vector3 currentDirection = sourceLight.transform.forward.normalized;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        for (int i = 0; i < maxReflections; i++)
        {
            bool hasHit = FindFirstLaserHit(currentPosition, currentDirection, out RaycastHit hit);
            Vector3 segmentEnd = hasHit ? hit.point : currentPosition + currentDirection * maxDistance;

            AddLinePoint(segmentEnd);
            ActivateReceiversNearBeamSegment(currentPosition, segmentEnd, mirrorReflectionsThisPath);

            if (!hasHit)
            {
                break;
            }

            if (IsMirrorSurface(hit.collider))
            {
                RegisterMirrorReflection(hit.collider);
                currentDirection = Vector3.Reflect(currentDirection, hit.normal).normalized;
                currentPosition = hit.point + currentDirection * surfaceOffset;
                continue;
            }

            Receiver hitReceiver = GetReceiver(hit.collider);
            if (hitReceiver != null)
            {
                TryPowerReceiver(hitReceiver, mirrorReflectionsThisPath);
            }

            // Receiver or obstacle: the laser stops here.
            break;
        }

        TurnOffReceiversNotHit();
    }

    private bool FindFirstLaserHit(Vector3 origin, Vector3 direction, out RaycastHit selectedHit)
    {
        selectedHit = default;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance, ~0, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0)
        {
            return false;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit candidate in hits)
        {
            Collider candidateCollider = candidate.collider;
            if (candidateCollider == null)
            {
                continue;
            }

            if (candidateCollider.transform == transform || candidateCollider.transform.IsChildOf(transform))
            {
                continue;
            }

            // Mirror and receiver colliders are allowed to be triggers.
            // Check them BEFORE the generic trigger ignore rule, otherwise mirror
            // trigger colliders are skipped and the laser cannot reflect.
            if (IsMirrorSurface(candidateCollider) || GetReceiver(candidateCollider) != null)
            {
                selectedHit = candidate;
                return true;
            }

            // Triggers such as ExitZone, pickup ranges, UI helpers and guard vision
            // cones should not block the beam.
            if (candidateCollider.isTrigger)
            {
                continue;
            }

            if (IsIgnoredLaserObject(candidateCollider))
            {
                continue;
            }

            if (obstaclesBlockLaser && IsLaserBlocker(candidateCollider))
            {
                selectedHit = candidate;
                return true;
            }
        }

        return false;
    }

    private bool IsMirrorSurface(Collider collider)
    {
        if (collider == null)
        {
            return false;
        }

        // Only the reflective surface object should have the Mirror tag. Frame,
        // base and stand objects are decoration and are ignored by the beam.
        if (collider.CompareTag("Mirror"))
        {
            return true;
        }

        MirrorController mirrorController = collider.GetComponent<MirrorController>();
        return mirrorController != null;
    }

    private bool IsLaserBlocker(Collider collider)
    {
        string objectName = collider.gameObject.name.ToLowerInvariant();
        string parentName = collider.transform.parent != null ? collider.transform.parent.name.ToLowerInvariant() : string.Empty;
        string fullName = objectName + " " + parentName;

        // These are the real puzzle blockers. The intended solution should route
        // the beam around these using mirrors.
        if (fullName.Contains("wall") ||
            fullName.Contains("cover") ||
            fullName.Contains("crate") ||
            fullName.Contains("box") ||
            fullName.Contains("obstacle") ||
            fullName.Contains("barrier") ||
            fullName.Contains("securitydoor") ||
            fullName.Contains("doorpanel"))
        {
            return true;
        }

        return false;
    }

    private bool IsIgnoredLaserObject(Collider collider)
    {
        string objectName = collider.gameObject.name.ToLowerInvariant();
        string parentName = collider.transform.parent != null ? collider.transform.parent.name.ToLowerInvariant() : string.Empty;
        string fullName = objectName + " " + parentName;

        if (collider.GetComponentInParent<PlayerController>() != null ||
            collider.GetComponentInParent<GuardAI>() != null ||
            collider.GetComponentInParent<PickupItem>() != null ||
            collider.GetComponentInParent<LaserSwitch>() != null)
        {
            return true;
        }

        // Floors and decoration are not part of the optical puzzle. This prevents
        // tiny frame/pedestal objects from accidentally stopping a good solution.
        if (fullName.Contains("floor") ||
            fullName.Contains("ground") ||
            fullName.Contains("frame") ||
            fullName.Contains("base") ||
            fullName.Contains("stand") ||
            fullName.Contains("pedestal") ||
            fullName.Contains("marker") ||
            fullName.Contains("patrolpoint") ||
            fullName.Contains("visioncone"))
        {
            return true;
        }

        return false;
    }

    private Receiver GetReceiver(Collider collider)
    {
        if (collider == null)
        {
            return null;
        }

        Receiver receiver = collider.GetComponentInParent<Receiver>();
        if (receiver != null)
        {
            return receiver;
        }

        if (collider.CompareTag("Receiver"))
        {
            return collider.GetComponent<Receiver>();
        }

        return null;
    }

    private void ActivateReceiversNearBeamSegment(Vector3 start, Vector3 end, int mirrorCountForThisSegment)
    {
        Receiver[] receivers = Object.FindObjectsOfType<Receiver>();
        foreach (Receiver receiver in receivers)
        {
            if (receiver == null || !receiver.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 closestPoint = GetClosestPointOnSegment(start, end, receiver.transform.position);
            float distance = Vector3.Distance(closestPoint, receiver.transform.position);
            if (distance <= receiverActivationRadius)
            {
                TryPowerReceiver(receiver, mirrorCountForThisSegment);
            }
        }
    }

    private Vector3 GetClosestPointOnSegment(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 segment = end - start;
        float segmentLengthSquared = segment.sqrMagnitude;
        if (segmentLengthSquared <= 0.0001f)
        {
            return start;
        }

        float t = Vector3.Dot(point - start, segment) / segmentLengthSquared;
        t = Mathf.Clamp01(t);
        return start + segment * t;
    }

    private void RegisterMirrorReflection(Collider mirrorCollider)
    {
        if (mirrorCollider == null)
        {
            return;
        }

        Transform mirrorRoot = mirrorCollider.transform;
        MirrorController controller = mirrorCollider.GetComponentInParent<MirrorController>();
        if (controller != null)
        {
            mirrorRoot = controller.transform;
        }

        int id = mirrorRoot.GetInstanceID();
        if (mirrorsHitThisPath.Add(id))
        {
            mirrorReflectionsThisPath = mirrorsHitThisPath.Count;
        }
    }

    private void TryPowerReceiver(Receiver receiver, int mirrorCount)
    {
        bool validCount = requireExactMirrorReflectionCount
            ? mirrorCount == requiredMirrorReflections
            : mirrorCount >= requiredMirrorReflections;

        if (!validCount)
        {
            // Beam reached the receiver with the wrong mirror count.
            // Too few mirrors is too easy; too many mirrors means the player used a decoy route.
            return;
        }

        PowerReceiver(receiver);
    }

    private void PowerReceiver(Receiver receiver)
    {
        receiver.SetPowered(true);
        receiversHitThisFrame.Add(receiver);
    }

    private void AddLinePoint(Vector3 point)
    {
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);
    }

    private void TurnOffReceiversNotHit()
    {
        for (int i = receiversPoweredLastFrame.Count - 1; i >= 0; i--)
        {
            Receiver receiver = receiversPoweredLastFrame[i];
            if (receiver == null || receiversHitThisFrame.Contains(receiver))
            {
                continue;
            }

            receiver.SetPowered(false);
        }

        receiversPoweredLastFrame.Clear();
        receiversPoweredLastFrame.AddRange(receiversHitThisFrame);
    }

    private void ConfigureLineRenderer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.06f;
        lineRenderer.endWidth = 0.06f;
        lineRenderer.positionCount = 0;

        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
    }
}
