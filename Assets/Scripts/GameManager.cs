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
        ApplyDungeonAtmosphere();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyDungeonAtmosphere()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.10f, 0.09f, 0.08f);
        RenderSettings.fogDensity = 0.018f;
        RenderSettings.ambientLight = new Color(0.18f, 0.14f, 0.10f);
        RenderSettings.ambientIntensity = 0.65f;
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
            if (sceneName == "Level06")
            {
                title = "LEVEL 6 - FINAL MIRROR CATACOMB:";
                body =
                    "1. Collect the Master Keycard, three Energy Cores, and the EMP Device.\n" +
                    "2. Solve the six-mirror laser route around multiple blockers.\n" +
                    "3. Use EMP carefully to survive the final guard patrols.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level05")
            {
                title = "LEVEL 5 - SEALED CRYPT MAZE:";
                body =
                    "1. Collect the Crypt Keycard, two Energy Cores, and the EMP Device.\n" +
                    "2. Route the laser through five mirrors; direct paths are blocked.\n" +
                    "3. Open the door and escape through the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level04")
            {
                title = "LEVEL 4 - ANCIENT CORE CHAMBER:";
                body =
                    "1. Collect two Energy Cores and the Master Keycard.\n" +
                    "2. Use limited EMP charges against two AI guards.\n" +
                    "3. Solve the four-mirror laser path and reach the next dungeon gate.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level03")
            {
                title = "LEVEL 3 - GUARD VAULT:";
                body =
                    "1. Pick up the EMP Device and Security Keycard.\n" +
                    "2. Use F to stun the guard when needed.\n" +
                    "3. Solve the three-mirror path and reach the exit.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level02")
            {
                title = "LEVEL 2 - DUNGEON SWITCH ROOM:";
                body =
                    "1. Use two mirrors to redirect the beam into the receiver.\n" +
                    "2. The door opens only while the receiver is powered.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, X toggle laser, Q/E rotate mirrors, R restart after win/loss.";
            }
            else
            {
                title = "LEVEL 1 - CASTLE ENTRANCE:";
                body =
                    "1. Approach the mirror and press Q / E to rotate it.\n" +
                    "2. Reflect the yellow beam into the red receiver to open the door.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, R restart after win/loss.";
            }
        }

        GUI.Box(new Rect(15f, 15f, 620f, 112f), title + "\n" + body);

        if (messageTimer > 0f && !string.IsNullOrEmpty(temporaryMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 300f, Screen.height / 2f - 48f, 600f, 96f), temporaryMessage);
        }
    }
}
