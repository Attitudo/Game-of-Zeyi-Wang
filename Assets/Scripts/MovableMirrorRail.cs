using UnityEngine;

public class MovableMirrorRail : MonoBehaviour
{
    [Header("Rail Movement")]
    public Vector3 railDirection = Vector3.right;
    public Vector3 correctPosition;
    public float railRange = 4.5f;
    public float slideSpeed = 2.0f;
    public float interactDistance = 3.2f;
    public float snapDistance = 0.12f;
    public KeyCode slideBackwardKey = KeyCode.Z;
    public KeyCode slideForwardKey = KeyCode.C;

    [Header("Initial Puzzle Offset")]
    [Tooltip("Mirror starts away from the solution point so the player must slide it into place.")]
    public bool startAwayFromCorrectPosition = true;

    [Tooltip("Initial offset along the rail. Use positive/negative values to place mirrors at different rail ends.")]
    public float initialStartOffset = 4.0f;

    [Header("Visual Feedback")]
    public GameObject railVisual;
    public Color normalColor = new Color(0.35f, 0.35f, 0.35f);
    public Color activeColor = new Color(1.0f, 0.68f, 0.18f);

    private Transform player;
    private Renderer railRenderer;
    private bool playerInRange;
    private bool hasAppliedInitialOffset;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        NormalizeRailData();
        ApplyInitialOffsetIfNeeded();
        CreateRailVisualIfNeeded();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (GlobalMenuUI.GameplayBlocked)
        {
            playerInRange = false;
            KeepRailNormalColor();
            return;
        }

        // Only the nearest controllable rail should react.
        // This prevents several mirrors from moving at the same time when interaction areas overlap.
        playerInRange = IsThisTheActiveRailForPlayer(player);
        if (playerInRange)
        {
            float input = 0f;
            if (Input.GetKey(slideBackwardKey)) input -= 1f;
            if (Input.GetKey(slideForwardKey)) input += 1f;

            if (Mathf.Abs(input) > 0.01f)
            {
                Slide(input * slideSpeed * Time.deltaTime);
            }
        }

        KeepRailNormalColor();
    }

    public bool IsPlayerNearControlArea(Transform target)
    {
        if (target == null)
        {
            return false;
        }

        NormalizeRailData();

        Vector3 closest = GetClosestPointOnRail(target.position);
        float distanceToRail = Vector3.Distance(new Vector3(target.position.x, 0f, target.position.z), new Vector3(closest.x, 0f, closest.z));
        float distanceToMirror = Vector3.Distance(new Vector3(target.position.x, 0f, target.position.z), new Vector3(transform.position.x, 0f, transform.position.z));

        // Add a small margin so the player can stand near the rail and still pull the mirror back from an end.
        return distanceToRail <= interactDistance || distanceToMirror <= interactDistance;
    }

    public bool IsThisTheActiveRailForPlayer(Transform target)
    {
        MovableMirrorRail activeRail = GetActiveRailForPlayer(target);
        return activeRail == this;
    }

    public static MovableMirrorRail GetActiveRailForPlayer(Transform target)
    {
        if (target == null)
        {
            return null;
        }

        MovableMirrorRail[] rails = Object.FindObjectsOfType<MovableMirrorRail>();
        MovableMirrorRail best = null;
        float bestScore = float.MaxValue;

        foreach (MovableMirrorRail rail in rails)
        {
            if (rail == null || !rail.gameObject.activeInHierarchy)
            {
                continue;
            }

            float score = rail.GetInteractionPriorityScore(target);
            if (score < bestScore)
            {
                bestScore = score;
                best = rail;
            }
        }

        return best;
    }

    public float GetInteractionPriorityScore(Transform target)
    {
        if (target == null)
        {
            return float.MaxValue;
        }

        NormalizeRailData();

        Vector3 flatTarget = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 closest = GetClosestPointOnRail(target.position);
        Vector3 flatClosest = new Vector3(closest.x, 0f, closest.z);
        Vector3 flatMirror = new Vector3(transform.position.x, 0f, transform.position.z);

        float distanceToRail = Vector3.Distance(flatTarget, flatClosest);
        float distanceToMirror = Vector3.Distance(flatTarget, flatMirror);

        if (distanceToRail > interactDistance && distanceToMirror > interactDistance)
        {
            return float.MaxValue;
        }

        // Prefer the closest mirror, but still allow rail-end recovery.
        // This avoids selecting multiple mirrors from overlapping long rail ranges.
        return Mathf.Min(distanceToMirror, distanceToRail + 0.75f);
    }

    private Vector3 GetClosestPointOnRail(Vector3 worldPosition)
    {
        NormalizeRailData();

        Vector3 flatPoint = new Vector3(worldPosition.x, correctPosition.y, worldPosition.z);
        Vector3 relative = flatPoint - correctPosition;
        float along = Vector3.Dot(relative, railDirection);
        along = Mathf.Clamp(along, -railRange, railRange);
        return correctPosition + railDirection * along;
    }

    public void ResetMirrorToStartOffset()
    {
        hasAppliedInitialOffset = false;
        NormalizeRailData();
        ApplyInitialOffsetIfNeeded();
        CreateRailVisualIfNeeded();
    }

    private void NormalizeRailData()
    {
        railDirection.y = 0f;
        if (railDirection.sqrMagnitude < 0.001f)
        {
            railDirection = Vector3.right;
        }
        railDirection.Normalize();

        if (correctPosition == Vector3.zero)
        {
            correctPosition = transform.position;
        }

        railRange = Mathf.Max(railRange, 1.0f);
    }

    private void ApplyInitialOffsetIfNeeded()
    {
        if (hasAppliedInitialOffset || !startAwayFromCorrectPosition)
        {
            return;
        }

        hasAppliedInitialOffset = true;

        float offset = Mathf.Clamp(initialStartOffset, -railRange * 0.95f, railRange * 0.95f);
        if (Mathf.Abs(offset) < snapDistance * 5f)
        {
            offset = railRange * 0.85f;
        }

        Vector3 relative = transform.position - correctPosition;
        Vector3 perpendicular = relative - railDirection * Vector3.Dot(relative, railDirection);

        // Start near one end of the track, not at the solution point.
        transform.position = correctPosition + perpendicular + railDirection * offset;

        if (LightReflection.Instance != null)
        {
            LightReflection.Instance.RefreshLightPath();
        }
    }

    private void Slide(float distance)
    {
        Vector3 relative = transform.position - correctPosition;
        float currentAlongRail = Vector3.Dot(relative, railDirection);
        float nextAlongRail = Mathf.Clamp(currentAlongRail + distance, -railRange, railRange);

        Vector3 perpendicular = relative - railDirection * currentAlongRail;
        Vector3 newPosition = correctPosition + perpendicular + railDirection * nextAlongRail;

        if (Mathf.Abs(nextAlongRail) <= snapDistance)
        {
            newPosition = correctPosition + perpendicular;
        }

        transform.position = newPosition;

        if (LightReflection.Instance != null)
        {
            LightReflection.Instance.RefreshLightPath();
        }
    }

    private void CreateRailVisualIfNeeded()
    {
        if (railVisual != null)
        {
            railRenderer = railVisual.GetComponent<Renderer>();
            railVisual.transform.position = correctPosition + new Vector3(0f, -1.05f, 0f);
            railVisual.transform.rotation = Quaternion.LookRotation(railDirection, Vector3.up);
            railVisual.transform.localScale = new Vector3(0.18f, 0.06f, railRange * 2.2f);
            KeepRailNormalColor();
            return;
        }

        GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rail.name = gameObject.name + "_RailTrack";
        rail.transform.position = correctPosition + new Vector3(0f, -1.05f, 0f);
        rail.transform.rotation = Quaternion.LookRotation(railDirection, Vector3.up);
        rail.transform.localScale = new Vector3(0.18f, 0.06f, railRange * 2.2f);

        Collider col = rail.GetComponent<Collider>();
        if (col != null)
        {
            Object.Destroy(col);
        }

        railRenderer = rail.GetComponent<Renderer>();
        if (railRenderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = normalColor;
            railRenderer.material = mat;
        }

        railVisual = rail;
        KeepRailNormalColor();
    }

    private void KeepRailNormalColor()
    {
        if (railRenderer == null)
        {
            return;
        }

        // No green "correct position" hint. Active orange only shows the currently selected rail.
        railRenderer.material.color = playerInRange ? activeColor : normalColor;
    }

    private void OnGUI()
    {
        if (!playerInRange || !GlobalMenuUI.HelpVisible || GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        string prompt = "Mirror rail: hold Z / C to slide the mirror along the track. Use Q / E to rotate it.";
        float w = Mathf.Min(590f, Screen.width * 0.68f);
        float h = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(prompt, w), 64f, 110f);
        CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - w / 2f, Screen.height - h - 92f, w, h), prompt);
    }
}
