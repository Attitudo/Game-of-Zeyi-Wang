using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}
