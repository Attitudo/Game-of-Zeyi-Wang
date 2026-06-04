using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("Laser Switch")]
    public LightReflection targetLaser;
    public Light sourceLight;
    public bool isOn = false;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.X;

    private Transform player;
    private Renderer switchRenderer;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        switchRenderer = GetComponentInChildren<Renderer>();
        ApplyState();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= interactDistance && Input.GetKeyDown(interactKey))
        {
            ToggleLaser();
        }
    }

    public void ToggleLaser()
    {
        isOn = !isOn;
        ApplyState();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(isOn ? "Laser switch activated." : "Laser switch deactivated.", 2f);
        }
    }

    private void ApplyState()
    {
        if (targetLaser != null)
        {
            targetLaser.SetLaserEnabled(isOn);
        }

        if (sourceLight != null)
        {
            sourceLight.enabled = isOn;
        }

        if (switchRenderer != null)
        {
            switchRenderer.material.color = isOn ? Color.green : Color.red;
        }
    }

    private void OnGUI()
    {
        if (player == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= interactDistance)
        {
            GUI.Box(new Rect(Screen.width / 2f - 210f, Screen.height - 145f, 420f, 42f), "Laser switch: press X to toggle the emitter.");
        }
    }
}
