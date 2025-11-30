using UnityEngine;

public class HallwayEntryTrigger : MonoBehaviour
{
    [HideInInspector] public DoorLock doorBehind; // Assigned dynamically by HallwayManager
    public Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
            Debug.LogError("HallwayEntryTrigger requires a Collider set as trigger!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"doorBehind assigned? {doorBehind != null}");

        if (other.CompareTag("Player") && doorBehind != null)
        {
            // Close and lock the previous door
            doorBehind.CloseAndLock();

            // Disable the trigger so it only happens once
            if (triggerCollider != null)
                triggerCollider.enabled = false;
        }
    }
}
