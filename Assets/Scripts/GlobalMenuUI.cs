using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMenuUI : MonoBehaviour
{
    private enum MenuMode
    {
        None,
        Main,
        Pause,
        LevelSelect,
        Controls
    }

    private static GlobalMenuUI instance;
    private static bool helpVisible = false;
    private static bool gameStarted = false;
    private MenuMode mode = MenuMode.Main;

    public static bool HelpVisible
    {
        get { return helpVisible; }
    }

    public static bool GameplayBlocked
    {
        get { return instance != null && instance.mode != MenuMode.None; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("GlobalMenuUI");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<GlobalMenuUI>();
    }

    private void Start()
    {
        if (!gameStarted)
        {
            OpenMainMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            helpVisible = !helpVisible;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mode == MenuMode.None)
            {
                OpenPauseMenu();
            }
            else if (mode == MenuMode.Pause || mode == MenuMode.Controls || mode == MenuMode.LevelSelect)
            {
                CloseMenus();
            }
        }
    }

    private void OpenMainMenu()
    {
        mode = MenuMode.Main;
        helpVisible = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OpenPauseMenu()
    {
        if (!gameStarted)
        {
            OpenMainMenu();
            return;
        }

        mode = MenuMode.Pause;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseMenus()
    {
        mode = MenuMode.None;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void StartGame(string sceneName)
    {
        gameStarted = true;
        helpVisible = false;
        mode = MenuMode.None;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnGUI()
    {
        GUI.skin.button.font = CartoonGUI.GetCartoonFont();
        GUI.skin.button.fontSize = 17;
        GUI.skin.button.fontStyle = FontStyle.Bold;
        GUI.skin.button.wordWrap = true;

        DrawSmallRuntimeHint();

        if (mode == MenuMode.None)
        {
            return;
        }

        GUI.color = new Color(0f, 0f, 0f, 0.62f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (mode == MenuMode.Main)
        {
            DrawMainMenu();
        }
        else if (mode == MenuMode.Pause)
        {
            DrawPauseMenu();
        }
        else if (mode == MenuMode.LevelSelect)
        {
            DrawLevelSelect();
        }
        else if (mode == MenuMode.Controls)
        {
            DrawControls();
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void DrawSmallRuntimeHint()
    {
        if (mode != MenuMode.None)
        {
            return;
        }

        string text = helpVisible ? "H: Hide Help    ESC: Pause" : "H: Help    ESC: Pause";
        CartoonGUI.DrawSmallBox(new Rect(15f, Screen.height - 52f, 245f, 38f), text);
    }

    private void DrawMenuBackdrop(Rect panel)
    {
        Color old = GUI.color;
        GUI.color = new Color(0.02f, 0.018f, 0.012f, 0.86f);
        GUI.DrawTexture(panel, Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.80f, 0.25f, 0.22f);
        GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panel.x, panel.yMax - 4f, panel.width, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panel.x, panel.y, 4f, panel.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panel.xMax - 4f, panel.y, 4f, panel.height), Texture2D.whiteTexture);
        GUI.color = old;
    }

    private void DrawMenuTitle(Rect rect, string title, string subtitle)
    {
        CartoonGUI.DrawCenterBox(rect, "<color=#FFD45A>" + title + "</color>\n" + subtitle);
    }

    private void DrawMainMenu()
    {
        Rect full = new Rect(0f, 0f, Screen.width, Screen.height);

        Color old = GUI.color;
        GUI.color = new Color(0.015f, 0.012f, 0.008f, 0.96f);
        GUI.DrawTexture(full, Texture2D.whiteTexture);

        // Subtle golden frame, full-screen style.
        GUI.color = new Color(1f, 0.78f, 0.22f, 0.35f);
        GUI.DrawTexture(new Rect(24f, 24f, Screen.width - 48f, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(24f, Screen.height - 28f, Screen.width - 48f, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(24f, 24f, 4f, Screen.height - 48f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(Screen.width - 28f, 24f, 4f, Screen.height - 48f), Texture2D.whiteTexture);
        GUI.color = old;

        float titleW = Mathf.Min(720f, Screen.width * 0.78f);
        float titleH = 92f;
        float titleY = Mathf.Max(42f, Screen.height * 0.13f);
        DrawMenuTitle(new Rect(Screen.width / 2f - titleW / 2f, titleY, titleW, titleH),
            "MIRROR PUZZLE GUARD",
            "Cartoon Dungeon Laser Puzzle");

        float bw = Mathf.Min(310f, Screen.width * 0.38f);
        float bh = 50f;
        float x = Screen.width / 2f - bw / 2f;
        float y = titleY + 132f;
        float gap = 62f;

        if (GUI.Button(new Rect(x, y, bw, bh), "Start Game"))
        {
            StartGame("MainScene");
        }
        if (GUI.Button(new Rect(x, y + gap, bw, bh), "Level Select"))
        {
            mode = MenuMode.LevelSelect;
        }
        if (GUI.Button(new Rect(x, y + gap * 2f, bw, bh), "Controls"))
        {
            mode = MenuMode.Controls;
        }
        if (GUI.Button(new Rect(x, y + gap * 3f, bw, bh), "Quit"))
        {
            QuitGame();
        }

        string hint = "Use mirrors, lamps, and EMP to escape.";
        float hintW = Mathf.Min(620f, Screen.width * 0.72f);
        CartoonGUI.DrawCenterBox(new Rect(Screen.width / 2f - hintW / 2f, Screen.height - 94f, hintW, 50f), hint);
    }

    private void DrawPauseMenu()
    {
        float w = 500f;
        float h = 420f;
        Rect panel = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);
        DrawMenuBackdrop(panel);

        CartoonGUI.DrawCenterBox(new Rect(panel.x + 70f, panel.y + 28f, panel.width - 140f, 44f),
            "<color=#FFD45A>PAUSED</color>");
        CartoonGUI.DrawCenterBox(new Rect(panel.x + 70f, panel.y + 78f, panel.width - 140f, 44f),
            "Take a breath and solve the mirrors.");

        float bw = 260f;
        float bh = 44f;
        float x = Screen.width / 2f - bw / 2f;
        float y = panel.y + 145f;

        if (GUI.Button(new Rect(x, y, bw, bh), "Resume"))
        {
            CloseMenus();
        }
        if (GUI.Button(new Rect(x, y + 58f, bw, bh), "Restart Level"))
        {
            StartGame(SceneManager.GetActiveScene().name);
        }
        if (GUI.Button(new Rect(x, y + 116f, bw, bh), "Level Select"))
        {
            mode = MenuMode.LevelSelect;
        }
        if (GUI.Button(new Rect(x, y + 174f, bw, bh), "Main Menu"))
        {
            gameStarted = false;
            OpenMainMenu();
        }
    }

    private void DrawLevelSelect()
    {
        float w = 660f;
        float h = 470f;
        Rect panel = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);
        DrawMenuBackdrop(panel);
        DrawMenuTitle(new Rect(panel.x + 65f, panel.y + 28f, panel.width - 130f, 72f),
            "LEVEL SELECT",
            "Choose a dungeon trial.");

        string[] scenes = { "MainScene", "Level02", "Level03", "Level04", "Level05", "Level06" };
        string[] names =
        {
            "Level 1 - Tutorial Entrance",
            "Level 2 - Switch Room",
            "Level 3 - Guard Vault",
            "Level 4 - Mirror Chain",
            "Level 5 - Expert Maze",
            "Level 6 - Final Catacomb"
        };

        float bw = 265f;
        float bh = 42f;
        float startX = panel.x + 55f;
        float startY = panel.y + 120f;

        for (int i = 0; i < scenes.Length; i++)
        {
            float x = startX + (i % 2) * 295f;
            float y = startY + (i / 2) * 64f;

            if (GUI.Button(new Rect(x, y, bw, bh), names[i]))
            {
                StartGame(scenes[i]);
            }
        }

        if (GUI.Button(new Rect(Screen.width / 2f - 100f, panel.y + h - 68f, 200f, 42f), "Back"))
        {
            mode = gameStarted ? MenuMode.Pause : MenuMode.Main;
        }
    }

    private void DrawControls()
    {
        float w = 650f;
        float h = 455f;
        Rect panel = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);
        DrawMenuBackdrop(panel);

        string text =
            "<color=#FFD45A>CONTROLS</color>\n" +
            "WASD: Move    Mouse: Look\n" +
            "Q / E: Rotate the nearest mirror\n" +
            "Z / C: Slide the nearest rail mirror\n" +
            "X: Toggle lamp switch from Level 2 onward\n" +
            "F: Fire EMP when you have the device\n" +
            "H: Show or hide objectives and interaction hints\n" +
            "ESC: Pause menu\n\n" +
            "Goal: guide the laser through exactly the required number of mirrors, power the receiver, open the gate, and escape.";

        CartoonGUI.DrawBox(new Rect(panel.x + 45f, panel.y + 35f, panel.width - 90f, panel.height - 120f), text);

        if (GUI.Button(new Rect(Screen.width / 2f - 100f, panel.y + h - 62f, 200f, 42f), "Back"))
        {
            mode = gameStarted ? MenuMode.Pause : MenuMode.Main;
        }
    }

}