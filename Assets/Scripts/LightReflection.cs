using System.Collections.Generic;
using UnityEngine;

public class LightReflection : MonoBehaviour
{
    public static LightReflection Instance;

    [Header("Light Settings")]
    public LineRenderer lineRenderer;
    public Light sourceLight;
    public int maxReflections = 6;
    public float maxDistance = 50f;
    public float surfaceOffset = 0.05f;

    [Header("Receiver Detection")]
    [Tooltip("Extra tolerance used when the beam visually touches the receiver but the exact ray misses the small collider.")]
    public float receiverActivationRadius = 0.75f;

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

    public void RefreshLightPath()
    {
        if (lineRenderer == null || sourceLight == null)
        {
            return;
        }

        receiversHitThisFrame.Clear();

        Vector3 currentPosition = sourceLight.transform.position;
        Vector3 currentDirection = sourceLight.transform.forward.normalized;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        for (int i = 0; i < maxReflections; i++)
        {
            bool hasHit = Physics.Raycast(
                currentPosition,
                currentDirection,
                out RaycastHit hit,
                maxDistance,
                ~0,
                QueryTriggerInteraction.Collide);

            Vector3 segmentEnd = hasHit ? hit.point : currentPosition + currentDirection * maxDistance;
            AddLinePoint(segmentEnd);
            ActivateReceiversNearBeamSegment(currentPosition, segmentEnd);

            if (!hasHit)
            {
                break;
            }

            if (IsMirror(hit.collider))
            {
                currentDirection = Vector3.Reflect(currentDirection, hit.normal).normalized;
                currentPosition = hit.point + currentDirection * surfaceOffset;
                continue;
            }

            Receiver hitReceiver = hit.collider.GetComponentInParent<Receiver>();
            if (hitReceiver != null || hit.collider.CompareTag("Receiver"))
            {
                if (hitReceiver == null)
                {
                    hitReceiver = hit.collider.GetComponent<Receiver>();
                }

                if (hitReceiver != null)
                {
                    PowerReceiver(hitReceiver);
                }
            }

            break;
        }

        TurnOffReceiversNotHit();
    }

    private bool IsMirror(Collider collider)
    {
        if (collider == null)
        {
            return false;
        }

        if (collider.CompareTag("Mirror"))
        {
            return true;
        }

        MirrorController mirrorController = collider.GetComponentInParent<MirrorController>();
        return mirrorController != null;
    }

    private void ActivateReceiversNearBeamSegment(Vector3 start, Vector3 end)
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
                PowerReceiver(receiver);
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
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.08f;
        lineRenderer.endWidth = 0.08f;

        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }
}
