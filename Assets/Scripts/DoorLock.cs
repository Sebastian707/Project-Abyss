using UnityEngine;
using UnityEngine.AI;  

public class DoorLock : Interactable
{
    [Header("Keypad Settings")]
    public string requiredKeyID = "LabDoor";   
    public GameObject doorToUnlock;           
    private bool isLocked = true;
    private bool doorOpen;

    private TetrisSlot playerSlot;
    private NavMeshObstacle obstacle; 

    void Start()
    {
        playerSlot = FindObjectOfType<TetrisSlot>();
        if (playerSlot == null)
            Debug.LogError("No TetrisSlot found in scene!");


        if (doorToUnlock != null)
            obstacle = doorToUnlock.GetComponent<NavMeshObstacle>();
    }

    protected override void Interact()
    {
        if (playerSlot == null) return;

        foreach (TetrisItemSlot slot in playerSlot.itensInBag)
        {
            if (slot != null && slot.item != null)
            {
                Key key = slot.item as Key;
                if (key != null && key.keyID == requiredKeyID)
                {
                    UnlockDoor();
                    return;
                }
            }
        }

        UIDoorManager.Instance.ShowMessage("Access Denied: Missing key with ID: " + requiredKeyID);
    }

    private void UnlockDoor()
    {
        if (!isLocked) return;

        isLocked = false;
        UIDoorManager.Instance.ShowMessage("Access Granted! Correct key found.");

        if (doorToUnlock != null)
        {
            doorOpen = !doorOpen;
            doorToUnlock.GetComponent<Animator>().SetBool("IsOpen", doorOpen);

            if (obstacle != null)
                obstacle.enabled = !doorOpen; 
        }
        else
        {
            Debug.LogWarning("No door assigned to keypad!");
        }
    }
}
