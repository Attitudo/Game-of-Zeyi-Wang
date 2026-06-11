using UnityEngine;

public class GuardAI : MonoBehaviour
{
    private enum GuardState
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float waypointReachDistance = 0.3f;

    [Header("Vision")]
    public float viewDistance = 8f;
    [Range(10f, 180f)] public float viewAngle = 75f;
    public float catchDistance = 1.2f;
    public float searchTime = 2.5f;

    [Header("Vision Cone Visual")]
    public bool showVisionCone = true;
    public int visionConeSegments = 24;
    public float visionConeHeight = 0.04f;
    public Color patrolConeColor = new Color(1f, 0.85f, 0f, 0.22f);
    public Color chaseConeColor = new Color(1f, 0f, 0f, 0.30f);
    public Color stunnedConeColor = new Color(0f, 1f, 1f, 0.20f);

    [Header("Collision")]
    public float guardHeight = 1.8f;
    public float guardRadius = 0.38f;
    public float gravity = -18f;

    [Header("EMP Stun")]
    public float stunTimer = 0f;

    private int currentPatrolIndex;
    private GuardState state = GuardState.Patrol;
    private float searchTimer;
    private float verticalVelocity;
    private Vector3 lastKnownPlayerPosition;

    [Header("Wall Avoidance")]
    public float obstacleAvoidanceDistance = 1.1f;
    public float obstacleAvoidanceRadius = 0.32f;
    public float stuckAdvanceTime = 0.75f;

    private Vector3 lastStuckCheckPosition;
    private float stuckTimer;
    private bool lastMoveWasBlocked;
    private Renderer guardRenderer;
    private CharacterController characterController;
    private Mesh visionConeMesh;
    private MeshRenderer visionConeRenderer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        characterController.height = guardHeight;
        characterController.radius = guardRadius;
        characterController.center = Vector3.zero;
        characterController.stepOffset = 0.3f;
        characterController.slopeLimit = 45f;
        characterController.skinWidth = 0.05f;
        characterController.detectCollisions = true;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        guardRenderer = GetComponentInChildren<Renderer>();
        SetColor(Color.blue);
        CreateVisionConeVisual();
        lastStuckCheckPosition = transform.position;
    }

    private void Update()
    {
        UpdateVisionConeVisual();

        if (player == null)
        {
            return;
        }

        if (GameManager.Instance != null && (GameManager.Instance.playerCaught || GameManager.Instance.levelCompleted))
        {
            return;
        }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            ApplyGravityOnly();
            SetColor(Color.cyan);
            return;
        }

        if (CanSeePlayer())
        {
            state = GuardState.Chase;
            lastKnownPlayerPosition = player.position;
            SetColor(Color.red);
        }

        switch (state)
        {
            case GuardState.Patrol:
                Patrol();
                break;
            case GuardState.Chase:
                Chase();
                break;
            case GuardState.Search:
                Search();
                break;
        }
    }

    public string CurrentStateText
    {
        get
        {
            if (stunTimer > 0f) return "STUNNED";
            if (state == GuardState.Chase) return "ALERT";
            if (state == GuardState.Search) return "SEARCH";
            return "PATROL";
        }
    }

    private void OnGUI()
    {
        if (!GlobalMenuUI.HelpVisible || GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 screen = cam.WorldToScreenPoint(transform.position + Vector3.up * 2.1f);
        if (screen.z <= 0f)
        {
            return;
        }

        string label = "Guard: " + CurrentStateText;
        float w = 130f;
        float h = 34f;
        CartoonGUI.DrawCenterBox(new Rect(screen.x - w / 2f, Screen.height - screen.y - h / 2f, w, h), label);
    }

    public void Stun(float seconds)
    {
        stunTimer = Mathf.Max(stunTimer, seconds);
        searchTimer = seconds;
        state = GuardState.Search;
        SetColor(Color.cyan);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage("EMP hit: guard stunned for " + seconds.ToString("0") + " seconds.", 2f);
        }
    }

    private void Patrol()
    {
        SetColor(Color.blue);
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            ApplyGravityOnly();
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        MoveTowards(target.position, patrolSpeed);

        if (lastMoveWasBlocked)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            return;
        }

        Vector3 flatCurrent = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(target.position.x, 0f, target.position.z);
        if (Vector3.Distance(flatCurrent, flatTarget) <= waypointReachDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void Chase()
    {
        MoveTowards(player.position, chaseSpeed);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= catchDistance)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }
            return;
        }

        if (!CanSeePlayer())
        {
            state = GuardState.Search;
            searchTimer = searchTime;
            lastKnownPlayerPosition = player.position;
            SetColor(Color.yellow);
        }
    }

    private void Search()
    {
        MoveTowards(lastKnownPlayerPosition, patrolSpeed);
        searchTimer -= Time.deltaTime;

        Vector3 flatCurrent = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(lastKnownPlayerPosition.x, 0f, lastKnownPlayerPosition.z);
        if (searchTimer <= 0f || Vector3.Distance(flatCurrent, flatTarget) <= waypointReachDistance)
        {
            state = GuardState.Patrol;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * 0.75f;
        Vector3 playerTarget = player.position + Vector3.up * 0.75f;
        Vector3 directionToPlayer = playerTarget - eyePosition;
        float distance = directionToPlayer.magnitude;

        if (distance > viewDistance)
        {
            return false;
        }

        float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);
        if (angle > viewAngle * 0.5f)
        {
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(eyePosition, directionToPlayer.normalized, viewDistance, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponentInParent<GuardAI>() == this)
            {
                continue;
            }

            if (hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerController>() != null)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        lastMoveWasBlocked = false;

        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        Vector3 direction = flatTarget - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            ApplyGravityOnly();
            return;
        }

        Vector3 desiredDirection = direction.normalized;
        Vector3 steeredDirection = GetWallAwareDirection(desiredDirection);
        if (steeredDirection.sqrMagnitude < 0.001f)
        {
            steeredDirection = desiredDirection;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(steeredDirection), 10f * Time.deltaTime);

        Vector3 before = transform.position;
        Vector3 horizontalMove = steeredDirection * speed * Time.deltaTime;
        MoveWithCollision(horizontalMove);
        UpdateStuckDetection(before, target);
    }

    private Vector3 GetWallAwareDirection(Vector3 desiredDirection)
    {
        Vector3 origin = transform.position + Vector3.up * 0.75f;
        RaycastHit[] hits = Physics.SphereCastAll(origin, obstacleAvoidanceRadius, desiredDirection, obstacleAvoidanceDistance, ~0, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0)
        {
            return desiredDirection;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.GetComponentInParent<GuardAI>() == this)
            {
                continue;
            }

            if (hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerController>() != null)
            {
                continue;
            }

            string n = hit.collider.gameObject.name.ToLowerInvariant();
            if (n.Contains("floor") || n.Contains("ceiling") || n.Contains("rail") || n.Contains("mirror") || n.Contains("receiver"))
            {
                continue;
            }

            Vector3 slide = Vector3.ProjectOnPlane(desiredDirection, hit.normal);
            slide.y = 0f;

            if (slide.sqrMagnitude < 0.001f)
            {
                slide = Vector3.Cross(Vector3.up, hit.normal);
            }

            if (Vector3.Dot(slide, desiredDirection) < 0f)
            {
                slide = -slide;
            }

            slide.y = 0f;
            if (slide.sqrMagnitude < 0.001f)
            {
                return -hit.normal;
            }

            return Vector3.Slerp(desiredDirection, slide.normalized, 0.85f).normalized;
        }

        return desiredDirection;
    }

    private void UpdateStuckDetection(Vector3 beforeMove, Vector3 target)
    {
        Vector3 flatBefore = new Vector3(beforeMove.x, 0f, beforeMove.z);
        Vector3 flatAfter = new Vector3(transform.position.x, 0f, transform.position.z);
        float moved = Vector3.Distance(flatBefore, flatAfter);

        Vector3 flatTarget = new Vector3(target.x, 0f, target.z);
        float distanceToTarget = Vector3.Distance(flatAfter, flatTarget);

        if (moved < 0.015f && distanceToTarget > waypointReachDistance * 2f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckAdvanceTime)
        {
            lastMoveWasBlocked = true;
            stuckTimer = 0f;
        }

        lastStuckCheckPosition = transform.position;
    }

    private void MoveWithCollision(Vector3 horizontalMove)
    {
        if (characterController != null && characterController.enabled)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 finalMove = horizontalMove + Vector3.up * verticalVelocity * Time.deltaTime;
            characterController.Move(finalMove);
        }
        else
        {
            transform.position += horizontalMove;
        }
    }

    private void ApplyGravityOnly()
    {
        if (characterController != null && characterController.enabled)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }
    }

    private void SetColor(Color color)
    {
        if (guardRenderer != null)
        {
            guardRenderer.material.color = color;
        }
    }

    private void CreateVisionConeVisual()
    {
        if (!showVisionCone)
        {
            return;
        }

        Transform existing = transform.Find("VisionConeVisual");
        GameObject coneObject = existing != null ? existing.gameObject : new GameObject("VisionConeVisual");
        coneObject.transform.SetParent(transform);
        coneObject.transform.localPosition = Vector3.zero;
        coneObject.transform.localRotation = Quaternion.identity;
        coneObject.transform.localScale = Vector3.one;

        MeshFilter meshFilter = coneObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = coneObject.AddComponent<MeshFilter>();
        }

        visionConeRenderer = coneObject.GetComponent<MeshRenderer>();
        if (visionConeRenderer == null)
        {
            visionConeRenderer = coneObject.AddComponent<MeshRenderer>();
        }

        visionConeMesh = new Mesh();
        visionConeMesh.name = "Guard Vision Cone Mesh";
        meshFilter.mesh = visionConeMesh;

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        Material material = new Material(shader);
        material.color = patrolConeColor;
        material.renderQueue = 3000;
        visionConeRenderer.material = material;
        visionConeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        visionConeRenderer.receiveShadows = false;
    }

    private void UpdateVisionConeVisual()
    {
        if (!showVisionCone)
        {
            if (visionConeRenderer != null)
            {
                visionConeRenderer.enabled = false;
            }
            return;
        }

        if (visionConeMesh == null || visionConeRenderer == null)
        {
            CreateVisionConeVisual();
        }

        if (visionConeMesh == null || visionConeRenderer == null)
        {
            return;
        }

        visionConeRenderer.enabled = true;

        Color coneColor = patrolConeColor;
        if (stunTimer > 0f)
        {
            coneColor = stunnedConeColor;
        }
        else if (state == GuardState.Chase)
        {
            coneColor = chaseConeColor;
        }

        visionConeRenderer.material.color = coneColor;

        int segments = Mathf.Max(6, visionConeSegments);
        Vector3[] vertices = new Vector3[segments + 2];
        // Use double-sided triangles so the cone is visible from normal camera angles.
        int[] triangles = new int[segments * 6];

        vertices[0] = new Vector3(0f, visionConeHeight, 0f);
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = -viewAngle * 0.5f + viewAngle * t;
            float radians = angle * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Sin(radians) * viewDistance, visionConeHeight, Mathf.Cos(radians) * viewDistance);
        }

        for (int i = 0; i < segments; i++)
        {
            int triangleIndex = i * 6;
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = i + 2;

            triangles[triangleIndex + 3] = 0;
            triangles[triangleIndex + 4] = i + 2;
            triangles[triangleIndex + 5] = i + 1;
        }

        visionConeMesh.Clear();
        visionConeMesh.vertices = vertices;
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
        visionConeMesh.RecalculateBounds();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
