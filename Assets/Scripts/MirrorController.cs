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

        MovableMirrorRail rail = GetComponent<MovableMirrorRail>();
        playerInRange = rail != null
            ? rail.IsThisTheActiveRailForPlayer(player)
            : IsClosestStandaloneMirror();
        if (!playerInRange || GlobalMenuUI.GameplayBlocked)
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

    private bool IsClosestStandaloneMirror()
    {
        if (player == null)
        {
            return false;
        }

        MirrorController[] mirrors = Object.FindObjectsOfType<MirrorController>();
        MirrorController best = null;
        float bestDistance = float.MaxValue;

        foreach (MirrorController mirror in mirrors)
        {
            if (mirror == null || !mirror.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (mirror.GetComponent<MovableMirrorRail>() != null)
            {
                continue;
            }

            float distance = Vector3.Distance(mirror.transform.position, player.position);
            if (distance <= mirror.interactDistance && distance < bestDistance)
            {
                bestDistance = distance;
                best = mirror;
            }
        }

        return best == this;
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
        if (!playerInRange || !GlobalMenuUI.HelpVisible || GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        string prompt = "Mirror control: press Q / E to rotate the mirror and redirect the beam.";
        float w = Mathf.Min(500f, Screen.width * 0.62f);
        float h = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(prompt, w), 56f, 95f);
        CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - w / 2f, Screen.height - h - 36f, w, h), prompt);
    }
}
