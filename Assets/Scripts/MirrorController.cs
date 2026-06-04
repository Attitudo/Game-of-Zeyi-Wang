using UnityEngine;

public class MirrorController : MonoBehaviour
{
    [Header("Mirror Settings")]
    public float rotateStep = 5f;
    public float interactDistance = 3f;
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    private Transform player;
    private bool playerInRange;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        playerInRange = Vector3.Distance(transform.position, player.position) <= interactDistance;
        if (!playerInRange)
        {
            return;
        }

        if (Input.GetKeyDown(rotateLeftKey))
        {
            RotateMirror(-rotateStep);
        }

        if (Input.GetKeyDown(rotateRightKey))
        {
            RotateMirror(rotateStep);
        }
    }

    private void RotateMirror(float angle)
    {
        transform.Rotate(0f, angle, 0f, Space.World);

        if (LightReflection.Instance != null)
        {
            LightReflection.Instance.RefreshLightPath();
        }
    }

    private void OnGUI()
    {
        if (!playerInRange)
        {
            return;
        }

        GUI.Box(new Rect(Screen.width / 2f - 220f, Screen.height - 90f, 440f, 45f), "Mirror control: press Q / E to rotate the mirror and redirect the beam.");
    }
}
