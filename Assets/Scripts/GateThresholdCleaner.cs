using UnityEngine;

public class GateThresholdCleaner : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CleanOnLoad()
    {
        string[] keywords =
        {
            "_CleanDoor_LowerStoneLip",
            "_CleanDoor_FrontStep",
            "_CleanDoor_WalkRamp",
            "_CleanDoor_InnerPlatform",
            "_CleanDoor_FloorBridge"
        };

        foreach (Transform t in Object.FindObjectsOfType<Transform>())
        {
            if (t == null || t.gameObject == null)
            {
                continue;
            }

            string n = t.gameObject.name;
            foreach (string key in keywords)
            {
                if (n.Contains(key))
                {
                    t.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
