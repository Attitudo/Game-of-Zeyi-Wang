using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    [Header("Scene Transition")]
    public string nextSceneName = "Level02";
    public bool isFinalLevel = false;

    [Header("Exit Requirements")]
    public int requiredKeycards = 0;
    public int requiredEnergyCores = 0;
    public bool requiresEmpDevice = false;

    [Header("Door Requirement")]
    [Tooltip("Leave this false for puzzle levels. The exit will stay locked until the security door is opened by the receiver.")]
    public bool allowExitWithoutDoor = false;
    public DoorAnimator requiredDoor;

    [Header("Locked Exit Feedback")]
    public float lockedPushBackDistance = 0.55f;

    [Header("Final Level Message")]
    [TextArea(2, 4)] public string finalLevelMessage = "All levels complete. Press R to replay this level.";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        TryExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // If the player waits on the exit pad, the level can complete as soon as
        // the receiver opens the door. This also prevents one-frame trigger issues.
        TryExit(other);
    }

    private void TryExit(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (!RequirementsMet(inventory))
        {
            PushPlayerOutOfLockedExit(other);
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

    private bool RequirementsMet(PlayerInventory inventory)
    {
        if (!DoorRequirementMet())
        {
            ShowRequirementMessage("Exit locked: power the receiver and open the security door first.");
            return false;
        }

        if (requiredKeycards <= 0 && requiredEnergyCores <= 0 && !requiresEmpDevice)
        {
            return true;
        }

        if (inventory == null)
        {
            ShowRequirementMessage("You need the required items before leaving this level.");
            return false;
        }

        if (inventory.keycards < requiredKeycards)
        {
            ShowRequirementMessage("Exit locked: find the security keycard.");
            return false;
        }

        if (inventory.energyCores < requiredEnergyCores)
        {
            ShowRequirementMessage("Exit locked: collect all energy cores.");
            return false;
        }

        if (requiresEmpDevice && !inventory.hasEmpDevice)
        {
            ShowRequirementMessage("Exit locked: pick up the EMP device first.");
            return false;
        }

        return true;
    }

    private bool DoorRequirementMet()
    {
        if (allowExitWithoutDoor)
        {
            return true;
        }

        // Puzzle exits are locked by the optical receiver, not merely by the
        // player touching the green pad. This prevents bypassing the door by
        // stepping on ExitZone before the laser solution is complete.
        Receiver[] receivers = Object.FindObjectsOfType<Receiver>();
        if (receivers != null && receivers.Length > 0)
        {
            foreach (Receiver receiver in receivers)
            {
                if (receiver != null && !receiver.powered)
                {
                    return false;
                }
            }

            return true;
        }

        DoorAnimator door = requiredDoor;
        if (door == null)
        {
            door = Object.FindObjectOfType<DoorAnimator>();
        }

        if (door != null)
        {
            return door.IsOpen;
        }

        // Non-puzzle fallback only.
        return true;
    }

    private void PushPlayerOutOfLockedExit(Collider other)
    {
        if (lockedPushBackDistance <= 0f || other == null)
        {
            return;
        }

        Vector3 direction = other.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller != null && controller.enabled)
        {
            controller.Move(direction * lockedPushBackDistance);
        }
        else
        {
            other.transform.position += direction * lockedPushBackDistance;
        }
    }

    private void ShowRequirementMessage(string message)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage(message, 1.2f);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
