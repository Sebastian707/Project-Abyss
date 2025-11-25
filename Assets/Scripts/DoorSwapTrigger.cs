using UnityEngine;

public class DoorSwapTrigger : MonoBehaviour
{
    [Header("Door that should CLOSE + LOCK")]
    public DoorLock doorToCloseAndLock;

    [Header("Door that should OPEN")]
    public DoorLock doorToOpen;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // ---- CLOSE & LOCK FIRST DOOR ----
            if (doorToCloseAndLock != null)
            {
                doorToCloseAndLock.CloseAndLock();
            }

            // ---- OPEN SECOND DOOR ----
            if (doorToOpen != null)
            {
                // Unlock first
                doorToOpen.UnlockDoor();
                // Then open via ToggleDoor() (anim + sound)
                doorToOpen.ToggleDoor();
            }
        }
    }
}
