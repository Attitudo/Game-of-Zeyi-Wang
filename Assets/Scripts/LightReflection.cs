using UnityEngine;

public class LightReflection : MonoBehaviour
{
    public static LightReflection Instance;

    [Header("Light Settings")]
    public LineRenderer lineRenderer;
    public Light sourceLight;
    public int maxReflections = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        RefreshLightPath();
    }

    public void RefreshLightPath()
    {
        Vector3 currentPosition = sourceLight.transform.position;
        Vector3 currentDirection = sourceLight.transform.forward;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        for (int i = 0; i < maxReflections; i++)
        {
            if (Physics.Raycast(currentPosition, currentDirection, out RaycastHit hit))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                    currentPosition = hit.point + currentDirection * 0.01f;
                }
                else if (hit.collider.CompareTag("Receiver"))
                {
                    hit.collider.GetComponent<Receiver>().Activate();
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition + currentDirection * 50);
                return;
            }
        }
    }
}