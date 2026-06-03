using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    [Header("Scene Transition")]
    public string nextSceneName = "Level02";
    public bool isFinalLevel = false;

    [Header("Final Level Message")]
    [TextArea(2, 4)] public string finalLevelMessage = "All levels complete. Press R to replay this level.";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        triggered = true;

        if (isFinalLevel)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel(finalLevelMessage);
            }
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
