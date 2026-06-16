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
        GameAudio.PlayCaught();
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
        if (!GlobalMenuUI.GameplayBlocked && messageTimer > 0f && !string.IsNullOrEmpty(temporaryMessage))
        {
            float messageWidth = Mathf.Min(520f, Screen.width * 0.58f);
            float messageHeight = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(temporaryMessage, messageWidth), 52f, 105f);
            CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - messageWidth / 2f, 205f, messageWidth, messageHeight), temporaryMessage);
        }
    }
}
