using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextSceneName = "Level02";
    public bool isFinalLevel = false;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        if (!isFinalLevel)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Final level complete.");
        }
    }
}