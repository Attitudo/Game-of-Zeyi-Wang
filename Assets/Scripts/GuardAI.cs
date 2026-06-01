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

    private int currentPatrolIndex;
    private GuardState state = GuardState.Patrol;
    private float searchTimer;
    private Vector3 lastKnownPlayerPosition;
    private Renderer guardRenderer;

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
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (GameManager.Instance != null && (GameManager.Instance.playerCaught || GameManager.Instance.levelCompleted))
        {
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

    private void Patrol()
    {
        SetColor(Color.blue);
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        MoveTowards(target.position, patrolSpeed);

        if (Vector3.Distance(transform.position, target.position) <= waypointReachDistance)
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
            SetColor(Color.yellow);
        }
    }

    private void Search()
    {
        MoveTowards(lastKnownPlayerPosition, patrolSpeed);
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0f || Vector3.Distance(transform.position, lastKnownPlayerPosition) <= waypointReachDistance)
        {
            state = GuardState.Patrol;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * 0.6f;
        Vector3 playerTarget = player.position + Vector3.up * 0.6f;
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

        if (Physics.Raycast(eyePosition, directionToPlayer.normalized, out RaycastHit hit, viewDistance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 movement = direction.normalized * speed * Time.deltaTime;
        transform.position += movement;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 10f * Time.deltaTime);
    }

    private void SetColor(Color color)
    {
        if (guardRenderer != null)
        {
            guardRenderer.material.color = color;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
