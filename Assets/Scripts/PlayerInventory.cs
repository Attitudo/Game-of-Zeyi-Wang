using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int keycards = 0;
    public int energyCores = 0;
    public bool hasEmpDevice = false;
    public int empCharges = 0;

    private float pickupMessageTimer;
    private string pickupMessage = "";

    public void AddPickup(PickupType type, int amount, string displayName)
    {
        switch (type)
        {
            case PickupType.SecurityKeycard:
                keycards += amount;
                ShowPickupMessage("Picked up: " + displayName);
                break;
            case PickupType.EnergyCore:
                energyCores += amount;
                ShowPickupMessage("Picked up: " + displayName + " (" + energyCores + ")");
                break;
            case PickupType.EmpDevice:
                hasEmpDevice = true;
                empCharges += Mathf.Max(1, amount);
                ShowPickupMessage("Picked up: EMP Device. Press F to stun guards.");
                break;
            case PickupType.EmpCharge:
                empCharges += amount;
                ShowPickupMessage("Picked up: EMP Charge +" + amount);
                break;
        }
    }

    public bool ConsumeEmpCharge()
    {
        if (!hasEmpDevice || empCharges <= 0)
        {
            return false;
        }

        empCharges--;
        return true;
    }

    public void ShowPickupMessage(string message)
    {
        pickupMessage = message;
        pickupMessageTimer = 3f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(message, 2.5f);
        }
    }

    private void Update()
    {
        if (pickupMessageTimer > 0f)
        {
            pickupMessageTimer -= Time.deltaTime;
        }
    }

    private void OnGUI()
    {
        if (GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        int reflected = 0;
        int required = 0;
        if (LightReflection.Instance != null)
        {
            reflected = LightReflection.Instance.CurrentMirrorReflections;
            required = LightReflection.Instance.RequiredMirrorReflections;
        }

        string status =
            "<color=#FFD45A>COUNTER</color>\n" +
            "Keycards: " + keycards + "\n" +
            "Energy Cores: " + energyCores + "\n" +
            "EMP Device: " + (hasEmpDevice ? "Yes" : "No") + "\n" +
            "EMP Charges: " + empCharges + "\n" +
            "<color=#8DFF8D>Reflections: " + reflected + " / " + required + "</color>";

        float counterWidth = 265f;
        float counterHeight = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(status, counterWidth, true), 150f, 205f);
        CartoonGUI.DrawSmallBox(new Rect(Screen.width - counterWidth - 15f, 15f, counterWidth, counterHeight), status);

        if (hasEmpDevice && GlobalMenuUI.HelpVisible)
        {
            string weaponPrompt = "Weapon: Press F to fire EMP stun.";
            float w = 270f;
            float h = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(weaponPrompt, w, true), 50f, 80f);
            CartoonGUI.DrawSmallBox(new Rect(Screen.width - w - 15f, Screen.height - h - 15f, w, h), weaponPrompt);
        }

    }
}
