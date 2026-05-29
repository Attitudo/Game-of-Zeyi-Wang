using UnityEngine;

public class Receiver : MonoBehaviour
{
    public GameObject door;
    private bool isActivated = false;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            door.SetActive(false);
            Debug.Log("Receiver activated! Door opened!");
        }
    }
}