using UnityEngine;
using UnityEngine.AI;
using Unity;

public class DoorLock : Interactable
{
    public string requiredKeyID = "LabDoor";
    public GameObject doorToUnlock;
    [SerializeField] private bool isLocked = true;
    private bool doorOpen;
    public AudioClip DoorSound;
    private AudioSource audioSource;

    private TetrisSlot playerSlot;
    private NavMeshObstacle obstacle;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        playerSlot = FindObjectOfType<TetrisSlot>();
        if (playerSlot == null)
            Debug.LogError("No TetrisSlot found in scene!");

        if (doorToUnlock != null)
            obstacle = doorToUnlock.GetComponent<NavMeshObstacle>();
    }

    protected override void Interact()
    {
        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

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

        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (doorToUnlock == null)
        {
            Debug.LogWarning("No door assigned to keypad!");
            return;
        }

        doorOpen = !doorOpen;
        doorToUnlock.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
        audioSource.PlayOneShot(DoorSound);

        if (obstacle != null)
            obstacle.enabled = !doorOpen;

        string message = doorOpen ? "Door opened." : "Door closed.";
        UIDoorManager.Instance.ShowMessage(message);
    }
}
