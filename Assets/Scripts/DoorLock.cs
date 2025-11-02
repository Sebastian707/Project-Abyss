using UnityEngine;
using UnityEngine.AI;
using Unity;
using System.Collections;

public class DoorLock : Interactable
{
    public string requiredKeyID = "LabDoor";
    public GameObject doorToUnlock;
    [SerializeField] private bool isLocked = true;
    private bool doorOpen;
    public AudioClip DoorSound;
    public AudioClip DoorLockedSound;
    public AudioClip DoorUnlockClick;
    private AudioSource audioSource;

    private TetrisSlot playerSlot;
    private NavMeshObstacle obstacle;

    [SerializeField] private float jiggleAmount = 5f;
    [SerializeField] private float jiggleDuration = 0.1f;
    private bool isJiggling = false;

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

        UIInfoManager.Instance.ShowMessage("Access Denied: Missing key with ID: " + requiredKeyID);
        audioSource.PlayOneShot(DoorLockedSound);
        StartCoroutine(JiggleDoor());
    }

    private void UnlockDoor()
    {
        if (!isLocked) return;

        if (isLocked)
        {
            isLocked = false;
            audioSource.PlayOneShot(DoorUnlockClick);
            UIInfoManager.Instance.ShowMessage("Correct key found");
        }
        else
        {
            ToggleDoor();
        }

            
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
        UIInfoManager.Instance.ShowMessage(message);
    }

    private IEnumerator JiggleDoor()
    {
        if (isJiggling || doorToUnlock == null) yield break;
        isJiggling = true;

        Transform doorTransform = doorToUnlock.transform;
        Quaternion originalRot = doorTransform.localRotation;

        float elapsed = 0f;
        while (elapsed < jiggleDuration)
        {
            elapsed += Time.deltaTime;
            float strength = Mathf.Sin(elapsed * 40f) * jiggleAmount;
            doorTransform.localRotation = originalRot * Quaternion.Euler(0f, strength, 0f);
            yield return null;
        }

        doorTransform.localRotation = originalRot;
        isJiggling = false;
    }
}
