using UnityEngine;

public class Receiver : MonoBehaviour
{
    [Header("Receiver Settings")]
    public GameObject door;
    public bool powered;

    private Renderer receiverRenderer;
    private Color defaultColor = Color.red;
    private Color poweredColor = Color.green;

    private void Awake()
    {
        receiverRenderer = GetComponent<Renderer>();
        if (receiverRenderer != null)
        {
            defaultColor = receiverRenderer.material.color;
        }
    }

    public void Activate()
    {
        SetPowered(true);
    }

    public void SetPowered(bool value)
    {
        if (powered == value)
        {
            return;
        }

        powered = value;

        if (door != null)
        {
            DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();
            if (doorAnimator != null)
            {
                if (powered)
                {
                    doorAnimator.OpenDoor();
                }
                else
                {
                    doorAnimator.CloseDoor();
                }
            }
            else
            {
                door.SetActive(!powered);
            }
        }

        if (receiverRenderer != null)
        {
            receiverRenderer.material.color = powered ? poweredColor : defaultColor;
        }

        Debug.Log(powered ? "Receiver powered: door opened." : "Receiver lost power: door closed.");
    }
}
