using UnityEngine;

public enum PickupType
{
    SecurityKeycard,
    EnergyCore,
    EmpDevice,
    EmpCharge
}

public class PickupItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public PickupType pickupType = PickupType.SecurityKeycard;
    public int amount = 1;
    public string displayName = "Item";

    [Header("Visual Motion")]
    public float rotationSpeed = 80f;
    public float bobHeight = 0.18f;
    public float bobSpeed = 2.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + Vector3.up * yOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = other.gameObject.AddComponent<PlayerInventory>();
        }

        inventory.AddPickup(pickupType, amount, displayName);
        Destroy(gameObject);
    }
}
