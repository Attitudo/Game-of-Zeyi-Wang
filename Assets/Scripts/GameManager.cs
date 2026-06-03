using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level State")]
    public bool levelCompleted;
    public bool playerCaught;

    [Header("UI Text")]
    public string customObjectiveTitle = "";
    [TextArea(3, 6)] public string customObjectiveBody = "";
    [TextArea(2, 4)] public string customCompleteMessage = "";

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

    public void ConfigureObjective(string title, string body, string completeMessage)
    {
        customObjectiveTitle = title;
        customObjectiveBody = body;
        customCompleteMessage = completeMessage;
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
        CompleteLevel("");
    }

    public void CompleteLevel(string message)
    {
        if (playerCaught || levelCompleted)
        {
            return;
        }

        levelCompleted = true;

        if (!string.IsNullOrEmpty(message))
        {
            temporaryMessage = message;
        }
        else if (!string.IsNullOrEmpty(customCompleteMessage))
        {
            temporaryMessage = customCompleteMessage;
        }
        else
        {
            temporaryMessage = "Level complete. Press R to replay.";
        }

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
        string title = customObjectiveTitle;
        string body = customObjectiveBody;

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(body))
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Level02")
            {
                title = "LEVEL 2 OBJECTIVE:";
                body =
                    "1. Use two mirrors to redirect the beam into the receiver.\n" +
                    "2. The door opens only while the receiver is powered.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, R restart after win/loss.";
            }
            else
            {
                title = "LEVEL 1 OBJECTIVE:";
                body =
                    "1. Approach the mirror and press Q / E to rotate it.\n" +
                    "2. Reflect the yellow beam into the red receiver to open the door.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, R restart after win/loss.";
            }
        }

        GUI.Box(new Rect(15f, 15f, 620f, 112f), title + "\n" + body);

        if (messageTimer > 0f && !string.IsNullOrEmpty(temporaryMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 300f, Screen.height / 2f - 48f, 600f, 96f), temporaryMessage);
        }
    }
}
