using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private Vector3 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("[Lifecycle] Awake: Player initialized");
    }

    private void Start()
    {
        Debug.Log("[Lifecycle] Start: Game started");
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = transform.right * horizontal + transform.forward * vertical;
        movement.Normalize();
    }

    private void MovePlayer()
    {
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
    }
}