using UnityEngine;

public class MirrorController : MonoBehaviour
{
    public float rotateSpeed = 45f;
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(Vector3.up, rotateSpeed);
            LightReflection.Instance.RefreshLightPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }*/
}