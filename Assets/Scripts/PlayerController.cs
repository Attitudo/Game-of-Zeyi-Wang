using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Tooltip("Disabled for this puzzle game to prevent camera/wall clipping and unnecessary vertical movement.")]
    public bool allowJump = false;
    public float jumpForce = 0f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 185f;

    [Header("Camera Collision")]
    public bool preventCameraWallClipping = true;
    public float cameraCollisionRadius = 0.18f;
    public float cameraWallPadding = 0.16f;
    public float cameraAnchorHeight = 1.25f;

    private Rigidbody rb;
    private float xRotation;
    private Vector3 movement;
    private bool isGrounded;
    private Camera playerCamera;
    private Vector3 desiredCameraLocalPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // The game no longer needs jumping. Locking Z rotation and keeping gravity is enough;
        // jump input is ignored unless allowJump is manually enabled in the Inspector.
        playerCamera = Camera.main;
        if (playerCamera != null)
        {
            desiredCameraLocalPosition = playerCamera.transform.localPosition;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (ShouldBlockPlayerControl())
        {
            movement = Vector3.zero;
            if (rb != null)
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
            return;
        }

        HandleMouseLook();
        HandleMovementInput();

        if (allowJump && Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void LateUpdate()
    {
        PreventCameraWallClipping();
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (ShouldBlockPlayerControl())
        {
            movement = Vector3.zero;
            if (rb != null)
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
            return;
        }

        MovePlayer();
    }

    private bool ShouldBlockPlayerControl()
    {
        if (GlobalMenuUI.GameplayBlocked)
        {
            return true;
        }

        if (GameManager.Instance != null && (GameManager.Instance.playerCaught || GameManager.Instance.levelCompleted))
        {
            return true;
        }

        return false;
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = (transform.right * horizontal + transform.forward * vertical).normalized;
    }

    private void MovePlayer()
    {
        Vector3 targetVelocity = movement * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void PreventCameraWallClipping()
    {
        if (!preventCameraWallClipping || playerCamera == null)
        {
            return;
        }

        // Restore the intended third-person camera position first.
        // Then shorten it only when a wall/ceiling is between the player and the camera.
        playerCamera.transform.localPosition = desiredCameraLocalPosition;

        Vector3 anchor = transform.position + Vector3.up * cameraAnchorHeight;
        Vector3 desiredWorld = playerCamera.transform.position;
        Vector3 toCamera = desiredWorld - anchor;
        float distance = toCamera.magnitude;

        if (distance < 0.05f)
        {
            return;
        }

        Vector3 direction = toCamera / distance;
        RaycastHit[] hits = Physics.SphereCastAll(anchor, cameraCollisionRadius, direction, distance, ~0, QueryTriggerInteraction.Ignore);

        if (hits == null || hits.Length == 0)
        {
            return;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            string n = hit.collider.gameObject.name.ToLowerInvariant();
            if (n.Contains("player") || n.Contains("mirror") || n.Contains("receiver") || n.Contains("lightsource"))
            {
                continue;
            }

            float safeDistance = Mathf.Max(0.25f, hit.distance - cameraWallPadding);
            playerCamera.transform.position = anchor + direction * safeDistance;
            return;
        }
    }
}
