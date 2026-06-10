using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    [Header("Door Movement")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);
    public float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool initialized;
    private bool openCommanded;

    public bool IsOpen
    {
        get { return openCommanded; }
    }

    private void Start()
    {
        InitializeIfNeeded();
    }

    private void Update()
    {
        InitializeIfNeeded();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    public void OpenDoor()
    {
        InitializeIfNeeded();
        openCommanded = true;
        targetPosition = closedPosition + openOffset;
    }

    public void CloseDoor()
    {
        InitializeIfNeeded();
        openCommanded = false;
        targetPosition = closedPosition;
    }

    public void SnapClosedAtCurrentPosition()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
        initialized = true;
        openCommanded = false;
    }

    private void InitializeIfNeeded()
    {
        if (initialized)
        {
            return;
        }

        closedPosition = transform.position;
        targetPosition = closedPosition;
        initialized = true;
        openCommanded = false;
    }
}
