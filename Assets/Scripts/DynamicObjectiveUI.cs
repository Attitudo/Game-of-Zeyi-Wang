using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicObjectiveUI : MonoBehaviour
{
    private static DynamicObjectiveUI instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("DynamicObjectiveUI");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<DynamicObjectiveUI>();
    }

    private void OnGUI()
    {
        if (!GlobalMenuUI.HelpVisible || GlobalMenuUI.GameplayBlocked)
        {
            return;
        }

        string text = BuildObjectiveText();
        float w = Mathf.Min(570f, Screen.width * 0.48f);
        float h = Mathf.Clamp(CartoonGUI.GetWrappedBoxHeight(text, w), 120f, 230f);
        CartoonGUI.DrawBox(new Rect(15f, 15f, w, h), text);
    }

    private string BuildObjectiveText()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        int reflected = 0;
        int required = 0;
        bool laserOn = false;
        if (LightReflection.Instance != null)
        {
            reflected = LightReflection.Instance.CurrentMirrorReflections;
            required = LightReflection.Instance.RequiredMirrorReflections;
            laserOn = LightReflection.Instance.laserEnabled;
        }

        Receiver receiver = Object.FindObjectOfType<Receiver>();
        bool receiverPowered = receiver != null && receiver.powered;

        DoorAnimator door = Object.FindObjectOfType<DoorAnimator>();
        bool doorOpen = door != null && door.IsOpen;

        string levelTitle = GetLevelTitle(sceneName);

        string currentStep;
        if (sceneName == "MainScene")
        {
            if (reflected < required)
            {
                currentStep = "Tutorial: rotate the mirror with Q / E until the laser reaches the receiver.";
            }
            else if (!receiverPowered)
            {
                currentStep = "Tutorial: fine tune the mirror so the receiver stays powered.";
            }
            else if (!doorOpen)
            {
                currentStep = "Tutorial: wait for the gate to open.";
            }
            else
            {
                currentStep = "Tutorial complete: walk through the gate.";
            }
        }
        else
        {
            if (!laserOn)
            {
                currentStep = "Step 1: find the switch and press X to turn on the lamp.";
            }
            else if (reflected < required)
            {
                currentStep = "Step 2: slide and rotate mirrors. Reflection count is still too low.";
            }
            else if (reflected > required)
            {
                currentStep = "Step 2: too many mirrors are reflecting. Avoid decoy routes.";
            }
            else if (!receiverPowered)
            {
                currentStep = "Step 3: correct mirror count reached. Aim the beam into the receiver.";
            }
            else if (!doorOpen)
            {
                currentStep = "Step 4: receiver powered. Wait for the gate.";
            }
            else
            {
                currentStep = "Step 5: gate open. Reach the exit.";
            }
        }

        return
            "<color=#FFD45A>" + levelTitle + "</color>\n" +
            currentStep + "\n\n" +
            "Reflections: " + reflected + " / " + required + "\n" +
            "Receiver: " + (receiverPowered ? "Powered" : "Not powered") + "\n" +
            "Gate: " + (doorOpen ? "Open" : "Closed") + "\n" +
            "H: hide help";
    }

    private string GetLevelTitle(string sceneName)
    {
        if (sceneName == "Level06") return "LEVEL 6 - FINAL CATACOMB";
        if (sceneName == "Level05") return "LEVEL 5 - EXPERT MAZE";
        if (sceneName == "Level04") return "LEVEL 4 - MIRROR CHAIN";
        if (sceneName == "Level03") return "LEVEL 3 - GUARD VAULT";
        if (sceneName == "Level02") return "LEVEL 2 - SWITCH ROOM";
        return "LEVEL 1 - TUTORIAL ENTRANCE";
    }
}
