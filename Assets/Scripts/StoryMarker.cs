using UnityEngine;

public class StoryMarker : MonoBehaviour
{
    [TextArea]
    public string message = "Story log";
    public float showDuration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(message, showDuration);
        }
    }
}
