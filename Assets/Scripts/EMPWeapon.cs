using UnityEngine;

public class EMPWeapon : MonoBehaviour
{
    [Header("EMP Settings")]
    public float range = 14f;
    public float cooldown = 1.2f;
    public float stunDuration = 5f;
    public float aimRadius = 0.7f;
    public float aimAssistAngle = 8f;
    public KeyCode fireKey = KeyCode.F;

    [Header("Crosshair")]
    public bool showCrosshair = true;
    public float crosshairSize = 18f;
    public float crosshairThickness = 2f;

    private float cooldownTimer;
    private PlayerInventory inventory;
    private Camera playerCamera;
    private Transform empGunVisual;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        playerCamera = Camera.main;
        FindEmpGunVisual();
        UpdateEmpGunVisual();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        UpdateEmpGunVisual();

        if (Input.GetKeyDown(fireKey))
        {
            Fire();
        }
    }

    private void FindEmpGunVisual()
    {
        if (empGunVisual != null)
        {
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera != null)
        {
            empGunVisual = playerCamera.transform.Find("EMP_Blaster_Visual");
        }
    }

    private void UpdateEmpGunVisual()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        FindEmpGunVisual();

        if (empGunVisual != null)
        {
            empGunVisual.gameObject.SetActive(inventory != null && inventory.hasEmpDevice);
        }
    }

    private void Fire()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        if (inventory == null || !inventory.hasEmpDevice)
        {
            ShowMessage("You need to pick up the EMP device first.");
            return;
        }

        if (cooldownTimer > 0f)
        {
            return;
        }

        if (inventory.empCharges <= 0)
        {
            ShowMessage("No EMP charges left.");
            return;
        }

        cooldownTimer = cooldown;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        Vector3 origin = playerCamera != null ? playerCamera.transform.position : transform.position + Vector3.up * 0.8f;
        Vector3 direction = playerCamera != null ? playerCamera.transform.forward : transform.forward;

        Debug.DrawRay(origin, direction * range, Color.cyan, 0.9f);

        GuardAI guard = FindGuardInAim(origin, direction);
        if (guard != null)
        {
            inventory.ConsumeEmpCharge();
            guard.Stun(stunDuration);
            ShowMessage("EMP hit: guard stunned.");
            return;
        }

        inventory.ConsumeEmpCharge();
        ShowMessage("EMP missed. Aim at the guard with the center crosshair.");
    }

    private GuardAI FindGuardInAim(Vector3 origin, Vector3 direction)
    {
        GuardAI bestGuard = null;
        float bestDistance = float.MaxValue;

        RaycastHit[] sphereHits = Physics.SphereCastAll(origin, aimRadius, direction, range, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(sphereHits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in sphereHits)
        {
            GuardAI guard = hit.collider.GetComponentInParent<GuardAI>();
            if (guard != null)
            {
                return guard;
            }
        }

        GuardAI[] guards = Object.FindObjectsOfType<GuardAI>();
        foreach (GuardAI guard in guards)
        {
            if (guard == null || !guard.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 guardTarget = guard.transform.position + Vector3.up * 0.75f;
            Vector3 toGuard = guardTarget - origin;
            float distance = toGuard.magnitude;
            if (distance > range)
            {
                continue;
            }

            float angle = Vector3.Angle(direction, toGuard.normalized);
            if (angle > aimAssistAngle)
            {
                continue;
            }

            if (!HasClearLineToGuard(origin, guardTarget, guard, distance))
            {
                continue;
            }

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestGuard = guard;
            }
        }

        return bestGuard;
    }

    private bool HasClearLineToGuard(Vector3 origin, Vector3 target, GuardAI guard, float distance)
    {
        Vector3 direction = (target - origin).normalized;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponentInParent<GuardAI>() == guard)
            {
                return true;
            }

            if (hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerController>() != null)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private void ShowMessage(string message)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(message, 2f);
        }
        else
        {
            Debug.Log(message);
        }
    }

    private void OnGUI()
    {
        if (!showCrosshair)
        {
            return;
        }

        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        if (inventory == null || !inventory.hasEmpDevice)
        {
            return;
        }

        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;
        float gap = 5f;
        float length = crosshairSize;

        Color oldColor = GUI.color;
        GUI.color = Color.cyan;

        GUI.DrawTexture(new Rect(centerX - length - gap, centerY - crosshairThickness * 0.5f, length, crosshairThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX + gap, centerY - crosshairThickness * 0.5f, length, crosshairThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - crosshairThickness * 0.5f, centerY - length - gap, crosshairThickness, length), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - crosshairThickness * 0.5f, centerY + gap, crosshairThickness, length), Texture2D.whiteTexture);

        GUI.color = oldColor;
        GUI.Box(new Rect(centerX - 90f, centerY + 42f, 180f, 32f), "EMP: F | Charges: " + inventory.empCharges);
    }
}
