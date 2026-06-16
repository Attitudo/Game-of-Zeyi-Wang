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
    private Renderer baseRenderer;
    private Renderer indicatorRenderer;
    private Transform leverTransform;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        CacheVisualParts();
        ApplyState();
    }

    private void CacheVisualParts()
    {
        Transform basePart = transform.Find("SwitchBase");
        if (basePart != null) baseRenderer = basePart.GetComponent<Renderer>();

        Transform leverPart = transform.Find("SwitchLever");
        if (leverPart != null)
        {
            leverTransform = leverPart;
            if (indicatorRenderer == null)
            {
                indicatorRenderer = leverPart.GetComponent<Renderer>();
            }
        }

        Transform indicatorPart = transform.Find("SwitchIndicator");
        if (indicatorPart != null)
        {
            indicatorRenderer = indicatorPart.GetComponent<Renderer>();
        }

        if (baseRenderer == null)
        {
            Renderer[] rs = GetComponentsInChildren<Renderer>();
            if (rs.Length > 0) baseRenderer = rs[0];
        }
    }

    private void Update()
    {
        if (player == null || GlobalMenuUI.GameplayBlocked)
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
        GameAudio.PlaySwitch();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(isOn ? "Power switch activated." : "Power switch deactivated.", 2f);
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

        if (baseRenderer != null)
        {
            baseRenderer.material.color = new Color(0.18f, 0.18f, 0.18f);
        }

        if (indicatorRenderer != null)
        {
            indicatorRenderer.material.color = isOn ? new Color(0.15f, 0.85f, 0.25f) : new Color(0.85f, 0.15f, 0.15f);
        }

        if (leverTransform != null)
        {
            // Use a sideways lever motion so the switch looks more like a real toggle.
            leverTransform.localRotation = Quaternion.Euler(0f, 0f, isOn ? -35f : 35f);
        }
    }

    private void UpdateSwitchVisualState()
    {
        Transform lever = transform.Find("SwitchLever");
        if (lever != null)
        {
            lever.localRotation = Quaternion.Euler(isOn ? 32f : -32f, 0f, 0f);
        }

        Transform indicator = transform.Find("SwitchIndicator");
        if (indicator != null)
        {
            Renderer r = indicator.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.color = isOn ? new Color(0.2f, 1.0f, 0.25f) : new Color(0.8f, 0.15f, 0.15f);
            }
        }
    }

    private void OnGUI()
    {
        if (player == null || !GlobalMenuUI.HelpVisible || GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= interactDistance)
        {
            string prompt = "Switch: press X to power the lamp emitter.";
            float w = Mathf.Min(470f, Screen.width * 0.58f);
            float h = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(prompt, w), 54f, 90f);
            CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - w / 2f, Screen.height - h - 92f, w, h), prompt);
        }
    }
}
