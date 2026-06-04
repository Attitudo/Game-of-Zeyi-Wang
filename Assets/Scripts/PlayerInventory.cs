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
        string status =
            "INVENTORY\n" +
            "Keycards: " + keycards + "\n" +
            "Energy Cores: " + energyCores + "\n" +
            "EMP Device: " + (hasEmpDevice ? "Yes" : "No") + "\n" +
            "EMP Charges: " + empCharges;

        GUI.Box(new Rect(Screen.width - 220f, 15f, 200f, 112f), status);

        if (hasEmpDevice)
        {
            GUI.Box(new Rect(Screen.width - 260f, Screen.height - 72f, 240f, 48f), "Weapon: Press F to fire EMP stun.");
        }

        if (pickupMessageTimer > 0f && !string.IsNullOrEmpty(pickupMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 230f, 145f, 460f, 42f), pickupMessage);
        }
    }
}
