using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    [Header("Door Movement")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);
    public float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool initialized;

    private void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    public void OpenDoor()
    {
        if (!initialized)
        {
            closedPosition = transform.position;
            initialized = true;
        }

        targetPosition = closedPosition + openOffset;
    }

    public void CloseDoor()
    {
        if (!initialized)
        {
            closedPosition = transform.position;
            initialized = true;
        }

        targetPosition = closedPosition;
    }
}
