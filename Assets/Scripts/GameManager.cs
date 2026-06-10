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
                    "2. The receiver requires exactly 6 unique mirror reflections before it can power on.\n" +
                    "3. Use EMP carefully to survive the final guard patrols.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, Z/C slide rail mirrors, X laser switch, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level05")
            {
                title = "LEVEL 5 - SEALED CRYPT MAZE:";
                body =
                    "1. Collect the Crypt Keycard, two Energy Cores, and the EMP Device.\n" +
                    "2. The receiver requires exactly 5 unique mirror reflections before it can power on.\n" +
                    "3. Open the door and escape through the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, Z/C slide rail mirrors, X laser switch, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level04")
            {
                title = "LEVEL 4 - ANCIENT CORE CHAMBER:";
                body =
                    "1. Collect two Energy Cores and the Master Keycard.\n" +
                    "2. Use limited EMP charges against two AI guards.\n" +
                    "3. The receiver requires exactly 4 unique mirror reflections before it can power on.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, Z/C slide rail mirrors, X laser switch, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level03")
            {
                title = "LEVEL 3 - GUARD VAULT:";
                body =
                    "1. Pick up the EMP Device and Security Keycard.\n" +
                    "2. Use F to stun the guard when needed.\n" +
                    "3. The receiver requires exactly 3 unique mirror reflections before it can power on.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, Z/C slide rail mirrors, X laser switch, F fire EMP, R restart after win/loss.";
            }
            else if (sceneName == "Level02")
            {
                title = "LEVEL 2 - DUNGEON SWITCH ROOM:";
                body =
                    "1. The receiver requires exactly 2 unique mirror reflections before it can power on.\n" +
                    "2. The door opens only while the receiver is powered.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, X toggle laser, Q/E rotate mirrors, Z/C slide rail mirrors, R restart after win/loss.";
            }
            else
            {
                title = "LEVEL 1 - CASTLE ENTRANCE:";
                body =
                    "1. Approach the mirror and press Q / E to rotate it.\n" +
                    "2. The receiver requires exactly 1 mirror reflection before it can power on.\n" +
                    "3. Avoid the AI guard and reach the green exit zone.\n" +
                    "Controls: WASD move, Mouse look, Space jump, Q/E rotate mirrors, Z/C slide rail mirrors, R restart after win/loss.";
            }
        }

        string objectiveText = "<color=#FFD45A>" + title + "</color>\n" + body;
        float objectiveWidth = Mathf.Min(560f, Screen.width * 0.48f);
        float objectiveHeight = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(objectiveText, objectiveWidth), 120f, 245f);
        CartoonGUI.DrawBox(new Rect(15f, 15f, objectiveWidth, objectiveHeight), objectiveText);

        if (messageTimer > 0f && !string.IsNullOrEmpty(temporaryMessage))
        {
            float messageWidth = Mathf.Min(620f, Screen.width * 0.70f);
            float messageHeight = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(temporaryMessage, messageWidth), 70f, 150f);
            CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - messageWidth / 2f, Screen.height / 2f - messageHeight / 2f, messageWidth, messageHeight), temporaryMessage);
        }
    }
}
