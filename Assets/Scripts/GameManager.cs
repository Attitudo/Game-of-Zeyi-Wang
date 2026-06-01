using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level State")]
    public bool levelCompleted;
    public bool playerCaught;

    private float messageTimer;
    private string temporaryMessage = "";

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
        if ((playerCaught || levelCompleted) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
        }
    }

    public void PlayerCaught()
    {
        if (levelCompleted || playerCaught)
        {
            return;
        }

        playerCaught = true;
        temporaryMessage = "You were spotted by the security guard. Press R to restart.";
        messageTimer = 999f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CompleteLevel()
    {
        if (playerCaught || levelCompleted)
        {
            return;
        }

        levelCompleted = true;
        temporaryMessage = "Level 1 complete. You redirected the beam and escaped. Press R to replay.";
        messageTimer = 999f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowMessage(string message, float seconds = 3f)
    {
        temporaryMessage = message;
        messageTimer = seconds;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(15f, 15f, 560f, 105f),
            "LEVEL 1 OBJECTIVE:\n" +
            "1. Approach the mirror and press Q / E to rotate it.\n" +
            "2. Reflect the yellow beam into the red receiver to open the door.\n" +
            "3. Avoid the AI guard and reach the green exit zone.\n" +
            "Controls: WASD move, Mouse look, Space jump, R restart after win/loss.");

        if (messageTimer > 0f && !string.IsNullOrEmpty(temporaryMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 280f, Screen.height / 2f - 45f, 560f, 90f), temporaryMessage);
        }
    }
}
